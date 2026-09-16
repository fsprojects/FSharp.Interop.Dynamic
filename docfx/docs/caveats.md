# Caveats

## Explicit interface members

The DLR cannot see members implemented explicitly on an interface. Neither can `?`, `Dyn.get`, or C# `dynamic`. If you need that member, call it through the interface type in ordinary F#.

## Conversion vs missing

`RuntimeBinderException` means the binder could not do what you asked. That is **not** always "the member is missing":

- Missing property → binder exception → `tryGet` returns `None`
- Present property, wrong `'T` → conversion / unbox failure → **throws**, `tryGet` does not swallow it
- Void method inferred as a value → binder exception

Annotate `'T` / `unit` so the binder and the conversion agree.

## A present null exists

`Dyn.exists "x"` is true if `InvokeGet` succeeded, including when the value is null. `tryGet` returns `Some null` for a reference type. That is different from missing.

## Null target

`Dyn.tryGet` and `Dyn.exists` on a **null target** throw `NullReferenceException`. That is not a binder miss, so it is not `None` / `false`. Same for `Dyn.set`. A present null *value* is still `Some null` / `exists` true. Changing the null-target contract is [#111](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/111).

## More than 14 arguments

A call with **15 or more** arguments throws `TypeLoadException: Cannot Emit long delegates without ImpromptuInterface installed`. Fourteen arguments work. Dynamitey 3.0.3 never shipped ImpromptuInterface with this package. A fix belongs in a newer Dynamitey, not in a vendor here ([#29](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/29)).

## C# optional parameters

The DLR does not fill CLR optional defaults. `obj?Opt(5)` and `obj?Opt(Dyn.namedArg "a" 5)` both fail (`No overload takes 1 arguments`) when `Opt` is `Opt(int a, int b = 0)`. `Dyn.namedArg` only builds an `InvokeArg`; the miss is on the call. Pass every argument. Named args work when the full set is present.

## Function-typed results delay the call

If F# infers `'TResult` as a function, `?` / `Dyn.get` / `Dyn.invokeMember` return a callable. A missing member in that mode used to look like `Some (fun …)` until you applied it. `tryGet` does `InvokeGet` first, so a missing member is `None` even when `'T` is a function.

## Explicit interface and COM

COM and `IDynamicMetaObjectProvider` objects can succeed on names that reflection does not list. Existence checks go through `InvokeGet`, not `GetMemberNames`.

## Trimming and NativeAOT

This library is DLR-based. It is not trim-safe or NativeAOT-safe. Members it resolves at runtime can be removed by the trimmer. Do not publish a trimmed consumer and expect `?` to keep working.

## Historical .NET Core 2.0.0–2.0.2 bug

Nested classes inside generic classes broke C# `dynamic` (substring argument-length exceptions). .NET Core 2.0.3+ and every current TFM this package ships are fine. Recorded as [#11](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/11).

## Security

Member names are capabilities. Do not build `?` / `Dyn.get` / `Dyn.invokeMember` names from untrusted input. See [SECURITY.md](https://github.com/fsprojects/FSharp.Interop.Dynamic/blob/master/SECURITY.md).
