[![Issue Stats](http://issuestats.com/github/fsprojects/FSharp.Dynamic/badge/issue)](http://issuestats.com/github/fsprojects/FSharp.Dynamic)
[![Issue Stats](http://issuestats.com/github/fsprojects/FSharp.Dynamic/badge/pr)](http://issuestats.com/github/fsprojects/FSharp.Dynamic)

# FSharp.Dynamic [![NuGet Status](http://img.shields.io/nuget/v/FSharp.Interop.Dynamic.svg?style=flat)](https://www.nuget.org/packages/FSharp.Dynamic/)


F# Dynamic Operator using the DLR (Portable Class Library WinRT, .NET 4.5, Silverlight 5)

Install from [nuget](https://nuget.org/packages/FSharp.Interop.Dynamic/)
```
PM> Install-Package FSharp.Interop.Dynamic
```

# Build Status

[![Build Status](https://travis-ci.org/fsprojects/FSharp.Interop.Dynamic.svg?branch=master)](https://travis-ci.org/fsprojects/FSharp.Interop.Dynamic) [![Build status](https://ci.appveyor.com/api/projects/status/tbw9put64a0p3j9o/branch/master)](https://ci.appveyor.com/project/jbtule/fsharp-dynamic-832)

# Usage

`target?Property`, `target?Property<-value`, and `target?Method(arg,arg2)` allow you to dynamically get/set properties and call methods

Also `>?>` (`dynImplicit`), `>>?>>` (`dynExplicit`) and more.


# Examples:

###System.Dynamic
    open FSharp.Interop.Dynamic
    let ex1 = ExpandoObject()
    ex1?Test<-"Hi"//Set Dynamic Property
    ex1?Test //Get Dynamic

###SignalR

    open FSharp.Interop.Dynamic
    type MyHub =
        inherit Hub
        member x.Send (name : string) (message : string) =
            base.Clients.All?addMessage(name,message) |> ignore

###MVC ViewBag

    x.ViewBag?Name<-"George"

#Caveats:

The `dlr` is incompatible with interface explicit members, so are these operators, [just like C#'s `dynamic` keyword](http://stackoverflow.com/questions/22514892/iterate-through-a-dictionary-inserted-in-a-asp-net-mvc4-pages-viewdata-via-f-c).

## Maintainer(s)

- [@jbtule](https://github.com/jbtule)
- [@forki](https://github.com/forki)

The default maintainer account for projects under "fsprojects" is [@fsgit](https://github.com/fsgit) - F# Community Project Incubation Space (repo management)
