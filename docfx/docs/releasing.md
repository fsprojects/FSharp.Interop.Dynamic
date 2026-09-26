# Releasing

NuGet publish is tag-triggered. The workflow is `.github/workflows/publish.yml`. It authenticates with nuget.org **Trusted Publishing** (`NuGet/login` + OIDC), not a long-lived API key. The nuget.org policy must name this repository and `publish.yml`. The GitHub secret `NUGET_USER` is the nuget.org profile name.

## Version

`VersionPrefix` is **7.0.0**. That release references Dynamitey.Community 4.0.0 and targets `netstandard2.0` and `net10.0`. It is the last release that supports .NET Standard 2.0, and so the last release a .NET Framework 4.6.1 through 4.8.1 application can take.

**8.0.0** drops `netstandard2.0` and targets `net10.0` and `net11.0` ([#108](https://github.com/fsprojects/FSharp.Interop.Dynamic/issues/108)). It follows [Dynamitey.Community 5.0.0](https://github.com/dynamitey-community/dynamitey/issues/95). Not another 5.x.

## Cut a release

First make sure [`CHANGELOG.md`](https://github.com/fsprojects/FSharp.Interop.Dynamic/blob/master/CHANGELOG.md) has a `## [version]` section that describes the release. Then, on `master`, after CI is green:

```bash
git tag vX.Y.Z
git push origin vX.Y.Z
```

The Publish workflow will:

1. Extract the `## [X.Y.Z]` section of `CHANGELOG.md` for the tagged version, and fail if there isn't one
2. Restore, build `-warnaserror`, test
3. Pack `FSharp.Interop.Dynamic` at `X.Y.Z` (nupkg + snupkg)
4. Attest SLSA build provenance for the nupkg and snupkg, signed with Sigstore through GitHub's OIDC identity
5. `dotnet nuget push` to nuget.org with `--skip-duplicate`
6. Create the GitHub release `vX.Y.Z` with those notes, the packages, and the provenance bundle `FSharp.Interop.Dynamic.X.Y.Z.intoto.jsonl` attached. A version with a `-` suffix is marked as a prerelease.

`workflow_dispatch` with a version input does steps 1–5 without a tag and creates no GitHub release. Prefer the tag.

## Verify a package

Anyone can check that a package was built by this repository's Publish workflow:

```bash
gh attestation verify FSharp.Interop.Dynamic.X.Y.Z.nupkg --repo fsprojects/FSharp.Interop.Dynamic
```

Pull requests never publish.

## After it lands

Confirm nuget.org lists the new version and that README / getting-started install instructions match it. After 7.0.0, the next planned major is 8.0.0 (drop `netstandard2.0`). Not another 5.x.
