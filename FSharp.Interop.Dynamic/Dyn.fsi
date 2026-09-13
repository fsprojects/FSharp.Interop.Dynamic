namespace FSharp.Interop.Dynamic
  /// Names the dynamic member to get or invoke, or Direct to invoke the target itself.
  type Calling =
    /// Named member with generic type arguments.
    | GenericMember of string * System.Type array
    /// Named member (property, field, or method).
    | Member of string
    /// Invoke the target itself, not a named member.
    | Direct
   
  /// <summary>
  /// Function surface behind the operators. Use when you want piping, a name as data, or an operation <c>?</c> does not spell. Target is last on most members so <c>target |> Dyn.get "Name"</c> works; <c>invocation</c> is the exception.
  /// </summary>
  module Dyn = begin
    /// <summary>Static invoke context for dynamic calls to static members of <paramref name="target"/>.</summary>
    val staticContext : target:System.Type -> Dynamitey.InvokeContext
    /// Static invoke context for dynamic calls to static members of the type argument.
    val staticTarget<'TTarget> : Dynamitey.InvokeContext
    /// <summary>Implicit conversion of <paramref name="target"/> to <paramref name="convertType"/>.</summary>
    val implicitConvertTo : convertType:System.Type -> target:obj -> 'TResult
    /// <summary>Implicit conversion of <paramref name="target"/> to the inferred result type.</summary>
    val implicitConvert : target:obj -> 'TResult

    /// <summary>Explicit conversion of <paramref name="target"/> to <paramref name="convertType"/>.</summary>
    val explicitConvertTo : convertType:System.Type -> target:obj -> 'TResult
    /// <summary>Explicit conversion of <paramref name="target"/> to the inferred result type.</summary>
    val explicitConvert : target:obj -> 'TResult
    /// Marks an argument with a name for DLR named-argument invoke.
    val namedArg : name:string -> argValue:obj -> Dynamitey.InvokeArg
    /// <summary>Dynamically call <c>+=</c> on the named member (DLR add-assign, including events).</summary>
    val memberAddAssign : memberName:string -> value:obj -> target:obj -> unit
    /// <summary>Dynamically call <c>-=</c> on the named member (DLR subtract-assign, including events).</summary>
    val memberSubtractAssign :
      memberName:string -> value:obj -> target:obj -> unit
    /// <summary>
    /// Workhorse for dynamic get and invoke. Target is first because a function-typed result is a callable, not a get.
    /// </summary>
    /// <param name="target">Object, boxed function, or static context to read or invoke.</param>
    /// <param name="memberName">
    /// <c>Member</c> or <c>GenericMember</c> to get or invoke that member;
    /// <c>Direct</c> to invoke <paramref name="target"/> itself.
    /// </param>
    /// <returns>
    /// If the inferred result type is not a function, a DLR get converted to that type.
    /// If the inferred result type is a function, a callable: applying it does a DLR invoke
    /// (<c>InvokeMember</c> for a named member, <c>Invoke</c> for <c>Direct</c>).
    /// A void CLR method is an <c>unit</c>-returning function.
    /// </returns>
    val invocation : target:obj -> memberName:Calling -> 'TResult
    /// <summary>Invoke <paramref name="target"/> itself with <paramref name="value"/>. If the inferred result type is a function, returns a callable rather than applying immediately.</summary>
    val invokeDirect : value:'a -> target:obj -> 'TResult
    /// <summary>Invoke the named member of <paramref name="target"/> with <paramref name="value"/>. If the inferred result type is a function, returns a callable rather than applying immediately.</summary>
    val invokeMember : memberName:string -> value:'a -> target:obj -> 'TResult
    /// <summary>Invoke the named generic member of <paramref name="target"/> with <paramref name="typeArgs"/> and <paramref name="value"/>. If the inferred result type is a function, returns a callable rather than applying immediately.</summary>
    val invokeGeneric :
      memberName:string ->
        typeArgs:seq<System.Type> -> value:'a -> target:obj -> 'TResult
    /// <summary>
    /// Gets a named property or field (<c>InvokeGet</c>).
    /// If the inferred result type is a function, returns a callable rather than applying immediately.
    /// </summary>
    /// <param name="propertyName">Member name to read.</param>
    /// <param name="target">Object to read from.</param>
    /// <returns>The converted member value, or a callable if the inferred result type is a function.</returns>
    val get : propertyName:string -> target:obj -> 'TResult
    /// <summary>
    /// Gets a named member, or <c>None</c> when <c>InvokeGet</c> cannot find it.
    /// Conversion failures still throw; they are not treated as missing.
    /// </summary>
    /// <param name="propertyName">Member name to look up.</param>
    /// <param name="target">Object to read from.</param>
    /// <returns>
    /// <c>None</c> if the binder cannot find the member.
    /// <c>Some</c> value if the member is present and converts to <c>'T</c>
    /// (a present null is <c>Some null</c> for a reference <c>'T</c>).
    /// Throws if the member is present but cannot convert to <c>'T</c>.
    /// When <c>'T</c> is a function type and the member is missing, returns <c>None</c>
    /// immediately rather than a lazy callable.
    /// </returns>
    val tryGet : propertyName:string -> target:obj -> 'T option
    /// <summary>
    /// <c>true</c> when <c>InvokeGet</c> succeeds. A present null is still present.
    /// </summary>
    /// <param name="propertyName">Member name to look up.</param>
    /// <param name="target">Object to read from.</param>
    /// <returns>
    /// <c>true</c> if <c>InvokeGet</c> succeeds, including when the value is null.
    /// <c>false</c> if the binder cannot find the member.
    /// Does not convert the value, so this can be <c>true</c> while <c>tryGet</c> as a given type throws.
    /// A getter that throws something other than a binder miss is not reported as missing.
    /// </returns>
    val exists : propertyName:string -> target:obj -> bool
    /// <summary>Gets a chain of members (for example <c>A.B</c> from names <c>A</c> then <c>B</c>).</summary>
    val getChain : chainOfMembers:seq<string> -> target:obj -> 'TResult
    /// Dynamically get an indexer value.
    val getIndexer : indexers:seq<'T> -> target:obj -> 'TResult
    /// <summary>Sets a named property or field (<c>InvokeSet</c>).</summary>
    val set : propertyName:string -> value:obj -> target:obj -> unit
    /// <summary>Sets a chain of members (for example <c>A.B</c> from names <c>A</c> then <c>B</c>).</summary>
    val setChain : chainOfMembers:seq<string> -> value:obj -> target:obj -> unit
    /// Dynamically set an indexer value.
    val setIndexer : indexers:seq<'T> -> value:obj -> target:obj -> unit
    /// <summary>Obsolete. Use <c>getIndexer</c> (target last) instead.</summary>
    [<System.Obsolete ("Replaced with partial application version `getIndexer`")>]
    val getIndex : target:obj -> indexers:seq<'T> -> 'TResult
    /// <summary>Obsolete. Use <c>setIndexer</c> (target last) instead.</summary>
    [<System.Obsolete ("Replaced with partial application version `setIndexer`")>]
    val setIndex : target:obj -> indexers:seq<'T> -> value:obj -> unit
    /// <summary>Obsolete. Use <c>memberAddAssign</c> (target last) instead.</summary>
    [<System.Obsolete ("Replaced with partial application version `memberAddAssign`")>]
    val addAssignMember : target:obj -> memberName:string -> value:obj -> unit
    /// <summary>Obsolete. Use <c>memberSubtractAssign</c> (target last) instead.</summary>
    [<System.Obsolete ("Replaced with partial application version `memberSubtractAssign`")>]
    val subtractAssignMember :
      target:obj -> memberName:string -> value:obj -> unit
    /// <summary>Obsolete. Use <c>invocation</c> with <c>Member</c> or <c>Direct</c> instead of <c>Some</c> name or <c>None</c>.</summary>
    [<System.Obsolete("Replaced with `invocation`")>]
    val invoke : target:obj -> memberName:string option -> 'TResult
  end
