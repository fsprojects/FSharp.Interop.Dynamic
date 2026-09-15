# FSharp.Interop.Dynamic.BridgeSupport

The DLR call-site plumbing behind [FSharp.Interop.Dynamic](https://github.com/fsprojects/FSharp.Interop.Dynamic). It is a vendored, trimmed subset of [Dynamitey](https://github.com/ekonbenefits/dynamitey) 3.0.3 (Apache-2.0) containing only what the F# operators call. See `THIRD-PARTY-NOTICES.txt`.

You do not reference this package directly; `FSharp.Interop.Dynamic` depends on it.

## Supported surface

Two types show up in the F# API and are supported:

- `InvokeContext` — returned by `Dyn.staticContext` and `Dyn.staticTarget`, so `?` can call static members of a type. `StaticContext` is a convenience subclass; `Dyn` never returns it.
- `InvokeArg` — returned by `Dyn.namedArg`, so an argument can be passed by name.

Everything else (`Dynamic`, `Invocation`, `InvocationKind`, `InvokeMemberName`) is public because F# needs to call it, but it is hidden from IntelliSense and may change without notice.

## Not Dynamitey

These types live in the `FSharp.Interop.Dynamic.BridgeSupport` namespace, not `Dynamitey`. They cannot be passed to the Dynamitey package's own APIs (`Build<T>.NewObject`, `Dynamic.Curry`, …); construct `Dynamitey.InvokeArg` / `Dynamitey.StaticContext` for those. The two packages coexist in one process without conflict.
