namespace FSharp.Interop.Dynamic

module Operators=
    open System
    open Dynamitey
    open System.Linq.Expressions
    let (?%?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.Modulo, right)) :?> 'TResult
    
    let (?*?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.Multiply, right)) :?> 'TResult

    let (?+?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.Add, right)) :?> 'TResult
    
    let (?-?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.Subtract, right)) :?> 'TResult

    let (?/?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.Divide, right)) :?> 'TResult

    let (?&&&?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.And, right)) :?> 'TResult

    let (?|||?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.Or, right)) :?> 'TResult

    let (?^^^?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.ExclusiveOr, right)) :?> 'TResult

    let (?<<<?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.LeftShift, right)) :?> 'TResult

    let (?>>>?) (left:obj) (right: obj) : 'TResult =    
        (Dynamic.InvokeBinaryOperator(left, ExpressionType.RightShift, right)) :?> 'TResult

    let (?<=?) (left:obj) (right: obj) : bool =    
        Dynamic.InvokeBinaryOperator(left, ExpressionType.LessThanOrEqual, right) |> Dyn.explicitConvert

    let (?<>?) (left:obj) (right: obj) : bool =    
        Dynamic.InvokeBinaryOperator(left, ExpressionType.NotEqual, right) |> Dyn.explicitConvert

    let (?<?) (left:obj) (right: obj) : bool =    
        Dynamic.InvokeBinaryOperator(left, ExpressionType.LessThan, right) |> Dyn.explicitConvert

    let (?=?) (left:obj) (right: obj) : bool =    
        Dynamic.InvokeBinaryOperator(left, ExpressionType.Equal, right) |> Dyn.explicitConvert

    let (?>?) (left:obj) (right: obj) : bool =    
        Dynamic.InvokeBinaryOperator(left, ExpressionType.GreaterThan, right) |> Dyn.explicitConvert

    let (?>=?) (left:obj) (right: obj) : bool =    
        Dynamic.InvokeBinaryOperator(left, ExpressionType.GreaterThanOrEqual, right) |> Dyn.explicitConvert

  