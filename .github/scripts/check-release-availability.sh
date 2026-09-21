#!/usr/bin/env bash
# Checks that a version can still be released: the git tag is free and neither package
# exists on nuget.org at that version. NuGet.org never lets a version be re-published,
# even after unlisting, so this is the last line of defence before an irreversible push.
#
# Usage: check-release-availability.sh <version>
# Needs: git remote "origin", curl. Exits non-zero if anything is already taken.
set -euo pipefail

version="${1:?version required}"
fail=0

if git ls-remote --exit-code --tags origin "refs/tags/v$version" > /dev/null 2>&1; then
  echo "::error::Tag v$version already exists on origin. Bump <Version> in Directory.Build.props."
  fail=1
else
  echo "Tag v$version is free"
fi

if git ls-remote --exit-code --tags origin "refs/tags/upm-v$version" > /dev/null 2>&1; then
  echo "::error::Tag upm-v$version already exists on origin. Bump <Version> in Directory.Build.props."
  fail=1
else
  echo "Tag upm-v$version is free"
fi

lower_version=$(printf '%s' "$version" | tr '[:upper:]' '[:lower:]')
for id in Ethar.GeoPose Ethar.GeoPose.Authority; do
  lower_id=$(printf '%s' "$id" | tr '[:upper:]' '[:lower:]')
  url="https://api.nuget.org/v3-flatcontainer/$lower_id/$lower_version/$lower_id.nuspec"
  code=$(curl -s -o /dev/null -w '%{http_code}' "$url")
  case "$code" in
    404) echo "nuget.org: $id $version is free" ;;
    200) echo "::error::nuget.org already has $id $version. It cannot be re-published. Bump <Version> in Directory.Build.props."; fail=1 ;;
    *)   echo "::error::nuget.org returned HTTP $code for $url. Cannot confirm $id $version is free."; fail=1 ;;
  esac
done

exit $fail
