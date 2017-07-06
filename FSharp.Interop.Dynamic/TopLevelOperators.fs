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

[<AutoOpen>]
module TopLevelOperators=
    open System
    open Microsoft.CSharp.RuntimeBinder
    open Microsoft.FSharp.Reflection
    open Dynamitey

    ///Dynamic get property or method invocation
    let (?)  (target : obj) (name:string)  : 'TResult =
       Dyn.invoke target (Some(name))

    ///Dynamic set property
    let (?<-) (target : obj) (name : string) (value : 'TValue) : unit =
        Dynamic.InvokeSet(target, name, value) |> ignore

    ///Prefix operator that allows direct dynamic invocation of the object
    let (!?) (target:obj) : 'TResult =
        Dyn.invoke target None
