namespace FSharp.Interop.Dynamic
  type Calling =
    | GenericMember of string * System.Type array
    | Member of string
    | Direct
   
  ///Functions backing the operators and more
  module Dyn = begin
    ///allow access to static context for dynamic invocation of static methods
    val staticContext : target:System.Type -> Dynamitey.InvokeContext
    val staticTarget<'TTarget> : Dynamitey.InvokeContext
    ///implict convert via reflected type
    val implicitConvertTo : convertType:System.Type -> target:obj -> 'TResult
    ///implict convert via inferred type
    val implicitConvert : target:obj -> 'TResult

    ///explicit convert via reflected type
    val explicitConvertTo : convertType:System.Type -> target:obj -> 'TResult
    ///explicit convert via inferred type
    val explicitConvert : target:obj -> 'TResult
    ///allow marking args with names for dlr invoke
    val namedArg : name:string -> argValue:obj -> Dynamitey.InvokeArg
    ///Dynamically call `+=` on member
    val memberAddAssign : memberName:string -> value:obj -> target:obj -> unit
    ///Dynamically call `-=` on member
    val memberSubtractAssign :
      memberName:string -> value:obj -> target:obj -> unit
    /// main workhouse method; Some(methodName) or just None to invoke without name;
    /// infered casting with automatic implicit convert.
    /// target not last because result could be infered to be fsharp style curried function
    val invocation : target:obj -> memberName:Calling -> 'TResult
    ///allows result to be called like a function
    val invokeDirect : value:'a -> target:obj -> 'TResult
    ///calls member whose result can be called like a function
    val invokeMember : memberName:string -> value:'a -> target:obj -> 'TResult
    ///calls member and specify's generic parameters and whose result can be called like a function
    val invokeGeneric :
      memberName:string ->
        typeArgs:seq<System.Type> -> value:'a -> target:obj -> 'TResult
    val get : propertyName:string -> target:obj -> 'TResult
    val getChain : chainOfMembers:seq<string> -> target:obj -> 'TResult
    ///dynamically call get index
    val getIndexer : indexers:seq<'T> -> target:obj -> 'TResult
    val set : propertyName:string -> value:obj -> target:obj -> unit
    val setChain : chainOfMembers:seq<string> -> value:obj -> target:obj -> unit
    val setIndexer : indexers:seq<'T> -> value:obj -> target:obj -> unit
    /// *OBSOLETE*
    [<System.Obsolete ("Replaced with partial application version `getIndexer`")>]
    val getIndex : target:obj -> indexers:seq<'T> -> 'TResult
    /// *OBSOLETE*
    [<System.Obsolete ("Replaced with partial application version `setIndexer`")>]
    val setIndex : target:obj -> indexers:seq<'T> -> value:obj -> unit
    /// *OBSOLETE*
    [<System.Obsolete
      ("Replaced with partial application version `memberAddAssign`")>]
    val addAssignMember : target:obj -> memberName:string -> value:obj -> unit
    /// *OBSOLETE*
    [<System.Obsolete ()>]
    val subtractAssignMember :
      target:obj -> memberName:string -> value:obj -> unit
    /// *OBSOLETE*
    [<System.Obsolete("Replaced with `invocation`")>]
    val invoke : target:obj -> memberName:string option -> 'TResult
  end

