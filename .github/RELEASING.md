# Releasing Ethar.GeoPose

Publishing to nuget.org cannot be undone or repeated for the same version, so every release goes through a dry run first.

## Branches

- `development` is the integration branch. Feature branches are squash merged into it by pull request.
- `main` only receives merges from `development`. A pull request into `main` is a release candidate.
- `upm` holds the Unity package. Only the Release workflow writes to it.

## Version

`<Version>` in `Directory.Build.props` is the only place the version lives. Both NuGet packages, the `v<version>` tag, the `upm-v<version>` tag and `package.json` on the `upm` branch take it from there. Bump it in the pull request that carries the change. Breaking changes bump the major version.

## Workflows

| Workflow | Trigger | What it does | Publishes |
| --- | --- | --- | --- |
| Development dotnet build and test | pull requests into `development` or `main`, pushes to `development` | build, test, pack | nothing |
| Release preflight | pull requests into `main` | build, test, pack, validate packages, check the version is free on nuget.org and as a tag, show the `upm` diff, check `NUGET_TOKEN` exists | nothing |
| Release | manual, `dry-run` on by default | everything preflight does, plus zips and an artifact with the packages and `upm` payload | nothing while `dry-run` is on |
| Release with `dry-run` off | manual, from `main` only | tags `main`, creates the GitHub release, updates and tags `upm`, pushes to nuget.org, opens a PR to refresh `development` | yes |

## Release steps

1. Open a pull request from `development` into `main`. Read the Release preflight summary and fix anything it reports.
2. Fill in `.github/ReleaseNotes.md`. It becomes the NuGet release notes and the GitHub release body.
3. Merge the pull request with a merge commit.
4. Run the Release workflow on `main` with `dry-run` on. Download the `release-files` artifact and inspect the packages if you want to.
5. Run the Release workflow on `main` with `dry-run` off. Approve it in the `release` environment if reviewers are configured there.
6. Merge the pull request the workflow opens to refresh `development` from `main`.

## Secrets and settings

- `NUGET_TOKEN`: a nuget.org API key with push rights for `Ethar.GeoPose` and `Ethar.GeoPose.Authority`. Preflight fails if it is missing.
- The workflows use the built-in `GITHUB_TOKEN` for tags, releases and the `upm` branch. No personal access token is needed.
- Optional: add required reviewers to the `release` environment under repository settings so a real release needs a second approval.

## If a release fails half way

Tags, GitHub releases and `upm` commits can be deleted and the run repeated. A package that reached nuget.org cannot. If one package was pushed and the other was not, bump the patch version and release again rather than trying to re-push.
