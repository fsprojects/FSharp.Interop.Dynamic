# Dyn

`Dyn` is the function surface behind the operators. Use it when you want piping, a name as data, or an operation `?` does not spell.

Target is last on purpose, so `target |> Dyn.get "Name"` works.

## Read and write

| Function | Does |
| --- | --- |
| `Dyn.get name` | Property/field get |
| `Dyn.tryGet name` | Get, or `None` if the binder cannot find it |
| `Dyn.exists name` | `true` when `InvokeGet` succeeds (a present null still exists) |
| `Dyn.set name value` | Property/field set |
| `Dyn.getChain [ "A"; "B" ]` | `A.B` get |
| `Dyn.setChain [ "A"; "B" ] value` | `A.B` set |
| `Dyn.getIndexer indexes` | Indexer get |
| `Dyn.setIndexer indexes value` | Indexer set |

```fsharp
open System.Dynamic
let o = ExpandoObject()
o |> Dyn.set "Name" "Ada"
let name: string = o |> Dyn.get "Name"
let missing: string option = o |> Dyn.tryGet "NoSuch"
```

`tryGet` and `exists` catch **only** `RuntimeBinderException` from `InvokeGet`. A present member that cannot convert to `'T` still throws. See [tryGet and exists](tryget.md).

## Invoke

| Function | Does |
| --- | --- |
| `Dyn.invokeMember name args` | Named member, then apply `args` |
| `Dyn.invokeDirect args` | Invoke the target itself |
| `Dyn.invokeGeneric name typeArgs args` | Generic member |
| `Dyn.invocation target calling` | The workhorse; `Calling` is `Member`, `GenericMember`, or `Direct` |

```fsharp
let hello: string = "HelloWorld" |> Dyn.invokeMember "Substring" (0, 5)
let add3: int -> int = (+) |> Dyn.invokeDirect 3
```

If the inferred `'TResult` is a function, these return a callable rather than applying immediately. That is how F# currying of a DLR member works.

## Convert

```fsharp
let d: decimal = 50 |> Dyn.implicitConvert
let n: int = xelement |> Dyn.explicitConvert
```

The target type is inferred. `implicitConvertTo` / `explicitConvertTo` take an explicit `Type`.

## Named arguments and static context

```fsharp
Dyn.namedArg "dtype" someValue          // Dynamitey.InvokeArg
Dyn.staticContext typeof<Foo>
Dyn.staticTarget<Foo>
```

## Obsolete names

`Dyn.getIndex`, `setIndex`, `addAssignMember`, `subtractAssignMember`, and `invoke` with `string option` still exist and still work. They are the pre-partial-application argument order. New code should use `getIndexer`, `setIndexer`, `memberAddAssign`, `memberSubtractAssign`, and `invocation`.
