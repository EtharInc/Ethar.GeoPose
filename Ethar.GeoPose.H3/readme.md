# Ethar.GeoPose.H3

Converts between GeoPose positions and H3 cell indexes, and searches GeoPoses by cell, with no dependency other than Ethar.GeoPose.

H3 is Uber's hexagonal hierarchical geospatial index. It divides the Earth into hexagonal cells at 16 resolutions, from about 1,280 km edges at resolution 0 to about 0.5 m edges at resolution 15, and names each cell with a 64-bit integer written as 15 hexadecimal characters. This package is a C# translation of the cell indexing subset of the Uber H3 v4.5.0 reference implementation, verified against Uber's own fixture files, so it produces the same cell indexes, centres and boundaries as the C library.

The assembly targets net452 through net7.0 and netstandard2.0 and 2.1, uses no unsafe code and runs under Unity 2022.3 IL2CPP.

## The two conversions

```csharp
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.H3;

var position = new TangentPointPosition(48.8566, 2.3522, 35.5);

// GeoPose position to the cell containing it. Height is ignored, H3 is two dimensional.
H3Index cell = position.ToH3Cell(9);          // resolution 9, about 200 m edges
string text = cell.ToString();                // 15 character lowercase hex, culture invariant
H3Index parsed = H3Index.Parse("8928308280FFFFF"); // upper case accepted

// Cell to the GeoPose position of its centre. The height is required so the datum choice is explicit.
TangentPointPosition centre = cell.ToTangentPointPosition(35.5);

// A Basic GeoPose at the cell centre.
var pose = cell.ToBasicYawPitchRollSdu(35.5, new YawPitchRollAngles(0, 0, 0));
H3Index again = pose.ToH3Cell(9);

// The cell outline for drawing, 5 to 10 vertices.
H3CellBoundary outline = cell.ToCellBoundary(35.5);
```

Latitude and longitude outside the GeoPose range and resolutions outside 0 to 15 throw an `H3Exception` carrying the H3 error code.

## Levels and containment

A GeoPose is a single point, so a position is in exactly one cell at each resolution. The cell at resolution r is the ancestor of the cell at resolution 15, and moving between levels is a few bit operations.

```csharp
H3Index fine = position.ToH3Cell(15);
H3Index level3 = fine.Parent(3);              // the resolution 3 cell
H3Index[] levels = fine.Ancestors();          // levels[r] is the cell at resolution r, 0 to 15
H3Index[] all = position.ToH3Cells();         // the same, straight from a position

bool inside = position.IsInH3Cell(level3);    // true
bool under = fine.IsWithin(level3);           // true, by the index hierarchy
bool holds = level3.Contains(fine);           // true
```

There are two containment rules, and they are not the same.

- Direct projection: `position.ToH3Cell(r)`, `position.ToH3Cells()` and `IsInH3Cell` index the position at resolution r itself. This is the exact H3 cell for that resolution.
- Hierarchy: `Parent`, `Ancestors`, `Contains`, `IsWithin` and `H3CellRegistry` follow the index digits, which is how H3 itself defines parent and child.

H3 hexagons only approximately nest. The union of a cell's descendants is a jagged shape, not the hexagon, so a position near a cell edge can project directly into one cell at resolution r while its resolution 15 cell descends from a neighbour. Measured on Uber's 5,000 random fixture points, the two rules disagree for 6 to 7 percent of points at every resolution, whatever the gap between the resolutions. Pick one rule for an application and keep to it: direct projection when the exact cell at a level matters, the hierarchy when consistency across levels and fast search matter.

## Searching registered GeoPoses by cell

`H3CellRegistry<T>` holds items placed at GeoPose positions and answers "which items are inside this cell" at any resolution.

```csharp
var registry = new H3CellRegistry<BasicYawPitchRollSdu>();
foreach (var pose in poses)
{
    registry.Add(pose, pose.Position);
}

H3Index area = new TangentPointPosition(48.85, 2.35, 0).ToH3Cell(6);
List<BasicYawPitchRollSdu> nearby = registry.FindWithin(area);
int count = registry.CountWithin(area);
```

