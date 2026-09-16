# Changelog

Each release has a `## [version]` section. When a `v*.*.*` tag is pushed, the Publish workflow copies that section into the GitHub release, and it fails if the section is missing.

## [Unreleased]

### Docs

- Caveats document 15-argument `TypeLoadException`, C# optional parameters, and `tryGet` / `exists` on a null target (#110).

## [6.0.0]

### Breaking

- Target frameworks are now `netstandard2.0` and `net10.0`. `net45` and `netstandard1.6` are dropped; `netstandard2.0` still covers .NET Framework 4.6.1+ and modern .NET.
- Minimum dependency versions are raised: FSharp.Core 4.7.2 (was 4.2), Dynamitey 3.0.3 (was 2.0), Microsoft.CSharp 4.7.0 (was 4.6.0). FSharp.Core is kept deliberately low so consumers on older F# toolchains can still use the package (#80).

### Added

- `Dyn.tryGet` and `Dyn.exists`: look up a member as `'T option`, or test for it, without throwing (#42, #43).

### Build, security, and docs

- Build moved to the .NET 10 SDK. CI runs on Linux, Windows, and macOS with warnings as errors, analyzers, and a coverage gate (#31).
- NuGet publishing runs from a tag-triggered workflow (#45).
- Added a security policy, code scanning, and OpenSSF Scorecard. Actions are pinned to commit SHAs, and NuGet restore uses lock files (#36, #39, #41, #70).
- Documentation moved from FAKE and FSharp.Formatting to a DocFX site on GitHub Pages. README examples are compiled as tests (#47, #51, #53).
- XML documentation on `Dyn`, the operators, and `SymbolicString` so API-reference summaries describe what each member does (#49).
- Pull-request checks no longer show skipped deploy / full-OSV / nuget-push jobs, or an ignored Scorecard SARIF check (#98).

## [5.0.1.268] - 2019-10-03

- Packages the `netstandard2.0` target together with its dependencies.
- Adds SourceLink and a symbols package (snupkg).

## [5.0.0.25] - 2019-09-11

No release notes were published for this version.

## [4.0.3.130] - 2018-04-05

- Adds a .NET Standard version.
- Adds the `Dyn` helper module.
- Adds new dynamic operators.
