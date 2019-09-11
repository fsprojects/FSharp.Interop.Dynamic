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
    let (?)  (target : obj) (name:string)  : 'TResult =
       Dyn.invocation target (Member name)

    let (?<-) (target : obj) (name : string) (value : 'TValue) : unit =
        target |> Dyn.set name value

    let (!?) (target:obj) : 'TResult =
        Dyn.invocation target Direct
