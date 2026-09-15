# Operators

`open FSharp.Interop.Dynamic` auto-opens `FSharp.Interop.Dynamic.TopLevelOperators`.

## `?` — get or invoke a named member

```fsharp
let length: int = "hello"?Length
let hello: string = "HelloWorld"?Substring(0, 5)
```

What happens depends on the **result type F# infers**:

- If `'TResult` is **not** a function, `?` does a DLR get (`InvokeGet`). `"hello"?Length` is a property get.
- If `'TResult` **is** a function, `?` returns a callable. `"HelloWorld"?Substring` has type `int * int -> string` (or whatever you bind), and applying it does `InvokeMember`.

That is why `target?Foo(1, 2)` works as a method call: the application forces a function type.

A void CLR method is an `unit`-returning function:

```fsharp
let items = ResizeArray<string>()
items?Add("1")
```

## `?<-` — set a named member

```fsharp
open System.Dynamic
let o = ExpandoObject()
o?Name <- "Ada"
```

Always a DLR set (`InvokeSet`). The result is `unit`.

## `!?` — invoke the target itself

```fsharp
let add = (+)
let add3: int -> int = !?add 3
add3 4  // 7
```

`!?` is `Dyn.invocation target Direct`. Use it on:

- An F# function boxed as `obj`
- A Dynamitey curry: `!?Dynamic.Curry(...)?Format("Test {0}")`
- Anything the DLR can `Invoke`

On a non-callable, both binders fail and you get `AggregateException` wrapping two `RuntimeBinderException`s.

## Named arguments

`Dyn.namedArg` wraps a value in a `FSharp.Interop.Dynamic.BridgeSupport.InvokeArg`, so it can be passed to any dynamic call in any order:

```fsharp
let described: string = target?Describe(Dyn.namedArg "two" 2, Dyn.namedArg "one" 1)
```

If you also use the Dynamitey package, its APIs (`Build<_>.NewObject`, `Dynamic.Invoke`) want Dynamitey's own `InvokeArg`, not this one.

pythonnet:

```fsharp
np?array([| 6.; 5.; 4. |], Dyn.namedArg "dtype" np?int32)
```

## Static members

```fsharp
open System.Linq
let empty: int seq = Dyn.staticTarget<Enumerable> |> Dyn.invokeGeneric "Empty" [typeof<int>] ()
```

`Dyn.staticContext typeof<Foo>` is the same thing when you have a `Type` in hand instead of a type parameter.

## Events

```fsharp
poco |> Dyn.memberAddAssign "Event" handler
poco |> Dyn.memberSubtractAssign "Event" handler
```

That is DLR `+=` / `-=` on the named member, not F# event syntax.
