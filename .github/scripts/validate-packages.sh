#!/usr/bin/env bash
set -euo pipefail

version="${1:?version required}"
dir="${2:?packages dir required}"
fail=0

for id in Ethar.GeoPose Ethar.GeoPose.Authority Ethar.GeoPose.H3; do
  pkg="$dir/$id.$version.nupkg"
  sym="$dir/$id.$version.snupkg"
  if [ ! -f "$pkg" ]; then
    echo "::error::Missing package $pkg"
    ls -1 "$dir" || true
    fail=1
    continue
  fi
  [ -f "$sym" ] || echo "::warning::Missing symbols package $sym"

  listing=$(unzip -Z1 "$pkg")
  for want in "$id.nuspec" readme.md nuget.png LICENSE "lib/netstandard2.0/$id.dll" "lib/net48/$id.dll"; do
    if ! grep -qx "$want" <<< "$listing"; then
      echo "::error::$pkg is missing $want"
      fail=1
    fi
  done

  nuspec=$(unzip -p "$pkg" "$id.nuspec")
  nuspec_version=$(sed -n -E 's:.*<version>([^<]+)</version>.*:\1:p' <<< "$nuspec" | head -1)
  if [ "$nuspec_version" != "$version" ]; then
    echo "::error::$pkg has nuspec version '$nuspec_version', expected '$version'"
    fail=1
  fi

  if ! grep -q "<releaseNotes>" <<< "$nuspec"; then
    echo "::warning::$pkg has no release notes. Fill in .github/ReleaseNotes.md before releasing."
  fi

  if [ "$id" != "Ethar.GeoPose" ]; then
    deps=$(grep -oE 'id="Ethar.GeoPose" version="[^"]+"' <<< "$nuspec" | sort -u)
    if [ "$(wc -l <<< "$deps")" -ne 1 ] || ! grep -q "version=\"$version\"" <<< "$deps"; then
      echo "::error::$pkg must depend on exactly Ethar.GeoPose $version, found: $deps"
      fail=1
    fi
  fi

  tfms=$(grep -oE '^lib/[^/]+/' <<< "$listing" | sort -u | tr '\n' ' ')
  echo "OK $pkg (targets: $tfms)"
done

exit $fail
