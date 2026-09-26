# Dynamitey

This library is an F# façade over Dynamitey. 7.0.0 references [**Dynamitey.Community 4.0.0**](https://www.nuget.org/packages/Dynamitey.Community/4.0.0). 6.0.0 referenced **Dynamitey 3.0.3**, the last release from [ekonbenefits/dynamitey](https://github.com/ekonbenefits/dynamitey), published 8 November 2023.

## What each project is

| | Dynamitey | FSharp.Interop.Dynamic |
| --- | --- | --- |
| Language | C# | F# |
| Package | [`Dynamitey.Community` 4.0.0](https://www.nuget.org/packages/Dynamitey.Community/4.0.0). The `Dynamitey` id is still 3.0.3 | `FSharp.Interop.Dynamic` 7.0.0 |
| Source | [dynamitey-community/dynamitey](https://github.com/dynamitey-community/dynamitey) | this repo, `fsprojects` |
| You write | `Dynamic.InvokeGet(o, "Name")` | `o?Name` / `o |> Dyn.get "Name"` |

The F# operators call `Dynamic.InvokeGet`, `InvokeSet`, `InvokeMember`, `InvokeConvert`, and friends. Named arguments are Dynamitey `InvokeArg`. Static calls use Dynamitey `InvokeContext`.

## 4.0.0

Jay Tuley wrote Dynamitey. The `Dynamitey` package id stopped at 3.0.3. The continuation publishes `Dynamitey.Community`, so both can be on nuget.org. The namespace is still `Dynamitey`, so `open Dynamitey` stays. The assembly name is `Dynamitey.Community`.

4.0.0 targets `netstandard2.0` and `net10.0`, the same pair as this library's 7.0.0. `net40` is dropped. 4.0.0 is the last Dynamitey.Community release that still targets `netstandard2.0`, which is .NET Framework 4.6.1 through 4.8.1. The release notes are [About 4.0.0](https://dynamitey-community.github.io/dynamitey/docs/v4.html).

`Dyn.namedArg`, `Dyn.staticContext`, and `Dyn.staticTarget` return `InvokeArg` and `InvokeContext` from that assembly. A project that also references `Dynamitey` 3.0.3 fails to compile with `CS0433`. ImpromptuInterface still references the original package. How to alias that is in the [community readme](https://github.com/dynamitey-community/dynamitey#if-you-end-up-with-both-packages).

[#95](https://github.com/dynamitey-community/dynamitey/issues/95) is Dynamitey.Community 5.0.0. It ships after .NET 11 is generally available (scheduled 10 November 2026) and targets `net10.0` and `net11.0`. The `netstandard2.0` leg is why that library still carries `#if NETSTANDARD2_0`, a `Guard.NotNull` helper instead of `ArgumentNullException.ThrowIfNull`, attribute polyfills, a C# 7.3 language default, and package references to `Microsoft.CSharp` 4.* and `System.Reflection.Emit` 4.7.0. Those packages are in the shared framework on .NET 10 and .NET 11. The Framework hash of a member name cannot call `string.GetHashCode(StringComparison.Ordinal)`, so it does not match the ordinal `Equals`. 5.0.0 deletes that split. It does not make a warm `InvokeMember` faster, and it does not make the library trim-safe.

7.0.0 of this library is the matching line: the last release with `netstandard2.0`. A Framework 4.6.1 through 4.8.1 application stays here. **8.0.0** ([#108](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/108)) takes Dynamitey.Community 5.0.0 and drops .NET Standard 2.0. [#29](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/29) was the older ticket. It is closed. The 4.0.0 switch is [#130](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/130).

## Do not take a compile-time dependency on Dynamitey internals

You can `open Dynamitey` for `Build`, `Dynamic.Curry`, `DynamicObjects.Dictionary`, `Return<_>`. That is public Dynamitey API. Anything under `Dynamitey.Internal` is DLR plumbing.
