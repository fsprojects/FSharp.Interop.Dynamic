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

module Dyn=
    open System
    open Dynamitey
    open Microsoft.CSharp.RuntimeBinder
    open Microsoft.FSharp.Reflection

    

    ///allow access to static context for dynamic invocation of static methods
    let staticContext (target:Type) = InvokeContext.CreateStatic.Invoke(target)


  
    ///implict convert via reflected type
    let implicitConvertTo (convertType:Type) (target:obj) : 'TResult  = 
        Dynamic.InvokeConvert(target, convertType, explicit = false) :?> 'TResult

    ///implict convert via inferred type
    let implicitConvert(target:obj) : 'TResult  = 
      implicitConvertTo typeof<'TResult> target

    ///explicit convert via reflected type
    let explicitConvertTo (convertType:Type) (target:obj) : 'TResult  = 
        Dynamic.InvokeConvert(target, convertType, explicit = true) :?> 'TResult

    ///explicit convert via inferred type
    let explicitConvert (target:obj) : 'TResult  = 
        explicitConvertTo typeof<'TResult> target

    //allow marking args with names for dlr invoke
    let namedArg (name:string) (argValue:obj) =
        InvokeArg(name, argValue)

    ///Dynamically call `+=` on member
    let addAssignMember (target:obj) (memberName:string) (value:obj)  =
        Dynamic.InvokeAddAssignMember(target, memberName, value)

     ///Dynamically call `-=` on member
    let subtractAssignMember (target:obj) (memberName:string) (value:obj)  =
        Dynamic.InvokeSubtractAssignMember(target, memberName, value)

    //dynamically call get index
    let getIndex (target:obj) (indexers: 'T seq) : 'TResult =
        Dynamic.InvokeGetIndex(target, (indexers |> Seq.map box  |> Seq.toArray)) :?> 'TResult
    // dynamically call set index
    let setIndex (target:obj) (indexers: 'T seq) (value:obj)  =
        Dynamic.InvokeSetValueOnIndexes(target, value, (indexers |> Seq.map box |> Seq.toArray)) |> ignore

    // main workhouse method; Some(methodName) or just None to invoke without name;
    // infered casting with automatic implicit convert.
    let invoke (target:obj) (memberName:string option) : 'TResult =
        let resultType = typeof<'TResult>
        let (|NoConversion| Conversion|) t = if t = typeof<obj> then NoConversion else Conversion

        if not (FSharpType.IsFunction resultType)
        then
            match memberName with
              | Some (name) ->
                let convert r = match resultType with
                                    | NoConversion -> r
                                    | ____________ -> implicitConvert r
                Dynamic.InvokeGet(target, name)
                    |> convert
                    |> unbox
              | None -> implicitConvert target
        else
            let lambda = fun arg ->
                               let argType,returnType = FSharpType.GetFunctionElements resultType

                               let argArray =
                                    match argType with
                                    | a when FSharpType.IsTuple(a) -> FSharpValue.GetTupleFields(arg)
                                    | a when a = typeof<unit>      -> [| |]
                                    | ____________________________ -> [|arg|]

                               let invoker k = Invocation(k, memberName 
                                                              |> Option.bind(fun name -> Some(InvokeMemberName(name,null)))
                                                              |> Option.toObj
                                                         ).Invoke(target, argArray)

                               let (|Action|Func|) t = if t = typeof<unit> then Action else Func
                               let (|Invoke|InvokeMember|) n = if n |> Option.isNone then Invoke else InvokeMember

                               let result =
                                    try //Either it has a member or it's something directly callable
                                        match (returnType, memberName) with
                                        | (Action,Invoke) -> invoker(InvocationKind.InvokeAction)
                                        | (Action,InvokeMember) -> invoker(InvocationKind.InvokeMemberAction)
                                        | (Func, Invoke) -> invoker(InvocationKind.Invoke)
                                        | (Func, InvokeMember) -> invoker(InvocationKind.InvokeMember)
                                    with  //Last chance incase we are trying to invoke an fsharpfunc
                                        |  :? RuntimeBinderException as e  ->
                                            try
                                                let invokeName =InvokeMemberName("Invoke", null) //FSharpFunc Invoke
                                                let invokeContext t = InvokeContext(t,typeof<obj>) //Improve cache hits by using the same context
                                                let invokeFSharpFold t (a:obj) =
                                                    Dynamic.InvokeMember(invokeContext(t),invokeName,a)
                                                let seed = match memberName with
                                                           | Some(name) -> Dynamic.InvokeGet(target,name)
                                                           | None       -> target
                                                Array.fold invokeFSharpFold seed argArray
                                            with
                                                | :? RuntimeBinderException as e2
                                                     -> AggregateException(e,e2) |> raise
                                                

                               match returnType with
                               | Action | NoConversion -> result
                               | _____________________ -> implicitConvertTo returnType result

            FSharpValue.MakeFunction(resultType,lambda) |> unbox<'TResult>