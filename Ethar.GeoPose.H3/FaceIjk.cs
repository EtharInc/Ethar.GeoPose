// <copyright file="FaceIjk.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/faceijk.c and src/h3lib/include/faceijk.h. Copyright 2016-2023, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using System;
    using System.Globalization;
    using Ethar.GeoPose.H3.Tables;

    /// <summary>
    /// Face number and ijk coordinates on that face-centred coordinate system, with the icosahedron projection functions of faceijk.c. Ported from the C struct <c>FaceIJK</c>.
    /// </summary>
    /// <remarks>
    /// The struct is mutable with public fields so that the ported maths can update the coordinate in place, as the C code does through a pointer.
    /// </remarks>
    internal struct FaceIjk
    {
        /// <summary>
        /// IJ quadrant faceNeighbors table direction. C name <c>IJ</c>.
        /// </summary>
        public const int IJ = 1;

        /// <summary>
        /// KI quadrant faceNeighbors table direction. C name <c>KI</c>.
        /// </summary>
        public const int KI = 2;

        /// <summary>
        /// JK quadrant faceNeighbors table direction. C name <c>JK</c>.
        /// </summary>
        public const int JK = 3;

        /// <summary>
        /// Invalid face index. C name <c>INVALID_FACE</c>.
        /// </summary>
        public const int InvalidFace = -1;

#pragma warning disable SA1401 // Fields should be private. Public fields mirror the C struct so that Coord can be passed by ref.
        /// <summary>
        /// The face number.
        /// </summary>
        public int Face;

        /// <summary>
        /// The ijk coordinates on that face.
        /// </summary>
        public CoordIjk Coord;
#pragma warning restore SA1401

        /// <summary>
        /// The vertexes of an origin-centered cell in a Class II resolution on a substrate grid with aperture sequence 33r. The aperture 3 gets us the vertices, and the 3r gets us back to Class II. Vertices listed ccw from the i-axes. C name <c>vertsCII</c> in <c>_faceIjkToVerts</c>.
        /// </summary>
        private static readonly CoordIjk[] VertsCII =
        {
            new CoordIjk(2, 1, 0),
            new CoordIjk(1, 2, 0),
            new CoordIjk(0, 2, 1),
            new CoordIjk(0, 1, 2),
            new CoordIjk(1, 0, 2),
            new CoordIjk(2, 0, 1),
        };

        /// <summary>
        /// The vertexes of an origin-centered cell in a Class III resolution on a substrate grid with aperture sequence 33r7r. The aperture 3 gets us the vertices, and the 3r7r gets us to Class II. Vertices listed ccw from the i-axes. C name <c>vertsCIII</c> in <c>_faceIjkToVerts</c>.
        /// </summary>
        private static readonly CoordIjk[] VertsCIII =
        {
            new CoordIjk(5, 4, 0),
            new CoordIjk(1, 5, 0),
            new CoordIjk(0, 5, 4),
            new CoordIjk(0, 1, 5),
            new CoordIjk(4, 0, 5),
            new CoordIjk(5, 0, 1),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceIjk"/> struct.
        /// </summary>
        /// <param name="face">The face number.</param>
        /// <param name="coord">The ijk coordinates on that face.</param>
        public FaceIjk(int face, CoordIjk coord)
        {
            this.Face = face;
            this.Coord = coord;
        }

        /// <summary>
        /// Encodes a unit sphere coordinate to the FaceIJK address of the containing cell at the specified resolution. C name <c>_vec3ToFaceIjk</c>.
        /// </summary>
        /// <param name="p">The coordinates to encode, on the unit sphere.</param>
        /// <param name="res">The desired H3 resolution for the encoding.</param>
        /// <returns>The FaceIJK address of the containing cell at resolution res.</returns>
        public static FaceIjk Vec3ToFaceIjk(Vec3d p, int res)
        {
            // first convert to hex2d
            Vec3ToHex2d(p, res, out var face, out var v);

            // then convert to ijk+
            return new FaceIjk(face, CoordIjk.Hex2dToCoordIjk(v));
        }

        /// <summary>
        /// Determines the center point in 3D coordinates of a cell given by a FaceIJK address at a specified resolution. C name <c>_faceIjkToVec3</c>.
        /// </summary>
        /// <param name="h">The FaceIJK address of the cell.</param>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <returns>The 3D coordinates of the cell center point.</returns>
        public static Vec3d FaceIjkToVec3(FaceIjk h, int res)
        {
            var v = CoordIjk.IjkToHex2d(h.Coord);
            return Hex2dToVec3(v, h.Face, res, false);
        }

        /// <summary>
        /// Generates the cell boundary in spherical coordinates for a pentagonal cell given by a FaceIJK address at a specified resolution. C name <c>_faceIjkPentToCellBoundary</c>.
        /// </summary>
        /// <param name="h">The FaceIJK address of the pentagonal cell.</param>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <param name="start">The first topological vertex to return.</param>
        /// <param name="length">The number of topological vertexes to return.</param>
        /// <param name="g">Output: the spherical coordinates of the cell boundary.</param>
        public static void FaceIjkPentToCellBoundary(FaceIjk h, int res, int start, int length, CellBoundary g)
        {
            var adjRes = res;
            var centerIJK = h;
            var fijkVerts = new FaceIjk[Constants.NUM_PENT_VERTS];
            FaceIjkPentToVerts(ref centerIJK, ref adjRes, fijkVerts);

            // If we're returning the entire loop, we need one more iteration in case of a distortion vertex on the last edge
            var additionalIteration = length == Constants.NUM_PENT_VERTS ? 1 : 0;

            // convert each vertex to lat/lng
            // adjust the face of each vertex as appropriate and introduce edge-crossing vertices as needed
            g.Clear();
            var lastFijk = default(FaceIjk);
            for (var vert = start; vert < start + length + additionalIteration; vert++)
            {
                var v = vert % Constants.NUM_PENT_VERTS;

                var fijk = fijkVerts[v];

                AdjustPentVertOverage(ref fijk, adjRes);

                // all Class III pentagon edges cross icosa edges
                // note that Class II pentagons have vertices on the edge, not edge intersections
                if (H3.IsResolutionClassIII(res) && vert > start)
                {
                    // find hex2d of the two vertexes on the last face
                    var tmpFijk = fijk;

                    var orig2d0 = CoordIjk.IjkToHex2d(lastFijk.Coord);

                    var currentToLastDir = FaceIjkTables.adjacentFaceDir[tmpFijk.Face, lastFijk.Face];

                    var fijkOrient = FaceIjkTables.faceNeighbors[tmpFijk.Face, currentToLastDir];

                    tmpFijk.Face = fijkOrient.Face;

                    // rotate and translate for adjacent face
                    for (var i = 0; i < fijkOrient.CcwRot60; i++)
                    {
                        CoordIjk.IjkRotate60ccw(ref tmpFijk.Coord);
                    }

                    var transVec = fijkOrient.Translate;
                    CoordIjk.IjkScale(ref transVec, FaceIjkTables.unitScaleByCIIres[adjRes] * 3);
                    tmpFijk.Coord = CoordIjk.IjkAdd(tmpFijk.Coord, transVec);
                    CoordIjk.IjkNormalize(ref tmpFijk.Coord);

                    var orig2d1 = CoordIjk.IjkToHex2d(tmpFijk.Coord);

                    // find the appropriate icosa face edge vertexes
                    var maxDim = FaceIjkTables.maxDimByCIIres[adjRes];
                    var v0 = new Vec2d(3.0 * maxDim, 0.0);
                    var v1 = new Vec2d(-1.5 * maxDim, 3.0 * Constants.M_SQRT3_2 * maxDim);
                    var v2 = new Vec2d(-1.5 * maxDim, -3.0 * Constants.M_SQRT3_2 * maxDim);

                    Vec2d edge0;
                    Vec2d edge1;
                    switch (FaceIjkTables.adjacentFaceDir[tmpFijk.Face, fijk.Face])
                    {
                        case IJ:
                            edge0 = v0;
                            edge1 = v1;
                            break;
                        case JK:
                            edge0 = v1;
                            edge1 = v2;
                            break;
                        default:
                            // case KI
                            edge0 = v2;
                            edge1 = v0;
                            break;
                    }

                    // find the intersection and add the lat/lng point to the result
                    var inter = Vec2d.V2dIntersect(orig2d0, orig2d1, edge0, edge1);
                    var v3 = Hex2dToVec3(inter, tmpFijk.Face, adjRes, true);
                    g.Add(Vec3d.Vec3ToLatLng(v3));
                }

                // convert vertex to lat/lng and add to the result
                // vert == start + NUM_PENT_VERTS is only used to test for possible intersection on last edge
                if (vert < start + Constants.NUM_PENT_VERTS)
                {
                    var vec = CoordIjk.IjkToHex2d(fijk.Coord);
                    var v3 = Hex2dToVec3(vec, fijk.Face, adjRes, true);
                    g.Add(Vec3d.Vec3ToLatLng(v3));
                }

                lastFijk = fijk;
            }
        }

        /// <summary>
        /// Get the vertices of a pentagon cell as substrate FaceIJK addresses. C name <c>_faceIjkPentToVerts</c>.
        /// </summary>
        /// <param name="fijk">The FaceIJK address of the cell, moved into the substrate grid.</param>
        /// <param name="res">In/out: the H3 resolution of the cell, adjusted for substrate.</param>
        /// <param name="fijkVerts">Output: array of at least 5 elements for the vertices.</param>
        public static void FaceIjkPentToVerts(ref FaceIjk fijk, ref int res, FaceIjk[] fijkVerts)
        {
            ToVerts(ref fijk, ref res, fijkVerts, Constants.NUM_PENT_VERTS);
        }

        /// <summary>
        /// Generates the cell boundary in spherical coordinates for a cell given by a FaceIJK address at a specified resolution. C name <c>_faceIjkToCellBoundary</c>.
        /// </summary>
        /// <param name="h">The FaceIJK address of the cell.</param>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <param name="start">The first topological vertex to return.</param>
        /// <param name="length">The number of topological vertexes to return.</param>
        /// <param name="g">Output: the spherical coordinates of the cell boundary.</param>
        public static void FaceIjkToCellBoundary(FaceIjk h, int res, int start, int length, CellBoundary g)
        {
            var adjRes = res;
            var centerIJK = h;
            var fijkVerts = new FaceIjk[Constants.NUM_HEX_VERTS];
            FaceIjkToVerts(ref centerIJK, ref adjRes, fijkVerts);

            // If we're returning the entire loop, we need one more iteration in case of a distortion vertex on the last edge
            var additionalIteration = length == Constants.NUM_HEX_VERTS ? 1 : 0;

            // convert each vertex to lat/lng
            // adjust the face of each vertex as appropriate and introduce edge-crossing vertices as needed
            g.Clear();
            var lastFace = -1;
            var lastOverage = Overage.NoOverage;
            for (var vert = start; vert < start + length + additionalIteration; vert++)
            {
                var v = vert % Constants.NUM_HEX_VERTS;

                var fijk = fijkVerts[v];

                const bool pentLeading4 = false;
                var overage = AdjustOverageClassII(ref fijk, adjRes, pentLeading4, true);

                // Check for edge-crossing. Each face of the underlying icosahedron is a different projection plane.
                // So if an edge of the hexagon crosses an icosahedron edge, an additional vertex must be introduced at that intersection point.
                // Then each half of the cell edge can be projected to geographic coordinates using the appropriate icosahedron face projection.
                // Note that Class II cell edges have vertices on the face edge, with no edge line intersections.
                if (H3.IsResolutionClassIII(res) && vert > start && fijk.Face != lastFace && lastOverage != Overage.FaceEdge)
                {
                    // find hex2d of the two vertexes on original face
                    var lastV = (v + 5) % Constants.NUM_HEX_VERTS;
                    var orig2d0 = CoordIjk.IjkToHex2d(fijkVerts[lastV].Coord);
                    var orig2d1 = CoordIjk.IjkToHex2d(fijkVerts[v].Coord);

                    // find the appropriate icosa face edge vertexes
                    var maxDim = FaceIjkTables.maxDimByCIIres[adjRes];
                    var v0 = new Vec2d(3.0 * maxDim, 0.0);
                    var v1 = new Vec2d(-1.5 * maxDim, 3.0 * Constants.M_SQRT3_2 * maxDim);
                    var v2 = new Vec2d(-1.5 * maxDim, -3.0 * Constants.M_SQRT3_2 * maxDim);

                    var face2 = lastFace == centerIJK.Face ? fijk.Face : lastFace;
                    Vec2d edge0;
                    Vec2d edge1;
                    switch (FaceIjkTables.adjacentFaceDir[centerIJK.Face, face2])
                    {
                        case IJ:
                            edge0 = v0;
                            edge1 = v1;
                            break;
                        case JK:
                            edge0 = v1;
                            edge1 = v2;
                            break;
                        default:
                            // case KI
                            edge0 = v2;
                            edge1 = v0;
                            break;
                    }

                    // find the intersection and add the lat/lng point to the result
                    var inter = Vec2d.V2dIntersect(orig2d0, orig2d1, edge0, edge1);

                    // If a point of intersection occurs at a hexagon vertex, then each adjacent hexagon edge will lie completely on a single icosahedron face, and no additional vertex is required.
                    var isIntersectionAtVertex = Vec2d.V2dAlmostEquals(orig2d0, inter) || Vec2d.V2dAlmostEquals(orig2d1, inter);
                    if (!isIntersectionAtVertex)
                    {
                        var v3 = Hex2dToVec3(inter, centerIJK.Face, adjRes, true);
                        g.Add(Vec3d.Vec3ToLatLng(v3));
                    }
                }

                // convert vertex to lat/lng and add to the result
                // vert == start + NUM_HEX_VERTS is only used to test for possible intersection on last edge
                if (vert < start + Constants.NUM_HEX_VERTS)
                {
                    var vec = CoordIjk.IjkToHex2d(fijk.Coord);
                    var v3 = Hex2dToVec3(vec, fijk.Face, adjRes, true);
                    g.Add(Vec3d.Vec3ToLatLng(v3));
                }

                lastFace = fijk.Face;
                lastOverage = overage;
            }
        }

        /// <summary>
        /// Get the vertices of a cell as substrate FaceIJK addresses. C name <c>_faceIjkToVerts</c>.
        /// </summary>
        /// <param name="fijk">The FaceIJK address of the cell, moved into the substrate grid.</param>
        /// <param name="res">In/out: the H3 resolution of the cell, adjusted for substrate.</param>
        /// <param name="fijkVerts">Output: array of at least 6 elements for the vertices.</param>
        public static void FaceIjkToVerts(ref FaceIjk fijk, ref int res, FaceIjk[] fijkVerts)
        {
            ToVerts(ref fijk, ref res, fijkVerts, Constants.NUM_HEX_VERTS);
        }

        /// <summary>
        /// Adjusts a FaceIJK address in place so that the resulting cell address is relative to the correct icosahedral face. C name <c>_adjustOverageClassII</c>.
        /// </summary>
        /// <param name="fijk">The FaceIJK address of the cell.</param>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <param name="pentLeading4">Whether or not the cell is a pentagon with a leading digit 4.</param>
        /// <param name="substrate">Whether or not the cell is in a substrate grid.</param>
        /// <returns><see cref="Overage.NoOverage"/> if on the original face, <see cref="Overage.FaceEdge"/> if on a face edge (only occurs on substrate grids), or <see cref="Overage.NewFace"/> if overage on a new face interior.</returns>
        public static Overage AdjustOverageClassII(ref FaceIjk fijk, int res, bool pentLeading4, bool substrate)
        {
            var overage = Overage.NoOverage;

            // get the maximum dimension value; scale if a substrate grid
            var maxDim = FaceIjkTables.maxDimByCIIres[res];
            if (substrate)
            {
                maxDim *= 3;
            }

            // check for overage
            if (substrate && fijk.Coord.I + fijk.Coord.J + fijk.Coord.K == maxDim)
            {
                // on edge
                overage = Overage.FaceEdge;
            }
            else if (fijk.Coord.I + fijk.Coord.J + fijk.Coord.K > maxDim)
            {
                // overage
                overage = Overage.NewFace;

                FaceOrientIjk fijkOrient;
                if (fijk.Coord.K > 0)
                {
                    if (fijk.Coord.J > 0)
                    {
                        // jk "quadrant"
                        fijkOrient = FaceIjkTables.faceNeighbors[fijk.Face, JK];
                    }
                    else
                    {
                        // ik "quadrant"
                        fijkOrient = FaceIjkTables.faceNeighbors[fijk.Face, KI];

                        // adjust for the pentagonal missing sequence
                        if (pentLeading4)
                        {
                            // translate origin to center of pentagon
                            var origin = new CoordIjk(maxDim, 0, 0);
                            var tmp = CoordIjk.IjkSub(fijk.Coord, origin);

                            // rotate to adjust for the missing sequence
                            CoordIjk.IjkRotate60cw(ref tmp);

                            // translate the origin back to the center of the triangle
                            fijk.Coord = CoordIjk.IjkAdd(tmp, origin);
                        }
                    }
                }
                else
                {
                    // ij "quadrant"
                    fijkOrient = FaceIjkTables.faceNeighbors[fijk.Face, IJ];
                }

                fijk.Face = fijkOrient.Face;

                // rotate and translate for adjacent face
                for (var i = 0; i < fijkOrient.CcwRot60; i++)
                {
                    CoordIjk.IjkRotate60ccw(ref fijk.Coord);
                }

                var transVec = fijkOrient.Translate;
                var unitScale = FaceIjkTables.unitScaleByCIIres[res];
                if (substrate)
                {
                    unitScale *= 3;
                }

                CoordIjk.IjkScale(ref transVec, unitScale);
                fijk.Coord = CoordIjk.IjkAdd(fijk.Coord, transVec);
                CoordIjk.IjkNormalize(ref fijk.Coord);

                // overage points on pentagon boundaries can end up on edges
                if (substrate && fijk.Coord.I + fijk.Coord.J + fijk.Coord.K == maxDim)
                {
                    // on edge
                    overage = Overage.FaceEdge;
                }
            }

            return overage;
        }

        /// <summary>
        /// Adjusts a FaceIJK address for a pentagon vertex in a substrate grid in place so that the resulting cell address is relative to the correct icosahedral face. C name <c>_adjustPentVertOverage</c>.
        /// </summary>
        /// <param name="fijk">The FaceIJK address of the cell.</param>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <returns>The final overage state.</returns>
        public static Overage AdjustPentVertOverage(ref FaceIjk fijk, int res)
        {
            const bool pentLeading4 = false;
            Overage overage;
            do
            {
                overage = AdjustOverageClassII(ref fijk, res, pentLeading4, true);
            }
            while (overage == Overage.NewFace);

            return overage;
        }

        /// <summary>
        /// Determines the 3D coordinates of a cell given by 2D hex coordinates on a particular icosahedral face. C name <c>_hex2dToVec3</c>.
        /// </summary>
        /// <param name="v">The 2D hex coordinates of the cell.</param>
        /// <param name="face">The icosahedral face upon which the 2D hex coordinate system is centered.</param>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <param name="substrate">Indicates whether or not this grid is actually a substrate grid relative to the specified resolution.</param>
        /// <returns>The 3D coordinates of the cell center point.</returns>
        public static Vec3d Hex2dToVec3(Vec2d v, int face, int res, bool substrate)
        {
            // calculate (r, theta) in hex2d
            var r = Vec2d.V2dMag(v);

            if (r < Constants.EPSILON)
            {
                return FaceIjkTables.faceCenterPoint[face];
            }

            var theta = Math.Atan2(v.Y, v.X);

            // scale for current resolution length u
            for (var i = 0; i < res; i++)
            {
                r *= FaceIjkTables.M_RSQRT7;
            }

            // scale accordingly if this is a substrate grid
            if (substrate)
            {
                r *= Constants.M_ONETHIRD;
                if (H3.IsResolutionClassIII(res))
                {
                    r *= FaceIjkTables.M_RSQRT7;
                }
            }

            r *= Constants.RES0_U_GNOMONIC;

            // perform inverse gnomonic scaling of r
            r = Math.Atan(r);

            // adjust theta for Class III
            // if a substrate grid, then it's already been adjusted for Class III
            if (!substrate && H3.IsResolutionClassIII(res))
            {
                theta = LatLng.PosAngleRads(theta + Constants.M_AP7_ROT_RADS);
            }

            // find theta as an azimuth
            theta = LatLng.PosAngleRads(FaceIjkTables.faceAxesAzRadsCII[face, 0] - theta);

            // now find the point at (r,theta) from the face center
            Vec3TangentBasis(FaceIjkTables.faceCenterPoint[face], out var northDir, out var eastDir);
            var dir = Vec3d.Vec3LinComb(Math.Cos(theta), northDir, Math.Sin(theta), eastDir);
            var v3 = Vec3d.Vec3LinComb(Math.Cos(r), FaceIjkTables.faceCenterPoint[face], Math.Sin(r), dir);
            Vec3d.Vec3Normalize(ref v3);
            return v3;
        }

        /// <summary>
        /// Encodes a coordinate on the sphere to the corresponding icosahedral face and containing 2D hex coordinates relative to that face center. C name <c>_vec3ToHex2d</c>.
        /// </summary>
        /// <param name="p">The coordinates to encode, on the unit sphere.</param>
        /// <param name="res">The desired H3 resolution for the encoding.</param>
        /// <param name="face">Output: the icosahedral face containing the coordinates.</param>
        /// <param name="v">Output: the 2D hex coordinates of the cell containing the point.</param>
        public static void Vec3ToHex2d(Vec3d p, int res, out int face, out Vec2d v)
        {
            // determine the icosahedron face
            Vec3ToClosestFace(p, out face, out var sqd);

            // cos(r) = 1 - 2 * sin^2(r/2) = 1 - 2 * (sqd / 4) = 1 - sqd/2
            var r = Math.Acos(1 - (sqd * 0.5));

            if (r < Constants.EPSILON)
            {
                v = new Vec2d(0.0, 0.0);
                return;
            }

            // now have face and r, now find CCW theta from CII i-axis
            var theta = LatLng.PosAngleRads(FaceIjkTables.faceAxesAzRadsCII[face, 0] - LatLng.PosAngleRads(Vec3AzimuthRads(FaceIjkTables.faceCenterPoint[face], p)));

            // adjust theta for Class III (odd resolutions)
            if (H3.IsResolutionClassIII(res))
            {
                theta = LatLng.PosAngleRads(theta - Constants.M_AP7_ROT_RADS);
            }

            // perform gnomonic scaling of r
            r = Math.Tan(r);

            // scale for current resolution length u
            r *= Constants.INV_RES0_U_GNOMONIC;
            for (var i = 0; i < res; i++)
            {
                r *= FaceIjkTables.M_SQRT7;
            }

            // we now have (r, theta) in hex2d with theta ccw from x-axes
            // convert to local x,y
            v = new Vec2d(r * Math.Cos(theta), r * Math.Sin(theta));
        }

        /// <summary>
        /// Compute the local north and east directions on the tangent plane at a point on the unit sphere. Will not work if p is at a pole, but icosahedron face centers are never at the poles. C name <c>_vec3TangentBasis</c>.
        /// </summary>
        /// <param name="p">Unit vector on the sphere.</param>
        /// <param name="north">Output: local north direction on the tangent plane.</param>
        /// <param name="east">Output: local east direction on the tangent plane.</param>
        public static void Vec3TangentBasis(Vec3d p, out Vec3d north, out Vec3d east)
        {
            var northPole = new Vec3d(0.0, 0.0, 1.0);
            north = Vec3d.Vec3LinComb(1.0, northPole, -Vec3d.Vec3Dot(northPole, p), p);
            Vec3d.Vec3Normalize(ref north);
            east = Vec3d.Vec3Cross(north, p);
        }

        /// <summary>
        /// Calculates the azimuth from p1 to p2. C name <c>_vec3AzimuthRads</c>.
        /// </summary>
        /// <param name="p1">The first vector.</param>
        /// <param name="p2">The second vector.</param>
        /// <returns>The azimuth in radians.</returns>
        public static double Vec3AzimuthRads(Vec3d p1, Vec3d p2)
        {
            Vec3TangentBasis(p1, out var northDir, out var eastDir);

            // project p2 onto tangent plane at p1
            var p2Proj = Vec3d.Vec3LinComb(1.0, p2, -Vec3d.Vec3Dot(p2, p1), p1);
            Vec3d.Vec3Normalize(ref p2Proj);

            return Math.Atan2(Vec3d.Vec3Dot(p2Proj, eastDir), Vec3d.Vec3Dot(p2Proj, northDir));
        }

        /// <summary>
        /// Encodes a coordinate on the sphere to the corresponding icosahedral face and the squared euclidean distance to that face center. C name <c>_vec3ToClosestFace</c>.
        /// </summary>
        /// <param name="v">The coordinates to encode, on the unit sphere.</param>
        /// <param name="face">Output: the icosahedral face containing the coordinates.</param>
        /// <param name="sqd">Output: the squared euclidean distance to its face center.</param>
        public static void Vec3ToClosestFace(Vec3d v, out int face, out double sqd)
        {
            face = 0;

            // The distance between two farthest points is 2.0, therefore the square of the distance between two points should always be less or equal than 4.0.
            sqd = 5.0;
            for (var f = 0; f < Constants.NUM_ICOSA_FACES; ++f)
            {
                var sqdT = Vec3d.Vec3DistSq(FaceIjkTables.faceCenterPoint[f], v);
                if (sqdT < sqd)
                {
                    face = f;
                    sqd = sqdT;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "face {0} {1}", this.Face, this.Coord);
        }

        /// <summary>
        /// Shared body of <c>_faceIjkToVerts</c> and <c>_faceIjkPentToVerts</c>, which differ only in the number of vertices.
        /// </summary>
        /// <param name="fijk">The FaceIJK address of the cell, moved into the substrate grid.</param>
        /// <param name="res">In/out: the H3 resolution of the cell, adjusted for substrate.</param>
        /// <param name="fijkVerts">Output: array for the vertices.</param>
        /// <param name="numVerts">6 for a hexagon, 5 for a pentagon.</param>
        private static void ToVerts(ref FaceIjk fijk, ref int res, FaceIjk[] fijkVerts, int numVerts)
        {
            // get the correct set of substrate vertices for this resolution
            var verts = H3.IsResolutionClassIII(res) ? VertsCIII : VertsCII;

            // adjust the center point to be in an aperture 33r substrate grid
            // these should be composed for speed
            CoordIjk.DownAp3(ref fijk.Coord);
            CoordIjk.DownAp3r(ref fijk.Coord);

            // if res is Class III we need to add a cw aperture 7 to get to icosahedral Class II
            if (H3.IsResolutionClassIII(res))
            {
                CoordIjk.DownAp7r(ref fijk.Coord);
                res += 1;
            }

            // The center point is now in the same substrate grid as the origin cell vertices.
            // Add the center point substrate coordinates to each vertex to translate the vertices to that cell.
            for (var v = 0; v < numVerts; v++)
            {
                var coord = CoordIjk.IjkAdd(fijk.Coord, verts[v]);
                CoordIjk.IjkNormalize(ref coord);
                fijkVerts[v] = new FaceIjk(fijk.Face, coord);
            }
        }
    }
}
