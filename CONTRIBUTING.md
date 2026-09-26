# Contributing

## Before you start

- Small fixes: open a pull request.
- Anything larger: open an issue first.
- Security problems: follow [SECURITY.md](SECURITY.md), not a public issue.

## How a change gets merged

All changes reach `master` through a pull request, squash-merged.

1. Open the PR. CI runs the build, tests, coverage, analyzers, the docs build, and the security scans.
2. Copilot code review runs on the PR. Work through every comment: take the fix (Copilot can usually make it), or reply with why it is a false positive and resolve it.
3. Once CI is green and every Copilot comment is addressed or answered, the PR is merged. A maintainer review is welcome but not required.

## Requirements for a pull request

- **CI is green.** The build uses `-warnaserror` on Linux, Windows, and macOS, with the .NET analyzers at `AnalysisMode=All`, the Ionide F# analyzers, and FS1182 (unused bindings) on. Fix warnings; do not suppress them.
- **Behavior changes come with tests** in `Tests/` (xUnit + FsUnit). The suite must be 0 failed, 0 skipped, and must stay above the coverage floors below.
- **Docs follow the code.** Public API changes update the XML doc comments and any affected page under `docfx/`. README code samples are compiled by `Tests/ReadmeExamples.fs`, so keep the two in step.
- **Style matches the surrounding code.** No formatter is enforced; follow the naming, layout, and comment density of the file you are editing.
- **Dependencies are pinned.** Versions live in `Directory.Packages.props`. When you change one, regenerate the lock files with `dotnet restore --force-evaluate` and commit the updated `packages.lock.json` files. CI restores with `--locked-mode` and fails on a stale lock file.

## Building

Requires the .NET 10 SDK.

```bash
dotnet restore
dotnet build -c Release -warnaserror
dotnet test --project Tests/Tests.fsproj -c Release
```

The suite must be 0 failed, 0 skipped. Coverage floors are 77% line and 100% branch on `FSharp.Interop.Dynamic`.

## Documentation

Conceptual pages and the API reference live under `docfx/`. Do not resurrect `build.fsx` / FSharp.Formatting.

```bash
dotnet build FSharp.Interop.Dynamic/FSharp.Interop.Dynamic.fsproj -c Release
docfx docfx/docfx.json --warningsAsErrors
```

PRs build the site; only `master` deploys to GitHub Pages.

## `netstandard2.0` stays on 7.0.0, and ends there

7.0.0 is the last release that targets `netstandard2.0`. That is the build a .NET Framework 4.6.1 through 4.8.1 application uses. Do not drop it in a 7.x change. Framework 4.8.1 is serviced with Windows and does not gain BCL APIs, so those applications stay on 7.0.0.

8.0.0 ([#108](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/108), [milestone](https://github.com/fsprojects/FSharp.Interop.Dynamic/milestone/2)) targets `net10.0` and `net11.0` only. The target date is 10 November 2026. It waits on [Dynamitey.Community 5.0.0](https://github.com/dynamitey-community/dynamitey/issues/95), which drops the same target once .NET 11 is generally available. The point of that drop is one implementation: `ThrowIfNull`, ordinal hashing, and the BCL .NET 10 and .NET 11 already ship, without a second Framework copy of the call-site code.
