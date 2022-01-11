# FSharp.Interop.Dynamic [![NuGet Status](http://img.shields.io/nuget/v/FSharp.Interop.Dynamic.svg?style=flat)](https://www.nuget.org/packages/FSharp.Interop.Dynamic/)


The F# Dynamic Operator, powered by the DLR. Compiled for .NET Standard 2.0, .NET Standard 1.6, .NET Framework 4.5

Install from [NuGet](https://nuget.org/packages/FSharp.Interop.Dynamic/)
```
PM> Install-Package FSharp.Interop.Dynamic
```

# Build Status

Platform | Status
-------- | ------
Nuget Deployment | [![Build status](https://ci.appveyor.com/api/projects/status/tbw9put64a0p3j9o/branch/master?svg=true)](https://ci.appveyor.com/project/jbtule/fsharp-dynamic-832/branch/master)
Mac/Linux/Windows | [![Action Status](https://github.com/fsprojects/FSharp.Interop.Dynamic/workflows/.NET%20Core%20CI/badge.svg)](https://github.com/fsprojects/FSharp.Interop.Dynamic/actions?workflow=.NET+Core+CI)
Coverage| [![codecov](https://codecov.io/gh/fsprojects/FSharp.Interop.Dynamic/branch/master/graph/badge.svg)](https://codecov.io/gh/fsprojects/FSharp.Interop.Dynamic) [![Coverage Status](https://coveralls.io/repos/github/fsprojects/FSharp.Interop.Dynamic/badge.svg?branch=master)](https://coveralls.io/github/fsprojects/FSharp.Interop.Dynamic?branch=master)
 
 
# Bleeding edge feed on MyGet

[![MyGet Pre Release](https://img.shields.io/myget/dynamitey-ci/vpre/FSharp.Interop.Dynamic.svg)](https://www.myget.org/feed/dynamitey-ci/package/nuget/FSharp.Interop.Dynamic)

# Usage

`target?Property`, `target?Property<-value`, and `target?Method(arg,arg2)` allow you to dynamically get/set properties and call methods

Also `Dyn.implicitConvert`,`Dyn.explicitConvert`, comparison operators and more.


# Examples:

### System.Dynamic
```fsharp
open FSharp.Interop.Dynamic
let ex1 = ExpandoObject()
ex1?Test<-"Hi"//Set Dynamic Property
ex1?Test //Get Dynamic
```

### MVC ViewBag

```fsharp
x.ViewBag?Name<-"George"
```

### Dynamitey

```fsharp
open FSharp.Interop.Dynamic
open Dynamitey.DynamicObjects

let ComBinder = LateType("System.Dynamic.ComBinder, System.Dynamic, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")

let getPropertyNames (target:obj) =
  seq {
    yield! target.GetType().GetTypeInfo().GetProperties().Select(fun it -> it.Name)
    if (ComBinder.IsAvailable) then
      yield! ComBinder?GetDynamicDataMemberNames(target)
  }

```


### Python Interop

Translated from this example C# code: https://github.com/SciSharp/pythonnet#example

```fsharp
open Python.Runtime
open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators

do
  use __ = Py.GIL()

  let np = Py.Import("numpy")
  np?cos(np?pi ?*? 2)
  |> printfn "%O"

  let sin: obj -> obj = np?sin
  sin 5 |> printfn "%O"

  np?cos 5 ?+? sin 5
  |> printfn "%O"

  let a: obj = np?array([| 1.; 2.; 3. |])
  printfn "%O" a?dtype

  let b: obj = np?array([| 6.; 5.; 4. |], Dyn.namedArg "dtype" np?int32)
  printfn "%O" b?dtype

  a ?*? b
  |> printfn "%O"
```

Output

```
1.0
-0.9589242746631385
-0.6752620891999122
float64
int32
[ 6. 10. 12.]
```

### SignalR (.net framework version)

```fsharp
open FSharp.Interop.Dynamic
type MyHub =
    inherit Hub
    member x.Send (name : string) (message : string) =
        base.Clients.All?addMessage(name,message) |> ignore
```

# Caveats:

The `DLR` is incompatible with interface explicit members, so are these operators, [just like C#'s `dynamic` keyword](http://stackoverflow.com/questions/22514892/iterate-through-a-dictionary-inserted-in-a-asp-net-mvc4-pages-viewdata-via-f-c).

[.NET Core 2.0.0 to 2.0.2 had a major bug in the C# dynamic keyword with nested classes inside of generic classes.](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/11). You will know it from a substring argument length exception. .NET Framework 4.0+, .NET Core 1.x and .NET Core 2.0.3+ and later are unaffected.

## Maintainer(s)

- [@jbtule](https://github.com/jbtule)
- [@forki](https://github.com/forki)

The default maintainer account for projects under "fsprojects" is [@fsprojectsgit](https://github.com/fsprojectsgit) - F# Community Project Incubation Space (repo management)
