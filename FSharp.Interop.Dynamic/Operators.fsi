namespace FSharp.Interop.Dynamic
  /// Additional operators to dyanmically do normal operations
  module Operators = begin
    val ( ?%? ) : left:obj -> right:obj -> 'TResult
    val ( ?*? ) : left:obj -> right:obj -> 'TResult
    val ( ?+? ) : left:obj -> right:obj -> 'TResult
    val ( ?-? ) : left:obj -> right:obj -> 'TResult
    val ( ?/? ) : left:obj -> right:obj -> 'TResult
    val ( ?&&&? ) : left:obj -> right:obj -> 'TResult
    val ( ?|||? ) : left:obj -> right:obj -> 'TResult
    val ( ?^^^? ) : left:obj -> right:obj -> 'TResult
    val ( ?<<<? ) : left:obj -> right:obj -> 'TResult
    val ( ?>>>? ) : left:obj -> right:obj -> 'TResult
    val ( ?<=? ) : left:obj -> right:obj -> bool
    val ( ?<>? ) : left:obj -> right:obj -> bool
    val ( ?<? ) : left:obj -> right:obj -> bool
    val ( ?=? ) : left:obj -> right:obj -> bool
    val ( ?>? ) : left:obj -> right:obj -> bool
    val ( ?>=? ) : left:obj -> right:obj -> bool
  end

