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


namespace EkonBenefits.FSharp

module Util=
    open Dynamitey
    open System.Dynamic
     
    ///Wrap object to call the c# equivalent += dynamically when using f# dynamic set operator
    type PropertySetCallsAddAssign(target:obj)=
      inherit DynamicObject()
        
      override this.TrySetMember(binder:SetMemberBinder, value:obj) =
        Dynamic.InvokeAddAssignMember(target,binder.Name,value)
        true

    ///Wrap object to call the c# equivalent -= dynamically when using f# dynamic set operator
    type PropertySetCallsSubtractAssign(target:obj)=
      inherit DynamicObject()

      override this.TrySetMember(binder:SetMemberBinder, value:obj) =
        Dynamic.InvokeSubtractAssignMember(target,binder.Name,value)
        true

    ///Wrap object to use get operator to attach argument name for dynamic invocation
    type PropertyGetCallsNamedArgument(target:obj)=
      inherit DynamicObject()

      override this.TryGetMember(binder:GetMemberBinder,  result: obj byref) =
        result <- InvokeArg(binder.Name,target) 
        true
