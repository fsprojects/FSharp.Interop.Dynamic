# Security policy

## Reporting a vulnerability

**Do not open a public issue for a security problem.**

Report it through GitHub's private vulnerability reporting:

> [**Report a vulnerability**](https://github.com/fsprojects/FSharp.Interop.Dynamic/security/advisories/new)

That creates a private advisory visible only to you and the maintainers. If the
link does not work for you, open a normal issue saying only that you have a
security report and would like a private channel — no details — and a
maintainer will open the advisory.

Useful things to include, as far as you have them: the version or commit, the
target framework, what an attacker can do, and a reproduction.

## What to expect

This project is maintained by volunteers, so these are honest intentions rather
than a contractual commitment:

| | |
| --- | --- |
| Acknowledgment | within 7 days |
| Initial assessment | within 30 days |
| Fix or a decision not to fix | depends on severity and complexity |

You will be credited in the advisory unless you ask not to be.

## Supported versions

The published artefact is the `FSharp.Interop.Dynamic` package on nuget.org.

| Version | Support |
| --- | --- |
| Latest release on nuget.org | Reports accepted |
| `master` | Reports accepted; this is where fixes land |
| Older NuGet versions | Reports accepted; fixes ship in a new release, not as a patch of an old TFM |

This repository cannot issue fixes for **Dynamitey**. A vulnerability in that
dependency should be reported to
[`dynamitey-community/dynamitey`](https://github.com/dynamitey-community/dynamitey)
(the continuation) or, for the original package, its publishers. Reports about
*this* library's use of Dynamitey are still welcome here.

## Scope

This library performs dynamic dispatch through the DLR. Two consequences are
worth stating plainly, because they are properties of the design rather than
defects:

- **Invoking members named at runtime is what the library does.** If an
  application passes attacker-controlled strings to `?`, `Dyn.invokeMember`,
  `Dyn.get`, `Dyn.set`, `Dyn.invokeGeneric`, or `Dyn.invokeDirect`, that
  application has given the attacker the ability to call arbitrary members on
  the target. That is a vulnerability in the calling application, not in this
  library. Treat member names as you would treat SQL: never build them from
  untrusted input.
- **The library is not trim-safe or AOT-safe.** Trimming a consuming
  application can remove members this library resolves at runtime, turning a
  working call into a runtime failure.

Reports that this library will invoke whatever member it is asked to invoke are
not vulnerabilities. Reports that it can be made to invoke something it was
*not* asked to invoke very much are.

## What is scanned

Every pull request runs:

- CodeQL (`security-and-quality`) against the shipped library and against
  GitHub Actions workflows
- OpenSSF Scorecard
- Microsoft DevSkim
- a dependency review that blocks known-vulnerable dependencies
- `dotnet list package --vulnerable --include-transitive`

Dependabot is enabled for NuGet and GitHub Actions.

CodeQL has no F# extractor. The csharp job still builds the shipped project so
any extractable IL/C# surface is queried; the Actions job is the part that is
guaranteed to see this repository's YAML. DevSkim reads F# source as text.
