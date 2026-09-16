# Why this library

F# has no `dynamic` keyword. C# does. A lot of .NET still hands you objects whose shape is decided at runtime: `ExpandoObject`, `DynamicObject`, COM, pythonnet, SignalR clients, JSON bags deserialized without a type. From F#, without this library, you write a C# helper, sprinkle `:?` type tests, or wrap every access in `try/with` on `RuntimeBinderException`.

This library is the F# spelling of that call: `?`, `?<-`, `!?`, and `Dyn.*`, including `tryGet` / `exists` which C# `dynamic` does not have.

Use ordinary F# when the member is known at compile time. Use this library when it is not.

## 1. The payload has no F# type

An HTTP API, a script host, or a C# component gave you a bag of properties. There is no record to deserialize into yet, or the schema changes per tenant.

```fsharp
open System.Dynamic
open FSharp.Interop.Dynamic

let payload = ExpandoObject()
payload?userId <- "u-17"
payload?role <- "admin"
let userId: string = payload?userId
```

`payload.userId` does not compile: `ExpandoObject` has no such property. `payload?userId` does.

## 2. The field might be missing

`?` throws `RuntimeBinderException` when the binder cannot find the member. That is the wrong primitive for optional JSON.

```fsharp
open System.Dynamic
open FSharp.Interop.Dynamic

let payload = ExpandoObject()
payload?userId <- "u-17"
let role: string option = payload |> Dyn.tryGet "role"     // None
let userId: string option = payload |> Dyn.tryGet "userId" // Some "u-17"
```

`tryGet` is `None` on a miss and `Some` on a hit, including a present null. A present value that cannot convert to `'T` still throws. See [tryGet and exists](tryget.md).

## 3. The member name is data

A config file, a query string, or a spreadsheet header named the method. You cannot write `x.ToUpper` because `ToUpper` is not in the source.

```fsharp
let name = "ToUpper"
let result: string = "ada" |> Dyn.invokeMember name ()
```

That is the case C# `dynamic` also handles poorly: `d.ToUpper()` still requires the identifier `ToUpper` in source. `Dyn.invokeMember` takes a string.

## 4. A C# API returned `dynamic`

ASP.NET `ViewBag`, some SignalR client proxies, and older Office/COM wrappers expose DLR objects. F# sees `obj`. This library is how you call them without a C# trampoline.

```fsharp
type Client() =
    inherit DynamicObject()
    member val Last = "" with get, set
    override this.TryInvokeMember(binder, args, result) =
        this.Last <- sprintf "%s(%O)" binder.Name args.[0]
        result <- null
        true

let client = Client()
let _: unit = client?addMessage("hi")
```

The SignalR shape is the same idea: `clients.All?addMessage(name, message)`. That snippet is not compiled in this repo (no SignalR package). The `DynamicObject` above is, and is the same DLR path.

## 5. Python, COM, Office

pythonnet wraps Python objects as DLR objects. Excel/COM does too. The F# you write is the same `?` / `Dyn.namedArg` you use on an Expando.

```fsharp
// Sketch only — pythonnet is not referenced here.
// open FSharp.Interop.Dynamic
// open FSharp.Interop.Dynamic.Operators
// np?cos(np?pi ?*? 2)
// np?array([| 6.; 5.; 4. |], Dyn.namedArg "dtype" np?int32)
```

If you already use pythonnet from C# with `dynamic`, this is the F# equivalent.

## When not to use it

- The type is in your F# (or a referenced C#) project and the member is known. Call it normally.
- You need NativeAOT or trimming. The DLR will not survive that. See [Caveats](caveats.md).
- Member names come from the user. Treat them like SQL identifiers; see [SECURITY.md](https://github.com/fsprojects/FSharp.Interop.Dynamic/blob/master/SECURITY.md).

## What is tested

Every F# block in the README that is presented as working code is a fact in `Tests/ReadmeExamples.fs`. The numbered examples on this page (except the pythonnet sketch) are facts in `Tests/WhyExamples.fs`. If an example here changes, change the test too.