Every item is stored once, keyed by its resolution 15 cell. All descendants of a cell form one contiguous range of resolution 15 values, so a query is a binary search plus a scan of the matches: O(log n + matches) after an O(n log n) sort that runs on the first query after a change. The registry is not thread safe.

The registry uses the hierarchy rule throughout, so `FindWithin(position, r)` looks up the resolution 15 cell of the position and walks up to r; a registered position always finds itself. If an application needs the direct projection rule instead, bucket items in a `Dictionary<H3Index, List<T>>` keyed by `position.ToH3Cell(r)` for each level it queries.

## Guidance for adopters

- Store one `ulong`, the resolution 15 cell (`position.ToH3Cell(15).Value`), against each GeoPose. Every coarser level is derived from it by `Parent(r)` without touching the position again. This is the hierarchy rule; it is compact, consistent across levels and needs no maths after the first projection.
- If the exact cell at a given level matters more than consistency, project at that level with `position.ToH3Cell(r)`, or take all 16 at once with `position.ToH3Cells()`. Each projection costs a few microseconds. Storing 16 indexes per pose is the "superposition" option; it is only worth it when queries at many levels must follow the direct rule.
- To bucket poses at a fixed level, for example for a tile cache, key a dictionary by `fine.Parent(level)` or by `position.ToH3Cell(level)`, according to the rule chosen. To answer range queries at any level, use `H3CellRegistry<T>` or replicate its range trick: descendants of a cell lie between `cell.CenterChild(15)` and the same value with every digit below the cell's resolution set to 6.
- A pentagon cell (12 per resolution) has 6 children, not 7, and 5 vertices at even resolutions. The library handles this; only vertex counts and child counts are visible to callers.
- Projection costs a few microseconds per call in managed code. Index once when a pose is created or moves, not on every read. Bulk indexing belongs off the main thread in Unity.
- The engine class `H3` exposes the ported functions under their H3 names with radians, error codes and `out` parameters, for readers of the H3 documentation. The `H3Index` struct and the extension methods are the idiomatic surface and throw `H3Exception` instead.

## Resolution table

Average hexagon edge length, from `H3.GetHexagonEdgeLengthAvgM`.

| Resolution | Edge length | Resolution | Edge length |
| --- | --- | --- | --- |
| 0 | 1,281 km | 8 | 531 m |
| 1 | 483 km | 9 | 201 m |
| 2 | 183 km | 10 | 76 m |
| 3 | 69 km | 11 | 29 m |
| 4 | 26 km | 12 | 11 m |
| 5 | 9.9 km | 13 | 4.1 m |
| 6 | 3.7 km | 14 | 1.5 m |
| 7 | 1.4 km | 15 | 0.58 m |

## H3 cell frame specification

An Advanced GeoPose can name a cell as its frame. Register the authority once, then the frame serializes through the ordinary GeoPose JSON path.

```csharp
using Ethar.GeoPose.H3.Authority;

EtharGeoPoseH3Authority.Register();
var frame = new H3CellSpecification(cell, 35.5);
var advanced = new AdvancedSdu(validTime, quaternion, frame);
```

```json
{"authority":"/Ethar.GeoPose.H3/1.0","id":"H3-CELL","parameters":"cell=8928308280fffff&heightInMeters=35.5"}
```

The frame origin is the cell centre at the given height with east-north-up axes, the same as the Ethar LTP-ENU frame at that point. `H3CellSpecification.Position` resolves it. The authority defines this frame only; transition models come from the Ethar or OGC authority.

## What is ported

Index inspection, position to cell, cell to centre, cell to boundary, parent, children, centre child and the resolution metrics from H3 v4.5.0. Grid traversal (gridDisk, gridRing), polygon fill, edges, vertices, compaction and local IJ coordinates are not included. The lookup tables are generated from the Uber sources by a script kept in the test project so the port can track new H3 releases.

## Attribution

This package includes software developed by Uber Technologies, Inc. (<https://github.com/uber/h3>), licensed under the Apache License, Version 2.0, and ported to C# by Ethar Inc. Every ported file names its source file and Uber's copyright. See the NOTICE file for details.
