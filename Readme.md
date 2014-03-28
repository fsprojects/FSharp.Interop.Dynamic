# FSharp.Dynamic [![Build Status](https://travis-ci.org/fsprojects/FSharp.Dynamic.svg?branch=master)](https://travis-ci.org/fsprojects/FSharp.Dynamic) [![Build status](https://ci.appveyor.com/api/projects/status/8e9kl1749tgohc5y/branch/master)](https://ci.appveyor.com/project/jbtule/fsharp-dynamic)

F# Dynamic Operator using the DLR (Portable Class Library WinRT, .NET 4.5, Silverlight 5) + .NET 4.0 Library

Install from [nuget](https://nuget.org/packages/FSharp.Dynamic/)
```
PM> Install-Package FSharp.Dynamic
```

`target?Property`, `target?Property<-value`, and `target?Method(arg,arg2)` allow you to dynamically get/set properties and call methods


# Examples:

```fsharp
let ex1 = ExpandoObject()
ex1?Test<-"Hi"//Set Dynamic Property
ex1?Test //Get Dynamic Property
```
