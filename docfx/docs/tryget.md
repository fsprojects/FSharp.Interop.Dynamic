# tryGet and exists

[#27](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/27): unstructured objects, check `o?myProp` without eating a `RuntimeBinderException`.

These ship on Dynamitey **3.0.3**. They do not need Dynamitey 4.0.0.

## `Dyn.tryGet`

```fsharp
open System.Dynamic
open FSharp.Interop.Dynamic

let o = ExpandoObject()
o?myProp <- "hi"

let present: string option = o |> Dyn.tryGet "myProp"   // Some "hi"
let missing: string option = o |> Dyn.tryGet "nope"     // None
```

Lookup is `Dynamic.InvokeGet` only. Then the existing conversion / callable wrapping runs **outside** the catch.

| Situation | Result |
| --- | --- |
| Member missing | `None` |
| Member present, value converts to `'T` | `Some value` |
| Member present, value is null | `Some null` (for a reference `'T`) |
| Member present, value cannot convert to `'T` | throws (`InvalidCastException` or a binder exception) — **not** `None` |
| Member missing, `'T` is a function type | `None` immediately (does not return a lazy callable) |
| Getter throws something else | that exception propagates |
| Target is null | `NullReferenceException` (not `None`). [#110](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/110), [#111](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/111) |

## `Dyn.exists`

```fsharp
o |> Dyn.exists "myProp"   // true
o |> Dyn.exists "nope"     // false
```

True when `InvokeGet` succeeds. A present null is still present. A throwing getter is not reported as missing. A **null target** throws `NullReferenceException`, not `false`.

`exists` does not convert the value. That is the difference from `tryGet >> Option.isSome` when conversion would fail: `exists` can be true while `tryGet` as `int` throws.

## Do not use `GetMemberNames` for this

Dynamitey exposes `Dynamic.GetMemberNames`. It lists names; it is not the same test as `o?myProp`. COM, some `IDynamicMetaObjectProvider` implementations, and binder tricks can succeed on a get whose name is not in that list. `tryGet` / `exists` use the same `InvokeGet` as `?`.
