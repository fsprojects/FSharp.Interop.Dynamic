//
//  Copyright 2011  Ekon Benefits
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.


namespace FSharp.Interop.Dynamic
open System

type Calling = 
    | GenericMember of string * Type array 
    | Member of string
    | Direct

module Dyn =
    open Dynamitey
    open Microsoft.CSharp.RuntimeBinder
    open FSharp.Reflection

    let staticContext (target:Type) = InvokeContext.CreateStatic.Invoke(target)

    let staticTarget<'TTarget> = InvokeContext.CreateStatic.Invoke(typeof<'TTarget>)
  
    let implicitConvertTo (convertType:Type) (target:obj) : 'TResult  = 
        match target with 
        | null -> target
        | t -> Dynamic.InvokeConvert(t, convertType, explicit = false) 
        |> unbox<'TResult>
        
    
    let implicitConvert(target:obj) : 'TResult  =
        implicitConvertTo typeof<'TResult> target

   
    let explicitConvertTo (convertType:Type) (target:obj) : 'TResult  = 
        Dynamic.InvokeConvert(target, convertType, explicit = true) |> unbox<'TResult>

    let explicitConvert (target:obj) : 'TResult  = 
        explicitConvertTo typeof<'TResult> target

    let namedArg (name:string) (argValue:obj) =
        InvokeArg(name, argValue)

    let memberAddAssign (memberName:string) (value:obj) (target:obj) =
        Dynamic.InvokeAddAssignMember(target, memberName, value)
    
    
    let memberSubtractAssign (memberName:string) (value:obj) (target:obj) =
        Dynamic.InvokeSubtractAssignMember(target, memberName, value)

  
    let invocation (target:obj) (memberName:Calling)  : 'TResult =
        let resultType = typeof<'TResult>
        //Helper to dynamically call call FSharpFuncs
        let fsharpInvoke target' memberName' arg' =
            let invokeName = InvokeMemberName("Invoke", null) //FSharpFunc Invoke
            let invokeContext t = InvokeContext(t, typeof<obj>) //Improve cache hits by using the same context
            let start = match memberName' with
                        | GenericMember (name, _)
                        | Member name -> Dynamic.InvokeGet(target', name)
                        | Direct -> target'
            Dynamic.InvokeMember(invokeContext(start), invokeName, [|arg'|])
        let (|NoConversion| Conversion|) t = 
            if t = typeof<obj> then NoConversion else Conversion
        let finalConvertResult finalType result :'TResult = 
            match finalType with
            | x when FSharpType.IsFunction x -> // if return type is a function
                let rec curriedLambda target type' arg' = 
                    let result' = fsharpInvoke target Direct arg'
                    let _,retType = FSharpType.GetFunctionElements type'
                    if FSharpType.IsFunction retType then
                        FSharpValue.MakeFunction(retType, curriedLambda result' retType)
                    else
                        result'
                FSharpValue.MakeFunction(finalType, curriedLambda result finalType)
            | NoConversion -> result
            | Conversion -> implicitConvert result
            |> unbox
        if not (FSharpType.IsFunction resultType)
        then
            match memberName with
            | GenericMember (name, _)
            | Member name -> Dynamic.InvokeGet(target, name)
            | Direct -> target
            |> finalConvertResult resultType
        else
            let lambda (arg:obj) =
                let argType,returnType = FSharpType.GetFunctionElements resultType
                let argArray =
                    match argType with
                    | a when FSharpType.IsTuple(a) -> FSharpValue.GetTupleFields(arg)
                    | a when a = typeof<unit>      -> [| |]
                    | ____________________________ -> [|arg|]
                let invoker k = 
                    let memberName =
                         memberName |> function | GenericMember (name, targs) ->
                                                    InvokeMemberName(name, targs)
                                                | Member name -> 
                                                    InvokeMemberName(name, null)
                                                | Direct -> null
                    Invocation(k, memberName).Invoke(target, argArray)
                let (|Action|Func|) t = if t = typeof<unit> then Action else Func
                let result =
                    try //Either it has a member or it's something directly callable
                        match (returnType, memberName) with
                        | (Action, Direct) -> invoker(InvocationKind.InvokeAction)
                        | (Action, GenericMember _)
                        | (Action, Member _) -> invoker(InvocationKind.InvokeMemberAction)
                        | (Func, Direct) -> invoker(InvocationKind.Invoke)
                        | (Func, GenericMember _)
                        | (Func, Member _) -> invoker(InvocationKind.InvokeMember)
                    with  //Last chance incase we are trying to invoke an fsharpfunc
                        |  :? RuntimeBinderException as e  ->
                            try
                                fsharpInvoke target memberName arg
                            with
                                | :? RuntimeBinderException as e2
                                   -> AggregateException(e, e2) |> raise
                match returnType with
                | Action | NoConversion -> result
                | _____________________ -> result |> finalConvertResult returnType
            FSharpValue.MakeFunction(resultType, lambda) |> unbox<'TResult>

    let invokeDirect value (target:obj) : 'TResult =
        invocation target Direct value

   
    let invokeMember (memberName:string) value (target:obj) : 'TResult =
        invocation target (Member memberName) value


    let invokeGeneric (memberName:string) (typeArgs:Type seq) value (target:obj) : 'TResult =
        let typeArgs' = typeArgs |> Array.ofSeq
        let genericMember = GenericMember (memberName, typeArgs')
        invocation target genericMember value

    let get (propertyName:string) (target:obj) : 'TResult =
        invocation target (Member propertyName)

    let getChain (chainOfMembers:string seq) (target:obj) : 'TResult =
        let chainOfMembers' = String.concat "." chainOfMembers 
        Dynamic.InvokeGetChain(target, chainOfMembers') |> invocation <| Direct
 
   
    let getIndexer (indexers: 'T seq) (target:obj): 'TResult =
        let indexes = indexers |> Seq.map box  |> Seq.toArray
        Dynamic.InvokeGetIndex(target, indexes) |> invocation <| Direct

    let set (propertyName:string) (value:obj) (target:obj) =
        Dynamic.InvokeSet(target, propertyName, value) |> ignore

    let setChain (chainOfMembers: string seq) (value:obj) (target:obj) =
        let chainOfMembers' = String.concat "." chainOfMembers 
        Dynamic.InvokeSetChain(target, chainOfMembers', value) |> ignore

    /// dynamically call set index
    let setIndexer (indexers: 'T seq) (value:obj) (target:obj) =
        let indexes = indexers |> Seq.map box |> Seq.toArray
        Dynamic.InvokeSetValueOnIndexes(target, value, indexes) |> ignore

    [<Obsolete("Replaced with partial application version `getIndexer`")>]
    let getIndex (target:obj) (indexers: 'T seq) : 'TResult =
        target |> getIndexer indexers

    [<Obsolete("Replaced with partial application version `setIndexer`")>]
    let setIndex (target:obj) (indexers: 'T seq) (value:obj)  =
        target |> setIndexer indexers value

    [<Obsolete("Replaced with partial application version `memberAddAssign`")>]
    let addAssignMember (target:obj) (memberName:string) (value:obj)  =
        target |> memberAddAssign memberName value
    
    [<Obsolete>]
    let subtractAssignMember (target:obj) (memberName:string) (value:obj)  =
        target |> memberSubtractAssign memberName value

    [<Obsolete("Replaced with `invocation`")>]
    let invoke (target:obj) (memberName:string option) : 'TResult =
        let memberOrDirect = match memberName with | Some mn -> Member mn | None -> Direct
        invocation target memberOrDirect