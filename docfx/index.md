---
_layout: landing
title: FSharp.Interop.Dynamic
---

# FSharp.Interop.Dynamic

F# operators and helpers for the Dynamic Language Runtime. `target?Name`, `target?Name<-value`, and `!?target` do what C#'s `dynamic` keyword does, with F# inference and piping.

This is the fsprojects library that sits on [Dynamitey](https://www.nuget.org/packages/Dynamitey/) 3.0.3. It is not Dynamitey itself; it is the F# surface.

> [!IMPORTANT]
> nuget.org still serves **5.0.1.268** (the 2022 package). `master` is **6.0.0**: `netstandard2.0` and `net10.0`, GitHub Actions CI, `Dyn.tryGet` / `Dyn.exists`. It publishes when someone tags `v6.0.0`. Until then `dotnet add package FSharp.Interop.Dynamic` still gets 5.0.1.268.

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
| How this relates to Dynamitey 3.0.3 / 4.0.0 | [Dynamitey](docs/dynamitey.md) |
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

`netstandard2.0` and `net10.0`. The `netstandard2.0` target is kept deliberately — it is the only target reaching both .NET Framework 4.6.2+ and modern .NET from a single package. 6.0.0 dropped `net45` and `netstandard1.6`.

## A word on trimming and AOT

This library is DLR-based and will never be trim-safe or NativeAOT-safe. Members it resolves at runtime can disappear under the trimmer. See [Caveats](docs/caveats.md).
