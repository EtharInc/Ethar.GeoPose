#!/usr/bin/env bash
# Copies a UPM payload over a checkout of the upm branch and sets the package.json version.
# Prints what changed. It does not commit or push, so it is safe for a dry run.
#
# Usage: apply-upm-payload.sh <payload-dir> <upm-checkout-dir> <version>
set -euo pipefail

payload="${1:?payload dir required}"
upm="${2:?upm checkout dir required}"
version="${3:?version required}"

if [ ! -f "$upm/package.json" ]; then
  echo "::error::$upm does not look like a checkout of the upm branch (no package.json)"
  exit 1
fi

cp -R "$payload"/. "$upm"/

# Replace only the top-level version field so the rest of package.json keeps its formatting.
sed -i -E "0,/\"version\"[[:space:]]*:[[:space:]]*\"[^\"]*\"/s//\"version\": \"$version\"/" "$upm/package.json"
applied=$(sed -n -E 's/.*"version"[[:space:]]*:[[:space:]]*"([^"]*)".*/\1/p' "$upm/package.json" | head -1)
if [ "$applied" != "$version" ]; then
  echo "::error::Failed to set package.json version to $version (got '$applied')"
  exit 1
fi

echo "Changes on the upm branch that a release would commit:"
git -C "$upm" add -A
git -C "$upm" status --short
git -C "$upm" diff --cached --stat
git -C "$upm" reset -q
