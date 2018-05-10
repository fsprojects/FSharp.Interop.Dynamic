namespace FSharp.Interop.Dynamic
  type Calling =
    | GenericMember of string * System.Type array
    | Member of string
    | Direct
  module Dyn = begin
    val staticContext : target:System.Type -> Dynamitey.InvokeContext
    val staticTarget<'TTarget> : Dynamitey.InvokeContext
    val implicitConvertTo : convertType:System.Type -> target:obj -> 'TResult
    val implicitConvert : target:obj -> 'TResult
    val explicitConvertTo : convertType:System.Type -> target:obj -> 'TResult
    val explicitConvert : target:obj -> 'TResult
    val namedArg : name:string -> argValue:obj -> Dynamitey.InvokeArg
    val memberAddAssign : memberName:string -> value:obj -> target:obj -> unit
    val memberSubtractAssign :
      memberName:string -> value:obj -> target:obj -> unit
    val invocation : memberName:Calling -> target:obj -> 'TResult
    val invokeDirect : target:obj -> 'TResult
    val invokeMember : memberName:string -> target:obj -> 'TResult
    val invokeGeneric :
      memberName:string -> typeArgs:seq<System.Type> -> target:obj -> 'TResult
    val get : propertyName:string -> target:obj -> 'TResult
    val getChain : chainOfMembers:seq<string> -> target:obj -> 'TResult
    val getIndexer : indexers:seq<'T> -> target:obj -> 'TResult
    val set : propertyName:string -> value:obj -> target:obj -> unit
    val setChain : chainOfMembers:seq<string> -> value:obj -> target:obj -> unit
    val setIndexer : indexers:seq<'T> -> value:obj -> target:obj -> unit
    [<System.Obsolete ("Replaced with partial application version `getIndexer`")>]
    val getIndex : target:obj -> indexers:seq<'T> -> 'TResult
    [<System.Obsolete ("Replaced with partial application version `setIndexer`")>]
    val setIndex : target:obj -> indexers:seq<'T> -> value:obj -> unit
    [<System.Obsolete
      ("Replaced with partial application version `memberAddAssign`")>]
    val addAssignMember : target:obj -> memberName:string -> value:obj -> unit
    [<System.Obsolete ()>]
    val subtractAssignMember :
      target:obj -> memberName:string -> value:obj -> unit
    [<System.Obsolete ("Replaced with partial application version `invocation`")>]
    val invoke : target:obj -> memberName:string option -> 'TResult
  end

