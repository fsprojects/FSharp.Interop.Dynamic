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

///Functions backing the operators and more
module Dyn =
    open Dynamitey
    open Microsoft.CSharp.RuntimeBinder
    open Microsoft.FSharp.Reflection

    ///allow access to static context for dynamic invocation of static methods
    let staticContext (target:Type) = InvokeContext.CreateStatic.Invoke(target)

    let staticTarget<'TTarget> = InvokeContext.CreateStatic.Invoke(typeof<'TTarget>)
  
    ///implict convert via reflected type
    let implicitConvertTo (convertType:Type) (target:obj) : 'TResult  = 
        Dynamic.InvokeConvert(target, convertType, explicit = false) |> unbox<'TResult>

    ///implict convert via inferred type
    let implicitConvert(target:obj) : 'TResult  = 
        implicitConvertTo typeof<'TResult> target

    ///explicit convert via reflected type
    let explicitConvertTo (convertType:Type) (target:obj) : 'TResult  = 
        Dynamic.InvokeConvert(target, convertType, explicit = true) |> unbox<'TResult>

    ///explicit convert via inferred type
    let explicitConvert (target:obj) : 'TResult  = 
        explicitConvertTo typeof<'TResult> target

    ///allow marking args with names for dlr invoke
    let namedArg (name:string) (argValue:obj) =
        InvokeArg(name, argValue)

    ///Dynamically call `+=` on member
    let memberAddAssign (memberName:string) (value:obj) (target:obj) =
        Dynamic.InvokeAddAssignMember(target, memberName, value)
    
    ///Dynamically call `-=` on member
    let memberSubtractAssign (memberName:string) (value:obj) (target:obj) =
        Dynamic.InvokeSubtractAssignMember(target, memberName, value)

    /// main workhouse method; Some(methodName) or just None to invoke without name;
    /// infered casting with automatic implicit convert.
    let invocation (memberName:Calling) (target:obj)  : 'TResult =
        let resultType = typeof<'TResult>
        let (|NoConversion| Conversion|) t = 
            if t = typeof<obj> then NoConversion else Conversion
        let finalConvertResult finalType r :'TResult = 
            match finalType with
            | NoConversion -> r
            | Conversion -> implicitConvert r
            |> unbox
        if not (FSharpType.IsFunction resultType)
        then
            match memberName with
            | GenericMember (name, _)
            | Member name -> Dynamic.InvokeGet(target, name)
            | Direct -> target
            |> finalConvertResult resultType
        else
            let lambda arg =
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
               let (|Action|Func|) t =
                    if t = typeof<unit> then Action else Func
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
                                let invokeName = InvokeMemberName("Invoke", null) //FSharpFunc Invoke
                                let invokeContext t = InvokeContext(t, typeof<obj>) //Improve cache hits by using the same context
                                let invokeFSharpFold t (a:obj) =
                                    Dynamic.InvokeMember(invokeContext(t), invokeName,a)
                                let seed = match memberName with
                                           | GenericMember (name, _)
                                           | Member name -> Dynamic.InvokeGet(target, name)
                                           | Direct       -> target
                                Array.fold invokeFSharpFold seed argArray
                            with
                                | :? RuntimeBinderException as e2
                                     -> AggregateException(e, e2) |> raise
               match returnType with
               | Action | NoConversion -> result
               | _____________________ -> result |> finalConvertResult returnType
            FSharpValue.MakeFunction(resultType,lambda) |> unbox<'TResult>

    //allows result to be called like a function
    let invokeDirect (target:obj) : 'TResult =
        target |> invocation Direct

    //calls member whose result can be called like a function
    let invokeMember (memberName:string) (target:obj) : 'TResult =
        target |> invocation (Member memberName)

    //calls member and specify's generic parameters and whose result can be called like a function
    let invokeGeneric (memberName:string) (typeArgs:Type seq) (target:obj) : 'TResult =
        let typeArgs' = typeArgs |> Array.ofSeq
        let genericMember = GenericMember (memberName, typeArgs')
        target |> invocation genericMember

    let get (propertyName:string) (target:obj) : 'TResult =
        target |> invocation (Member propertyName)

    let getChain (chainOfMembers:string seq) (target:obj) : 'TResult =
        let chainOfMembers' = String.concat "." chainOfMembers 
        Dynamic.InvokeGetChain(target, chainOfMembers') |> invocation Direct
 
    ///dynamically call get index
    let getIndexer (indexers: 'T seq) (target:obj): 'TResult =
        let indexes = indexers |> Seq.map box  |> Seq.toArray
        Dynamic.InvokeGetIndex(target, indexes) |> invocation Direct

    let set (propertyName:string) (value:obj) (target:obj) =
        Dynamic.InvokeSet(target, propertyName, value) |> ignore

    let setChain (chainOfMembers: string seq) (value:obj) (target:obj) =
        let chainOfMembers' = String.concat "." chainOfMembers 
        Dynamic.InvokeSet(target, chainOfMembers', value) |> ignore

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

    [<Obsolete("Replaced with partial application version `invocation`")>]
    let invoke (target:obj) (memberName:string option) : 'TResult =
        let memberOrDirect = match memberName with | Some mn -> Member mn | None -> Direct
        target |> invocation memberOrDirect