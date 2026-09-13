# Releasing

NuGet publish is tag-triggered. The workflow is `.github/workflows/publish.yml`. The API key is the repo secret `NUGET_PUBLISH_KEY` (installed in #28).

## Version

`VersionPrefix` is **6.0.0**. That is the TFM break (`net45` / `netstandard1.6` dropped). nuget.org's last version is 5.0.1.268.

## Cut 6.0.0

On `master`, after CI is green:

```bash
git tag v6.0.0
git push origin v6.0.0
```

The Publish workflow will:

1. Restore, build `-warnaserror`, test
2. Pack `FSharp.Interop.Dynamic` at `6.0.0` (nupkg + snupkg)
3. `dotnet nuget push` to nuget.org with `--skip-duplicate`

`workflow_dispatch` with a version input does the same without a tag. Prefer the tag.

Pull requests never publish.

## After it lands

Update the README / docs callout that nuget.org is still 5.0.1.268. The next patch is `v6.0.1`, not another 5.x.
