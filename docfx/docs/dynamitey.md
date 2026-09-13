# Dynamitey

This library is an F# façade over [Dynamitey](https://www.nuget.org/packages/Dynamitey/). 6.0.0 references **Dynamitey 3.0.3** — the last upstream release. It does not need Dynamitey 4.0.0 to ship.

## What each project is

| | Dynamitey | FSharp.Interop.Dynamic |
| --- | --- | --- |
| Language | C# | F# |
| Package | `Dynamitey` 3.0.3 on nuget.org | `FSharp.Interop.Dynamic` |
| Continuation | [dynamitey-community/dynamitey](https://github.com/dynamitey-community/dynamitey) (4.0.0 not published yet) | this repo, `fsprojects` |
| You write | `Dynamic.InvokeGet(o, "Name")` | `o?Name` / `o |> Dyn.get "Name"` |

The F# operators call `Dynamic.InvokeGet`, `InvokeSet`, `InvokeMember`, `InvokeConvert`, and friends. Named arguments are Dynamitey `InvokeArg`. Static calls use Dynamitey `InvokeContext`.

## 4.0.0

The community continuation targets `netstandard2.0` + `net10.0` and is ready as Dynamitey 4.0.0. Until that package exists on nuget.org, this library stays on 3.0.3. Switching the package reference is [#29](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/29). `tryGet` / `exists` already work on 3.0.3 because they use `InvokeGet`.

## Do not take a compile-time dependency on Dynamitey internals

You can `open Dynamitey` for `Build`, `Dynamic.Curry`, `DynamicObjects.Dictionary`, `Return<_>`. That is public Dynamitey API. Anything under `Dynamitey.Internal` is DLR plumbing.
