# Dynamitey and BridgeSupport

This library is an F# façade over the DLR. The C# call-site plumbing it needs is vendored from [Dynamitey](https://github.com/ekonbenefits/dynamitey) 3.0.3 (Apache-2.0) into a second package, **FSharp.Interop.Dynamic.BridgeSupport**, which `FSharp.Interop.Dynamic` depends on. The `Dynamitey` package itself is not a dependency any more.

## What each project is

| | Dynamitey | FSharp.Interop.Dynamic.BridgeSupport | FSharp.Interop.Dynamic |
| --- | --- | --- | --- |
| Language | C# | C# | F# |
| Package | `Dynamitey` 3.0.3 on nuget.org | `FSharp.Interop.Dynamic.BridgeSupport` | `FSharp.Interop.Dynamic` |
| Continuation | [dynamitey-community/dynamitey](https://github.com/dynamitey-community/dynamitey) | this repo | this repo, `fsprojects` |
| You write | `Dynamic.InvokeGet(o, "Name")` | nothing; the F# package calls it | `o?Name` / `o |> Dyn.get "Name"` |

BridgeSupport keeps only what the operators call: `InvokeGet`, `InvokeSet`, `InvokeMember`, `InvokeGetIndex`, `InvokeSetValueOnIndexes`, `InvokeGetChain`, `InvokeSetChain`, `InvokeConvert`, `InvokeBinaryOperator`, `InvokeAddAssignMember`, `InvokeSubtractAssignMember`, plus `InvokeContext`, `InvokeArg`, `InvokeMemberName` and `Invocation`. `DynamicObjects`, `Build`, `Curry`, constructors, unary operators and the ImpromptuInterface hooks are not included. The full list of files and modifications is in the package's `THIRD-PARTY-NOTICES.txt`.

## Types that show up in the F# API

`Dyn.staticContext` and `Dyn.staticTarget` return `FSharp.Interop.Dynamic.BridgeSupport.InvokeContext`; `Dyn.namedArg` returns `FSharp.Interop.Dynamic.BridgeSupport.InvokeArg`. Those two types are the supported surface of BridgeSupport. `Dynamic`, `Invocation`, `InvocationKind` and `InvokeMemberName` are public only because F# has to call them; they are hidden from IntelliSense and may change.

## Using Dynamitey alongside

You can still reference the `Dynamitey` package and `open Dynamitey` for `Build`, `Dynamic.Curry`, `DynamicObjects.Dictionary`, `Return<_>`. The namespaces are different, so nothing clashes.

The rule is: `Dyn` helpers (`Dyn.namedArg`, `Dyn.staticTarget`, `Dyn.staticContext`) go with `Dyn` functions and the `?` / `!?` operators; Dynamitey's `InvokeArg` / `StaticContext` go with Dynamitey's `Dynamic.*` calls. Each binder only unwraps its own types, so mixing them fails with a `RuntimeBinderException` rather than silently. Targets are fine to share: `!?Build<_>.NewObject(Dyn.namedArg "One" 1)` invokes a Dynamitey object through our operator, and the names travel as ordinary DLR named arguments.

## More than 14 arguments

Dynamitey built the delegate type for a call site with more than 14 arguments through ImpromptuInterface, and threw `TypeLoadException` when that package was absent, which it always was here. BridgeSupport builds it with `Expression.GetDelegateType`, so any arity works on runtimes that can generate code. NativeAOT is not supported by the DLR to begin with.

## Refreshing the subset

The vendored files each name their upstream path and the commit they came from. To pick up a fix from upstream, fetch the file at the new tag, re-apply the trims listed in `THIRD-PARTY-NOTICES.txt`, and update the header and notices. Moving to the community continuation is [#29](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/29).
