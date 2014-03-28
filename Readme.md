# FSharp.Dynamic [![Build Status](https://travis-ci.org/fsprojects/FSharp.Dynamic.svg?branch=master)](https://travis-ci.org/fsprojects/FSharp.Dynamic) [![Build status](https://ci.appveyor.com/api/projects/status/tbw9put64a0p3j9o/branch/master)](https://ci.appveyor.com/project/jbtule/fsharp-dynamic-832)

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
