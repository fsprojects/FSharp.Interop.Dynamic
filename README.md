# FSharp.Interop.Dynamic

[![NuGet](https://img.shields.io/nuget/v/FSharp.Interop.Dynamic.svg?style=flat)](https://www.nuget.org/packages/FSharp.Interop.Dynamic/)
[![CI](https://github.com/fsprojects/FSharp.Interop.Dynamic/actions/workflows/dotnet.yml/badge.svg)](https://github.com/fsprojects/FSharp.Interop.Dynamic/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](License.txt)

F# operators for the Dynamic Language Runtime. `target?Name`, `target?Name <- value`, and `!?target` are the F# spelling of C# `dynamic`, with piping and `'T option` lookups.

**Docs:** [fsprojects.github.io/FSharp.Interop.Dynamic](https://fsprojects.github.io/FSharp.Interop.Dynamic/)

This library sits on [Dynamitey](https://www.nuget.org/packages/Dynamitey/) 3.0.3. It is the F# surface, not Dynamitey itself.

F# has no `dynamic` keyword. Use this when the member is not known at compile time: Expando/JSON bags, optional fields, method names from config, C# APIs that return `dynamic`, pythonnet, COM. Use ordinary F# when the type is in your project. Full argument: [Why this library](https://fsprojects.github.io/FSharp.Interop.Dynamic/docs/why.html).

---

## Version story

> **nuget.org still serves 5.0.1.268** (2022: `net45`, `netstandard1.6`, `netstandard2.0`).
> `master` is **6.0.0**: `netstandard2.0` + `net10.0`, `Dyn.tryGet` / `Dyn.exists`, GitHub Actions CI and publish. It reaches nuget.org when someone pushes the tag `v6.0.0`. Until then `dotnet add package FSharp.Interop.Dynamic` installs 5.0.1.268.

6.0.0 is a TFM break. `netstandard2.0` still covers current .NET Framework and .NET.

---

## Install (6.0.0)

```bash
dotnet add package FSharp.Interop.Dynamic
```

```fsharp
open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators   // optional: ?+?, ?=?, …
```

---

## Quick start

```fsharp
open System.Dynamic
open FSharp.Interop.Dynamic

let o = ExpandoObject()
o?Name <- "Ada"
let name: string = o?Name

let hello: string = "HelloWorld"?Substring(0, 5)
```

Annotate the result type when F# cannot infer it. Void CLR methods need `unit`:

```fsharp
let items = ResizeArray<string>()
let _: unit = items?Add("x")
```

### Check a member without throwing

```fsharp
let present: string option = o |> Dyn.tryGet "Name"     // Some "Ada"
let missing: string option = o |> Dyn.tryGet "NoSuch"   // None
o |> Dyn.exists "Name"    // true
o |> Dyn.exists "NoSuch"  // false
```

Lookup is Dynamitey `InvokeGet`. A present null is still present. A present value that cannot convert to `'T` still throws — that is not a miss.

### Pipe through `Dyn`

```fsharp
o |> Dyn.set "Name" "Ada"
let name: string = o |> Dyn.get "Name"
let hello: string = "HelloWorld" |> Dyn.invokeMember "Substring" (0, 5)
```

Target is last so piping works.

### Binary operators

```fsharp
open FSharp.Interop.Dynamic.Operators

let n: int = 5 ?+? 4
let f: float = 5 ?+? 3.5
let s: string = "Hello" ?+? " World"
[1; 2; 3; 4] |> List.reduce (?+?)
```

### Direct invoke

```fsharp
let add3: int -> int = !?(+) 3
add3 4  // 7
```

---

## What `?` does

| You write | DLR |
| --- | --- |
| `target?Name` when `'T` is not a function | `InvokeGet` |
| `target?Name` when `'T` is a function, then apply | `InvokeMember` |
| `target?Name <- value` | `InvokeSet` |
| `!?target` | `Invoke` on the target itself |

That is why `target?Foo(1, 2)` is a method call: application forces a function type.

More: [Operators](https://fsprojects.github.io/FSharp.Interop.Dynamic/docs/operators.html), [Dyn](https://fsprojects.github.io/FSharp.Interop.Dynamic/docs/dyn.html), [tryGet](https://fsprojects.github.io/FSharp.Interop.Dynamic/docs/tryget.html).

---

## Dynamitey

6.0.0 references **Dynamitey 3.0.3**. `tryGet` / `exists` do not wait on Dynamitey 4.0.0. The community continuation of Dynamitey is [dynamitey-community/dynamitey](https://github.com/dynamitey-community/dynamitey); switching this package to it is [#29](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/29).

You can still `open Dynamitey` for `Build`, `Dynamic.Curry`, `DynamicObjects.Dictionary`.

---

## Caveats

- The DLR cannot see **explicit interface members** (same as C# `dynamic`).
- **Not trim-safe or NativeAOT-safe.**
- Do not build member names from untrusted input. [SECURITY.md](SECURITY.md).
- Historical: .NET Core 2.0.0–2.0.2 broke `dynamic` on nested types inside generics ([#11](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/11)). Current TFMs are fine.

Full list: [Caveats](https://fsprojects.github.io/FSharp.Interop.Dynamic/docs/caveats.html).

---

Every F# snippet above is a test in `Tests/ReadmeExamples.fs`. pythonnet and SignalR sketches live on the [why](https://fsprojects.github.io/FSharp.Interop.Dynamic/docs/why.html) page and are not compiled here.

---

## Build

Requires the .NET 10 SDK.

```bash
dotnet restore
dotnet build -c Release -warnaserror
dotnet test --project Tests/Tests.fsproj -c Release
```

Docs: `dotnet tool install -g docfx --version 2.78.5`, then `dotnet build FSharp.Interop.Dynamic/FSharp.Interop.Dynamic.fsproj -c Release && docfx docfx/docfx.json`.

Release: add the version's section to [CHANGELOG.md](CHANGELOG.md), then tag `v6.0.0` and push. [Releasing](https://fsprojects.github.io/FSharp.Interop.Dynamic/docs/releasing.html).

---

## Feedback and contributing

- **Bugs and feature requests:** [open an issue](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/new). Include the F# and .NET versions and a minimal snippet.
- **Security problems:** do not open a public issue. Follow [SECURITY.md](SECURITY.md).
- **Code and docs:** pull requests are welcome. [CONTRIBUTING.md](CONTRIBUTING.md) covers the process and what a PR needs before it merges.

---

## Maintainers

- [@AtwoodTM](https://github.com/AtwoodTM)
- [@jbtule](https://github.com/jbtule)
- [@forki](https://github.com/forki)

fsprojects default: [@fsprojectsgit](https://github.com/fsprojectsgit).
