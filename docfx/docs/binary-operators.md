# Binary operators

`open FSharp.Interop.Dynamic.Operators` (not auto-opened).

Each operator is a DLR binary op plus an inferred conversion of the result.

| Operator | DLR |
| --- | --- |
| `?+?` | add (also string concat) |
| `?-?` | subtract |
| `?*?` | multiply |
| `?/?` | divide |
| `?%?` | modulo |
| `?&&&?` `?\|\|\|?` `?^^^?` | bitwise and/or/xor |
| `?<<<?` `?>>>?` | shifts |
| `?=?` `?<>?` `?<?` `?<=?` `?>?` `?>=?` | comparisons; result is `bool` |

```fsharp
open FSharp.Interop.Dynamic.Operators

let n: int = 5 ?+? 4                 // 9
let f: float = 5 ?+? 3.5             // 8.5 — mixed numeric types
let s: string = "Hello" ?+? " World" // concatenation
let ok = 3 ?>? 2                     // true

[1; 2; 3; 4] |> List.reduce (?+?)    // 10 — operators as values
```

Divide by zero is a real `DivideByZeroException`, not a binder miss.

The comparison operators convert the DLR result with `Dyn.explicitConvert` to `bool`.
