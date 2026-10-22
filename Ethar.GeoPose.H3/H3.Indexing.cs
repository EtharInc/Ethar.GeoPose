// <copyright file="H3.Indexing.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/h3Index.c. Copyright 2016-2021, 2024, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using Ethar.GeoPose.H3.Tables;

    /// <summary>
    /// Position to cell, cell to position and cell to boundary, from h3Index.c.
    /// </summary>
    public static partial class H3
    {
        /// <summary>
        /// Encodes a coordinate on the sphere to the H3 index of the containing cell at the specified resolution. C name <c>latLngToCell</c>.
        /// </summary>
        /// <param name="g">The spherical coordinates to encode, in radians.</param>
        /// <param name="res">The desired H3 resolution for the encoding.</param>
        /// <param name="output">The encoded H3 index, or <see cref="H3Null"/> on failure.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15, <see cref="H3ErrorCode.LatLngDomain"/> if a coordinate is not finite, or <see cref="H3ErrorCode.Failed"/> if no cell was found.</returns>
        public static H3ErrorCode LatLngToCell(LatLng g, int res, out ulong output)
        {
            output = H3Null;
            if (res < 0 || res > Constants.MAX_H3_RES)
            {
                return H3ErrorCode.ResDomain;
            }

            if (!IsFinite(g.Lat) || !IsFinite(g.Lng))
            {
                return H3ErrorCode.LatLngDomain;
            }

            var v = Vec3d.LatLngToVec3(g);
            return Vec3ToCell(v, res, out output);
        }

        /// <summary>
        /// Determines the spherical coordinates of the center point of an H3 index. C name <c>cellToLatLng</c>.
        /// </summary>
        /// <param name="h3">The H3 index.</param>
        /// <param name="g">The spherical coordinates of the H3 cell center, in radians.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.CellInvalid"/> if the base cell is out of range.</returns>
        public static H3ErrorCode CellToLatLng(ulong h3, out LatLng g)
        {
            var e = CellToVec3(h3, out var v);
            if (e != H3ErrorCode.Success)
            {
                g = default;
                return e;
            }

            g = Vec3d.Vec3ToLatLng(v);
            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Determines the cell boundary in spherical coordinates for an H3 index. C name <c>cellToBoundary</c>.
        /// </summary>
        /// <param name="h3">The H3 index.</param>
        /// <param name="cb">The boundary of the H3 cell in spherical coordinates, or null on failure.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.CellInvalid"/> if the base cell is out of range.</returns>
        public static H3ErrorCode CellToBoundary(ulong h3, out CellBoundary cb)
        {
            var e = H3ToFaceIjk(h3, out var fijk);
            if (e != H3ErrorCode.Success)
            {
                cb = null;
                return e;
            }

            cb = new CellBoundary();
            if (IsPentagon(h3))
            {
                FaceIjk.FaceIjkPentToCellBoundary(fijk, GetResolution(h3), 0, Constants.NUM_PENT_VERTS, cb);
            }
            else
            {
                FaceIjk.FaceIjkToCellBoundary(fijk, GetResolution(h3), 0, Constants.NUM_HEX_VERTS, cb);
            }

            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Convert a FaceIJK address to the corresponding H3 index. C name <c>_faceIjkToH3</c>.
        /// </summary>
        /// <param name="fijk">The FaceIJK address.</param>
        /// <param name="res">The cell resolution.</param>
        /// <returns>The encoded H3 index, or <see cref="H3Null"/> on failure.</returns>
        internal static ulong FaceIjkToH3(FaceIjk fijk, int res)
        {
            // initialize the index
            var h = H3Init;
            h = SetMode(h, Constants.H3_CELL_MODE);
            h = SetResolution(h, res);

            // check for res 0/base cell
            if (res == 0)
            {
                if (fijk.Coord.I > BaseCells.MaxFaceCoord || fijk.Coord.J > BaseCells.MaxFaceCoord || fijk.Coord.K > BaseCells.MaxFaceCoord)
                {
                    // out of range input
                    return H3Null;
                }

                h = SetBaseCell(h, BaseCells.FaceIjkToBaseCell(fijk));
                return h;
            }

            // we need to find the correct base cell FaceIJK for this H3 index;
            // start with the passed in face and resolution res ijk coordinates in that face's coordinate system
            var fijkBC = fijk;

            // build the H3Index from finest res up
            // adjust r for the fact that the res 0 base cell offsets the indexing digits
            for (var r = res - 1; r >= 0; r--)
            {
                var lastIJK = fijkBC.Coord;
                CoordIjk lastCenter;
                if (IsResolutionClassIII(r + 1))
                {
                    // rotate ccw
                    CoordIjk.UpAp7(ref fijkBC.Coord);
                    lastCenter = fijkBC.Coord;
                    CoordIjk.DownAp7(ref lastCenter);
                }
                else
                {
                    // rotate cw
                    CoordIjk.UpAp7r(ref fijkBC.Coord);
                    lastCenter = fijkBC.Coord;
                    CoordIjk.DownAp7r(ref lastCenter);
                }

                var diff = CoordIjk.IjkSub(lastIJK, lastCenter);
                CoordIjk.IjkNormalize(ref diff);

                h = SetIndexDigit(h, r + 1, CoordIjk.UnitIjkToDigit(diff));
            }

            // fijkBC should now hold the IJK of the base cell in the coordinate system of the current face
            if (fijkBC.Coord.I > BaseCells.MaxFaceCoord || fijkBC.Coord.J > BaseCells.MaxFaceCoord || fijkBC.Coord.K > BaseCells.MaxFaceCoord)
            {
                // out of range input
                return H3Null;
            }

            // lookup the correct base cell
            var baseCell = BaseCells.FaceIjkToBaseCell(fijkBC);
            h = SetBaseCell(h, baseCell);

            // rotate if necessary to get canonical base cell orientation for this base cell
            var numRots = BaseCells.FaceIjkToBaseCellCcwRot60(fijkBC);
            if (BaseCells.IsBaseCellPentagon(baseCell))
            {
                // force rotation out of missing k-axes sub-sequence
                if (LeadingNonZeroDigit(h) == Direction.KAxes)
                {
                    // check for a cw/ccw offset face; default is ccw
                    if (BaseCells.BaseCellIsCwOffset(baseCell, fijkBC.Face))
                    {
                        h = Rotate60cw(h);
                    }
                    else
                    {
                        h = Rotate60ccw(h);
                    }
                }

                for (var i = 0; i < numRots; i++)
                {
                    h = RotatePent60ccw(h);
                }
            }
            else
            {
                for (var i = 0; i < numRots; i++)
                {
                    h = Rotate60ccw(h);
                }
            }

            return h;
        }

        /// <summary>
        /// Encodes a coordinate on the sphere to the H3 index of the containing cell at the specified resolution. C name <c>vec3ToCell</c>.
        /// </summary>
        /// <param name="v">The 3D cartesian coordinates to encode, on the unit sphere.</param>
        /// <param name="res">The desired H3 resolution for the encoding.</param>
        /// <param name="output">The encoded H3 index, or <see cref="H3Null"/> on failure.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, <see cref="H3ErrorCode.ResDomain"/>, <see cref="H3ErrorCode.Domain"/> if a component is not finite, or <see cref="H3ErrorCode.Failed"/>.</returns>
        internal static H3ErrorCode Vec3ToCell(Vec3d v, int res, out ulong output)
        {
            output = H3Null;
            if (res < 0 || res > Constants.MAX_H3_RES)
            {
                return H3ErrorCode.ResDomain;
            }

            if (!IsFinite(v.X) || !IsFinite(v.Y) || !IsFinite(v.Z))
            {
                return H3ErrorCode.Domain;
            }

            var fijk = FaceIjk.Vec3ToFaceIjk(v, res);
            output = FaceIjkToH3(fijk, res);
            return output != H3Null ? H3ErrorCode.Success : H3ErrorCode.Failed;
        }

        /// <summary>
        /// Convert an H3 index to the FaceIJK address on a specified icosahedral face. C name <c>_h3ToFaceIjkWithInitializedFijk</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="fijk">The FaceIJK address, initialized with the desired face and normalized base cell coordinates.</param>
        /// <returns>True if the possibility of overage exists, otherwise false.</returns>
        internal static bool H3ToFaceIjkWithInitializedFijk(ulong h, ref FaceIjk fijk)
        {
            var res = GetResolution(h);

            // center base cell hierarchy is entirely on this face
            var possibleOverage = true;
            if (!BaseCells.IsBaseCellPentagon(GetBaseCellNumber(h)) && (res == 0 || (fijk.Coord.I == 0 && fijk.Coord.J == 0 && fijk.Coord.K == 0)))
            {
                possibleOverage = false;
            }

            for (var r = 1; r <= res; r++)
            {
                if (IsResolutionClassIII(r))
                {
                    // Class III == rotate ccw
                    CoordIjk.DownAp7(ref fijk.Coord);
                }
                else
                {
                    // Class II == rotate cw
                    CoordIjk.DownAp7r(ref fijk.Coord);
                }

                CoordIjk.Neighbor(ref fijk.Coord, GetIndexDigit(h, r));
            }

            return possibleOverage;
        }

        /// <summary>
        /// Convert an H3 index to a FaceIJK address. C name <c>_h3ToFaceIjk</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="fijk">The corresponding FaceIJK address.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.CellInvalid"/> if the base cell is out of range.</returns>
        internal static H3ErrorCode H3ToFaceIjk(ulong h, out FaceIjk fijk)
        {
            var baseCell = GetBaseCellNumber(h);
            if (baseCell < 0 || baseCell >= Constants.NUM_BASE_CELLS)
            {
                // Base cells less than zero can not be represented in an index. To prevent reading uninitialized memory, we zero the output.
                fijk = new FaceIjk(0, new CoordIjk(0, 0, 0));
                return H3ErrorCode.CellInvalid;
            }

            // adjust for the pentagonal missing sequence; all of sub-sequence 5 needs to be adjusted (and some of sub-sequence 4 below)
            if (BaseCells.IsBaseCellPentagon(baseCell) && LeadingNonZeroDigit(h) == Direction.IkAxes)
            {
                h = Rotate60cw(h);
            }

            // start with the "home" face and ijk+ coordinates for the base cell of c
            fijk = BaseCells.BaseCellToFaceIjk(baseCell);
            if (!H3ToFaceIjkWithInitializedFijk(h, ref fijk))
            {
                // no overage is possible; h lies on this face
                return H3ErrorCode.Success;
            }

            // if we're here we have the potential for an "overage"; i.e., it is possible that c lies on an adjacent face
            var origIJK = fijk.Coord;

            // if we're in Class III, drop into the next finer Class II grid
            var res = GetResolution(h);
            if (IsResolutionClassIII(res))
            {
                // Class III
                CoordIjk.DownAp7r(ref fijk.Coord);
                res++;
            }

            // adjust for overage if needed
            // a pentagon base cell with a leading 4 digit requires special handling
            var pentLeading4 = BaseCells.IsBaseCellPentagon(baseCell) && LeadingNonZeroDigit(h) == Direction.IAxes;
            if (FaceIjk.AdjustOverageClassII(ref fijk, res, pentLeading4, false) != Overage.NoOverage)
            {
                // if the base cell is a pentagon we have the potential for secondary overages
                if (BaseCells.IsBaseCellPentagon(baseCell))
                {
                    while (FaceIjk.AdjustOverageClassII(ref fijk, res, false, false) != Overage.NoOverage)
                    {
                        continue;
                    }
                }

                if (res != GetResolution(h))
                {
                    CoordIjk.UpAp7r(ref fijk.Coord);
                }
            }
            else if (res != GetResolution(h))
            {
                fijk.Coord = origIJK;
            }

            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Determines the 3D cartesian coordinates of the center of an H3 cell. C name <c>cellToVec3</c>.
        /// </summary>
        /// <param name="h3">The H3 index.</param>
        /// <param name="v">The 3D cartesian coordinates of the H3 cell center.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.CellInvalid"/> if the base cell is out of range.</returns>
        internal static H3ErrorCode CellToVec3(ulong h3, out Vec3d v)
        {
            var e = H3ToFaceIjk(h3, out var fijk);
            if (e != H3ErrorCode.Success)
            {
                v = default;
                return e;
            }

            v = FaceIjk.FaceIjkToVec3(fijk, GetResolution(h3));
            return H3ErrorCode.Success;
        }

        /// <summary>
        /// The C <c>isfinite</c>: true unless the value is NaN or infinite.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if finite.</returns>
        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
