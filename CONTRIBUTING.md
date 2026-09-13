# Contributing

## Before you start

- Small fixes: open a pull request.
- Anything larger: open an issue first.

## Building

Requires the .NET 10 SDK.

```bash
dotnet restore
dotnet build -c Release -warnaserror
dotnet test Tests/Tests.fsproj -c Release
```

The suite must be 0 failed, 0 skipped. Coverage floors are 77% line and 100% branch on `FSharp.Interop.Dynamic`.

## Documentation

Conceptual pages and the API reference live under `docfx/`. Do not resurrect `build.fsx` / FSharp.Formatting.

```bash
dotnet build FSharp.Interop.Dynamic/FSharp.Interop.Dynamic.fsproj -c Release
docfx docfx/docfx.json --warningsAsErrors
```

PRs build the site; only `master` deploys to GitHub Pages.

## `netstandard2.0` is deliberate

It is the TFM that still reaches .NET Framework and modern .NET from one package. Do not drop it to "simplify" to `net10.0` only.
