#!/usr/bin/env bash
# Collects the files that the upm branch receives on release into one folder, laid out
# exactly as they sit on the upm branch. Nothing is committed here.
#
# Usage: build-upm-payload.sh <payload-dir>
# Run from the repository root after a Release build.
set -euo pipefail

out="${1:?payload dir required}"
rm -rf "$out"
mkdir -p \
  "$out/Runtime/Plugins/Ethar.GeoPose" \
  "$out/Runtime/Plugins/Ethar.GeoPose.Authority" \
  "$out/Samples~/BasicSerialization" \
  "$out/Samples~/AuthorityImplementation" \
  "$out/Samples~/Tests/Editor/GeoPose" \
  "$out/Samples~/Tests/Editor/Authority"

# Unity consumes the net48 build. Copy only this project's outputs, not the copied dependencies.
cp Ethar.GeoPose/bin/Release/net48/Ethar.GeoPose.dll  "$out/Runtime/Plugins/Ethar.GeoPose/"
cp Ethar.GeoPose/bin/Release/net48/Ethar.GeoPose.pdb  "$out/Runtime/Plugins/Ethar.GeoPose/"
cp Ethar.GeoPose/bin/Release/net48/Ethar.GeoPose.xml  "$out/Runtime/Plugins/Ethar.GeoPose/"
cp Ethar.GeoPose.Authority/bin/Release/net48/Ethar.GeoPose.Authority.dll "$out/Runtime/Plugins/Ethar.GeoPose.Authority/"
cp Ethar.GeoPose.Authority/bin/Release/net48/Ethar.GeoPose.Authority.pdb "$out/Runtime/Plugins/Ethar.GeoPose.Authority/"
cp Ethar.GeoPose.Authority/bin/Release/net48/Ethar.GeoPose.Authority.xml "$out/Runtime/Plugins/Ethar.GeoPose.Authority/"

cp Ethar.GeoPose.Examples/Example_BasicSerialization.cs        "$out/Samples~/BasicSerialization/"
cp Ethar.GeoPose.Examples/Example_AuthorityImplementation.cs   "$out/Samples~/AuthorityImplementation/"
cp Ethar.GeoPose.UnitTests/*.cs            "$out/Samples~/Tests/Editor/GeoPose/"
cp Ethar.GeoPose.Authority.UnitTests/*.cs  "$out/Samples~/Tests/Editor/Authority/"

echo "UPM payload:"
find "$out" -type f | sort
