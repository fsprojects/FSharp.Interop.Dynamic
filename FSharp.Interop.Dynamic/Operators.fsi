namespace FSharp.Interop.Dynamic
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

