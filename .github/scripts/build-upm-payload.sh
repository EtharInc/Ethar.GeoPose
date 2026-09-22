#!/usr/bin/env bash
set -euo pipefail

out="${1:?payload dir required}"
rm -rf "$out"
mkdir -p \
  "$out/Runtime/Plugins/Ethar.GeoPose" \
  "$out/Runtime/Plugins/Ethar.GeoPose.Authority" \
  "$out/Runtime/Plugins/Ethar.GeoPose.H3" \
  "$out/Samples~/BasicSerialization" \
  "$out/Samples~/AuthorityImplementation" \
  "$out/Samples~/Tests/Editor/GeoPose" \
  "$out/Samples~/Tests/Editor/Authority" \
  "$out/Samples~/Tests/Editor/H3/Fixtures/H3"

cp Ethar.GeoPose/bin/Release/net48/Ethar.GeoPose.dll  "$out/Runtime/Plugins/Ethar.GeoPose/"
cp Ethar.GeoPose/bin/Release/net48/Ethar.GeoPose.pdb  "$out/Runtime/Plugins/Ethar.GeoPose/"
cp Ethar.GeoPose/bin/Release/net48/Ethar.GeoPose.xml  "$out/Runtime/Plugins/Ethar.GeoPose/"
cp Ethar.GeoPose.Authority/bin/Release/net48/Ethar.GeoPose.Authority.dll "$out/Runtime/Plugins/Ethar.GeoPose.Authority/"
cp Ethar.GeoPose.Authority/bin/Release/net48/Ethar.GeoPose.Authority.pdb "$out/Runtime/Plugins/Ethar.GeoPose.Authority/"
cp Ethar.GeoPose.Authority/bin/Release/net48/Ethar.GeoPose.Authority.xml "$out/Runtime/Plugins/Ethar.GeoPose.Authority/"

# Ethar.GeoPose.H3 depends on Ethar.GeoPose only. LICENSE and NOTICE ship with the plugin because the assembly contains code ported from Uber H3 under Apache-2.0.
cp Ethar.GeoPose.H3/bin/Release/net48/Ethar.GeoPose.H3.dll "$out/Runtime/Plugins/Ethar.GeoPose.H3/"
cp Ethar.GeoPose.H3/bin/Release/net48/Ethar.GeoPose.H3.pdb "$out/Runtime/Plugins/Ethar.GeoPose.H3/"
cp Ethar.GeoPose.H3/bin/Release/net48/Ethar.GeoPose.H3.xml "$out/Runtime/Plugins/Ethar.GeoPose.H3/"
cp Ethar.GeoPose.H3/LICENSE "$out/Runtime/Plugins/Ethar.GeoPose.H3/"
cp Ethar.GeoPose.H3/NOTICE  "$out/Runtime/Plugins/Ethar.GeoPose.H3/"

cp Ethar.GeoPose.Examples/Example_BasicSerialization.cs        "$out/Samples~/BasicSerialization/"
cp Ethar.GeoPose.Examples/Example_AuthorityImplementation.cs   "$out/Samples~/AuthorityImplementation/"
cp Ethar.GeoPose.UnitTests/*.cs            "$out/Samples~/Tests/Editor/GeoPose/"
cp Ethar.GeoPose.Authority.UnitTests/*.cs  "$out/Samples~/Tests/Editor/Authority/"

# The H3 tests read the Uber fixture files relative to the test directory, so the fixtures travel with them.
cp Ethar.GeoPose.H3.UnitTests/*.cs                "$out/Samples~/Tests/Editor/H3/"
cp Ethar.GeoPose.H3.UnitTests/Fixtures/H3/*.txt   "$out/Samples~/Tests/Editor/H3/Fixtures/H3/"
cp Ethar.GeoPose.H3.UnitTests/Fixtures/H3/readme.md "$out/Samples~/Tests/Editor/H3/Fixtures/H3/"

echo "UPM payload:"
find "$out" -type f | sort
