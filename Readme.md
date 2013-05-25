# FSharp.Dynamic [![Build Status](https://travis-ci.org/ekonbenefits/FSharp.Dynamic.png?branch=master)](https://travis-ci.org/ekonbenefits/FSharp.Dynamic)

F# Dynamic Operator using the DLR (Portable Class Library WinRT, .NET 4.5, Silverlight 5)

*If you need .NET 40 support, for right now, continue using [ImpromptuInterface.FSharp](https://github.com/ekonbenefits/impromptu-interface)* from which this was ported from.

`target?Property`, `target?Property<-value`, and `target?Method(arg,arg2)` allow you to dynamically get/set properties and call methods

# To Do:

 - Backport to .net 4.0

# Examples:

```fsharp
let ex1 = ExpandoObject()
ex1?Test<-"Hi"//Set Dynamic Property
ex1?Test //Get Dynamic Property
```
