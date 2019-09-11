namespace FSharp.Interop.Dynamic
  /// Main operators for dynamic invocation
  [<AutoOpen>]
  module TopLevelOperators = begin
    /// Dynamic get property or method invocation
    val ( ? ) : target:obj -> name:string -> 'TResult
    /// Dynamic set property
    val ( ?<- ) : target:obj -> name:string -> value:'TValue -> unit
    /// Prefix operator that allows direct dynamic invocation of the object
    val ( !? ) : target:obj -> 'TResult
  end

