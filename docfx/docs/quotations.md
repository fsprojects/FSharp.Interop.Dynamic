# Quotations

`FSharp.Interop.Dynamic.SymbolicString` turns an F# quotation into a member name (or a `LeafInfo` with name, return type, and declaring type). Use it when you want the compiler to check that `Length` exists rather than shipping a magic string.

```fsharp
open FSharp.Interop.Dynamic.SymbolicString

Symbol.nameOf(String.length)     // "length"
Symbol.nameOf(fun (x:string) -> x.ToString)  // "ToString"
Symbol.nameOf(Option.Some)       // "Some"
```

`Symbol.typeOf` is the same walk, returning the `Type`. `Symbol.leafInfoOf` returns both plus `DeclaringType`.

The argument is `[<ReflectedDefinition>]`, so you write an expression, not `Expr.Value`. The library walks:

- named values
- union cases
- property / field gets
- method calls
- `fun` and `let` wrappers around those

Anything else (`1`, `(1, 2)`, `if … then …`) throws `ArgumentException`.

`sym<'T>` exists only so you can quote a typed hole. **Do not call it.** Evaluating it throws.
