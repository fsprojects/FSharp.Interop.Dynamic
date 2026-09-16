namespace FSharp.Interop.Dynamic
  /// <summary>
  /// Dynamic DLR binary operators. Arithmetic and bitwise operators unbox the DLR result (<c>:?&gt;</c>). Comparisons convert that result to <c>bool</c> with <c>Dyn.explicitConvert</c>. Not auto-opened; <c>open FSharp.Interop.Dynamic.Operators</c>.
  /// </summary>
  module Operators = begin
    /// <summary>Dynamically take modulo of <paramref name="left"/> by <paramref name="right"/>.</summary>
    val ( ?%? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically multiply <paramref name="left"/> and <paramref name="right"/>.</summary>
    val ( ?*? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically add <paramref name="left"/> and <paramref name="right"/> (also string concatenation).</summary>
    val ( ?+? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically subtract <paramref name="right"/> from <paramref name="left"/>.</summary>
    val ( ?-? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically divide <paramref name="left"/> by <paramref name="right"/>. Divide by zero is a real <c>DivideByZeroException</c>, not a binder miss.</summary>
    val ( ?/? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically bitwise-and <paramref name="left"/> and <paramref name="right"/>.</summary>
    val ( ?&&&? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically bitwise-or <paramref name="left"/> and <paramref name="right"/>.</summary>
    val ( ?|||? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically bitwise-xor <paramref name="left"/> and <paramref name="right"/>.</summary>
    val ( ?^^^? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically left-shift <paramref name="left"/> by <paramref name="right"/>.</summary>
    val ( ?<<<? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically right-shift <paramref name="left"/> by <paramref name="right"/>.</summary>
    val ( ?>>>? ) : left:obj -> right:obj -> 'TResult
    /// <summary>Dynamically compare whether <paramref name="left"/> is less than or equal to <paramref name="right"/>. Converts the DLR result to <c>bool</c>.</summary>
    val ( ?<=? ) : left:obj -> right:obj -> bool
    /// <summary>Dynamically compare whether <paramref name="left"/> is not equal to <paramref name="right"/>. Converts the DLR result to <c>bool</c>.</summary>
    val ( ?<>? ) : left:obj -> right:obj -> bool
    /// <summary>Dynamically compare whether <paramref name="left"/> is less than <paramref name="right"/>. Converts the DLR result to <c>bool</c>.</summary>
    val ( ?<? ) : left:obj -> right:obj -> bool
    /// <summary>Dynamically compare whether <paramref name="left"/> equals <paramref name="right"/>. Converts the DLR result to <c>bool</c>.</summary>
    val ( ?=? ) : left:obj -> right:obj -> bool
    /// <summary>Dynamically compare whether <paramref name="left"/> is greater than <paramref name="right"/>. Converts the DLR result to <c>bool</c>.</summary>
    val ( ?>? ) : left:obj -> right:obj -> bool
    /// <summary>Dynamically compare whether <paramref name="left"/> is greater than or equal to <paramref name="right"/>. Converts the DLR result to <c>bool</c>.</summary>
    val ( ?>=? ) : left:obj -> right:obj -> bool
  end
