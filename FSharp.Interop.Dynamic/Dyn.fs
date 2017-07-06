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
    open System.Dynamic
    let staticContext (target:Type) = InvokeContext.CreateStatic.Invoke(target)


    let implicitConvertTo (convertType:Type) (target:obj) : 'TResult  = 
      Dynamic.InvokeConvert(target, convertType, explicit = false) :?> 'TResult

    let implicitConvert(target:obj) : 'TResult  = 
      implicitConvertTo typeof<'TResult> target

    let explicitConvertTo (convertType:Type) (target:obj) : 'TResult  = 
      Dynamic.InvokeConvert(target, convertType, explicit = true) :?> 'TResult

    let explicitConvert (target:obj) : 'TResult  = 
       explicitConvertTo typeof<'TResult> target

    let namedArg (name:string) (argValue:obj) =
        InvokeArg(name, argValue)

    let addAssignMember (target:obj) (memberName:string) (value:obj)  =
        Dynamic.InvokeAddAssignMember(target, memberName, value)

    let subtractAssignMember (target:obj) (memberName:string) (value:obj)  =
        Dynamic.InvokeSubtractAssignMember(target, memberName, value)