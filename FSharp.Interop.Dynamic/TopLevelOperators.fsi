namespace FSharp.Interop.Dynamic
  /// <summary>Main operators for dynamic invocation. Auto-opened with <c>open FSharp.Interop.Dynamic</c>.</summary>
  [<AutoOpen>]
  module TopLevelOperators = begin
    /// <summary>
    /// Dynamic get or named-member invoke, depending on the inferred result type.
    /// When the result type is not a function, this is a DLR get (<c>InvokeGet</c>): <c>target?Length</c>.
    /// When the result type is a function, this returns a callable; applying it does <c>InvokeMember</c>.
    /// That is why <c>target?Foo(1, 2)</c> is a method call: application forces a function type.
    /// A void CLR method is an <c>unit</c>-returning function.
    /// </summary>
    /// <param name="target">Object to read or invoke on.</param>
    /// <param name="name">Member name.</param>
    /// <returns>
    /// The converted member value, or a callable if the inferred result type is a function.
    /// </returns>
    val ( ? ) : target:obj -> name:string -> 'TResult
    /// <summary>
    /// Dynamic set of a named member (<c>InvokeSet</c>). Always <c>unit</c>; not a get.
    /// </summary>
    /// <param name="target">Object to write to.</param>
    /// <param name="name">Member name.</param>
    /// <param name="value">Value to assign.</param>
    val ( ?<- ) : target:obj -> name:string -> value:'TValue -> unit
    /// <summary>
    /// Invoke the target itself (<c>Dyn.invocation target Direct</c>).
    /// Use on a boxed F# function, a Dynamitey curry, or anything the DLR can invoke.
    /// </summary>
    /// <param name="target">Callable object to invoke.</param>
    /// <returns>
    /// The converted invoke result, or a callable if the inferred result type is a function.
    /// </returns>
    val ( !? ) : target:obj -> 'TResult
  end
