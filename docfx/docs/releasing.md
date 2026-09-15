# Releasing

NuGet publish is tag-triggered. The workflow is `.github/workflows/publish.yml`. It authenticates with nuget.org **Trusted Publishing** (`NuGet/login` + OIDC), not a long-lived API key. The nuget.org policy must name this repository and `publish.yml`, and the profile must own **both** package ids, `FSharp.Interop.Dynamic` and `FSharp.Interop.Dynamic.BridgeSupport`. The GitHub secret `NUGET_USER` is the nuget.org profile name.

## Version

`VersionPrefix` is **6.0.0**. That was the TFM break (`net45` / `netstandard1.6` dropped). The next release is `v6.0.1` (or `v6.1.0` / `v7.0.0`), not another 5.x.

## Cut a release

First make sure [`CHANGELOG.md`](https://github.com/fsprojects/FSharp.Interop.Dynamic/blob/master/CHANGELOG.md) has a `## [version]` section that describes the release. Then, on `master`, after CI is green:

```bash
git tag vX.Y.Z
git push origin vX.Y.Z
```

The Publish workflow will:

1. Extract the `## [6.0.0]` section of `CHANGELOG.md`, and fail if there isn't one
2. Restore, build `-warnaserror`, test
3. Pack `FSharp.Interop.Dynamic` and `FSharp.Interop.Dynamic.BridgeSupport` at `6.0.0` (nupkg + snupkg each)
4. Attest SLSA build provenance for all four files in one bundle, signed with Sigstore through GitHub's OIDC identity
5. `dotnet nuget push` both packages to nuget.org with `--skip-duplicate`
6. Create the GitHub release `v6.0.0` with those notes, all four packages, and the provenance bundle `FSharp.Interop.Dynamic.6.0.0.intoto.jsonl` (it covers BridgeSupport too) attached. A version with a `-` suffix is marked as a prerelease.

`workflow_dispatch` with a version input does steps 1–5 without a tag and creates no GitHub release. Prefer the tag.

## Verify a package

Anyone can check that a package was built by this repository's Publish workflow:

```bash
gh attestation verify FSharp.Interop.Dynamic.6.0.0.nupkg --repo fsprojects/FSharp.Interop.Dynamic
```

Pull requests never publish.

## After it lands

Update the README / docs callout that nuget.org is still 5.0.1.268. The next patch is `v6.0.1`, not another 5.x.
