# Getting started

## Installing

> [!IMPORTANT]
> nuget.org's latest is **5.0.1.268**. That package still targets `net45` / `netstandard1.6` / `netstandard2.0`. `master` is 6.0.0 (`netstandard2.0` + `net10.0`) and is not on nuget.org until a `v6.0.0` tag. The snippets below are 6.0.0.

```bash
dotnet add package FSharp.Interop.Dynamic
```

```xml
<PackageReference Include="FSharp.Interop.Dynamic" Version="6.0.0" />
```

```fsharp
open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators   // optional: ?+?, ?=?, …
```

`open FSharp.Interop.Dynamic` brings in the auto-opened `?`, `?<-`, and `!?` operators. The `Dyn` module is always there; you do not need a second open for it.

## The three operators you actually use

| Operator | Meaning |
| --- | --- |
| `target?Name` | Get a property, or call a method (the result can be a function you then apply) |
| `target?Name <- value` | Set a property |
| `!?target` | Invoke `target` itself (an F# function boxed as `obj`, a delegate, a Dynamitey curry) |

```fsharp
open System.Dynamic

let o = ExpandoObject()
o?Name <- "Ada"
let name: string = o?Name

let parts: string[] = "a,b,c"?Split(',')
```

Annotate the result type when F# cannot infer it. The DLR conversion uses that type.

## Why not just use C# `dynamic` from F#?

You can, with a C# helper, and for a member whose name is a literal in C# that is often simpler. Reach for this library when:

- **The call is in F#.** There is no `dynamic` keyword. `?` is the F# spelling.
- **The name is data.** `o?Name` still writes `Name` in source; `Dyn.get name o` takes a string.
- **You want F# piping.** `target |> Dyn.get "Name"`, `target |> Dyn.tryGet "Name"`.
- **You want an option, not an exception.** `Dyn.tryGet` / `Dyn.exists` — see [tryGet and exists](tryget.md). C# `dynamic` has no equivalent.

## Infer the result, or you get `obj`

```fsharp
let n: int = "hello"?Length          // 5
let boxed = "hello"?Length           // obj, because nothing asked for int
```

Methods that return `unit` (void in CLR) need that annotation too, or the binder asks for a value from a void member and throws:

```fsharp
let items = ResizeArray<string>()
let _: unit = items?Add("x")
```

## Where to go next

- [Why this library](why.md) — untyped payloads, optional fields, names as data, C# `dynamic`
- [Operators](operators.md) — `?`, `?<-`, `!?` in detail
- [Dyn](dyn.md) — the functions behind those operators
- [tryGet and exists](tryget.md) — the issue #27 surface
- [Caveats](caveats.md) — explicit interface members, AOT, conversion vs missing
