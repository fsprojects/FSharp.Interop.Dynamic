---
_layout: landing
title: FSharp.Interop.Dynamic
---

# FSharp.Interop.Dynamic

F# operators and helpers for the Dynamic Language Runtime. `target?Name`, `target?Name<-value`, and `!?target` do what C#'s `dynamic` keyword does, with F# inference and piping.

This is the fsprojects library that sits on [Dynamitey.Community](https://www.nuget.org/packages/Dynamitey.Community/) 4.0.0. It is not Dynamitey itself; it is the F# surface.

> [!IMPORTANT]
> **7.0.0** is the current package, and the last one that supports .NET Standard 2.0 (.NET Framework 4.6.1 through 4.8.1) together with `net10.0`. It references Dynamitey.Community 4.0.0. **6.0.0** was the same target frameworks on Dynamitey 3.0.3. **5.0.1.268** was the last package that still targeted `net45` / `netstandard1.6`.

## Where to start

| If you want to | Read |
| --- | --- |
| Know when to reach for this | [Why this library](docs/why.md) |
| Install and make the first `?` call | [Getting started](docs/getting-started.md) |
| Get, set, and invoke through the operators | [Operators](docs/operators.md) |
| Pipe through `Dyn.get` / `Dyn.invokeMember` | [Dyn](docs/dyn.md) |
| Check a member without throwing | [tryGet and exists](docs/tryget.md) |
| Dynamic `+`, `*`, comparisons | [Binary operators](docs/binary-operators.md) |
| Turn a quotation into a member name | [Quotations](docs/quotations.md) |
| How this relates to Dynamitey.Community 4.0.0 | [Dynamitey](docs/dynamitey.md) |
| What the DLR will not do | [Caveats](docs/caveats.md) |
| Cut a NuGet release | [Releasing](docs/releasing.md) |
| Look up a specific type or member | [API reference](api/index.md) |

## The shortest example

```fsharp
open FSharp.Interop.Dynamic
open System.Dynamic

let o = ExpandoObject()
o?Name <- "Ada"
let name: string = o?Name
```

That is a DLR get/set. The compiler does not need to know `Name`. The same operators work on ViewBag, COM, pythonnet, SignalR clients, and ordinary CLR objects.

## What this library is for

F# has no `dynamic` keyword. This library is that keyword, plus piping and an option-returning lookup.

- **The call is in F#.** `?` / `?<-` / `!?` are the spelling.
- **The name is data.** `Dyn.get name` takes a string; `o?Name` still writes `Name` in source.
- **You want an option, not an exception.** [`Dyn.tryGet`](docs/tryget.md) / `Dyn.exists` catch binder misses. C# `dynamic` has no equivalent.
- **You want F# piping.** Target is last on `Dyn.*` so `o |> Dyn.get "Name"` works.

Reach for ordinary F# when the member is known at compile time. Reach for this library when it is not.

## Supported frameworks

7.0.0 targets `netstandard2.0` and `net10.0`. `netstandard2.0` is how a .NET Framework 4.6.1 through 4.8.1 application uses this package. It is also the last release that includes that target. 8.0.0 targets `net10.0` and `net11.0` only ([#108](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/108)), after [Dynamitey.Community 5.0.0](https://github.com/dynamitey-community/dynamitey/issues/95) drops `netstandard2.0`. Framework 4.8.1 stays serviced with Windows and does not gain BCL APIs, so a Framework application stays on 7.0.0.

## A word on trimming and AOT

This library is DLR-based and will never be trim-safe or NativeAOT-safe. Members it resolves at runtime can disappear under the trimmer. See [Caveats](docs/caveats.md).
