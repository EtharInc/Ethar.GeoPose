# H3 fixtures

These files are copied unchanged from the Uber H3 repository, tag v4.5.0, folder `tests/inputfiles` (https://github.com/uber/h3). They are licensed under the Apache License, Version 2.0, copyright Uber Technologies, Inc. See the NOTICE file in the Ethar.GeoPose.H3 project.

Refresh them by copying the same files from a newer tag and updating the version here.

## Files

| File | Format | Content |
| --- | --- | --- |
| res00cells.txt, res01cells.txt, res02cells.txt | Index on one line, then `{`, one `lat lng` line per boundary vertex in degrees, then `}` | Every cell at resolutions 0, 1 and 2 with its boundary |
| res00ic.txt to res04ic.txt | `index lat lng` per line, degrees | Every cell at resolutions 0 to 4 with its centre, printed to ten decimals |
| rand05centers.txt to rand15centers.txt | `index lat lng` per line, degrees, longitude may exceed 180 | 5,000 random points per resolution, each with the cell that contains it. The point is not the cell centre. |
| bc05r08centers.txt to bc05r15centers.txt | `index lat lng` per line, degrees | Cells under hexagon base cell 5 at resolutions 8 to 15 |
| bc14r08centers.txt to bc14r15centers.txt | `index lat lng` per line, degrees | Cells under pentagon base cell 14 at resolutions 8 to 15 |

Every index is the 15 character lowercase hex form written by H3's `h3ToString`.
