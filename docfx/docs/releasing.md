# Releasing

NuGet publish is tag-triggered. The workflow is `.github/workflows/publish.yml`. The API key is the repo secret `NUGET_PUBLISH_KEY` (installed in #28).

## Version

`VersionPrefix` is **6.0.0**. That is the TFM break (`net45` / `netstandard1.6` dropped). nuget.org's last version is 5.0.1.268.

## Cut 6.0.0

First make sure [`CHANGELOG.md`](https://github.com/fsprojects/FSharp.Interop.Dynamic/blob/master/CHANGELOG.md) has a `## [6.0.0]` section that describes the release. Then, on `master`, after CI is green:

```bash
git tag v6.0.0
git push origin v6.0.0
```

The Publish workflow will:

1. Extract the `## [6.0.0]` section of `CHANGELOG.md`, and fail if there isn't one
2. Restore, build `-warnaserror`, test
3. Pack `FSharp.Interop.Dynamic` at `6.0.0` (nupkg + snupkg)
4. `dotnet nuget push` to nuget.org with `--skip-duplicate`
5. Create the GitHub release `v6.0.0` with those notes and the packages attached. A version with a `-` suffix is marked as a prerelease.

`workflow_dispatch` with a version input does steps 1–4 without a tag and creates no GitHub release. Prefer the tag.

Pull requests never publish.

## After it lands

Update the README / docs callout that nuget.org is still 5.0.1.268. The next patch is `v6.0.1`, not another 5.x.
