// <copyright file="BaseCells.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/baseCells.c and src/h3lib/include/baseCells.h. Copyright 2016-2020 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using Ethar.GeoPose.H3.Tables;

    /// <summary>
    /// Base cell access functions over the tables in <see cref="BaseCellTables"/>. Ported from baseCells.c; the C function names are given on each member.
    /// </summary>
    internal static class BaseCells
    {
        /// <summary>
        /// Marker for a missing neighbouring base cell. C name <c>INVALID_BASE_CELL</c>.
        /// </summary>
        internal const int InvalidBaseCell = 127;

        /// <summary>
        /// Maximum input for any component to the face-to-base-cell lookup functions. C name <c>MAX_FACE_COORD</c>.
        /// </summary>
        internal const int MaxFaceCoord = 2;

        /// <summary>
        /// Invalid number of rotations. C name <c>INVALID_ROTATIONS</c>.
        /// </summary>
        internal const int InvalidRotations = -1;

        /// <summary>
        /// Returns whether or not the indicated base cell is a pentagon. C name <c>_isBaseCellPentagon</c>.
        /// </summary>
        /// <param name="baseCell">The base cell number.</param>
        /// <returns>True if the base cell is one of the 12 pentagons, false otherwise or when the number is out of range.</returns>
        internal static bool IsBaseCellPentagon(int baseCell)
        {
            if (baseCell < 0 || baseCell >= Constants.NUM_BASE_CELLS)
            {
                // Base cells less than zero can not be represented in an index
                return false;
            }

            return BaseCellTables.baseCellData[baseCell].IsPentagon;
        }

        /// <summary>
        /// Returns whether the indicated base cell is a pentagon where all neighbors are oriented towards it. C name <c>_isBaseCellPolarPentagon</c>.
        /// </summary>
        /// <param name="baseCell">The base cell number.</param>
        /// <returns>True for base cells 4 and 117.</returns>
        internal static bool IsBaseCellPolarPentagon(int baseCell)
        {
            return baseCell == 4 || baseCell == 117;
        }

        /// <summary>
        /// Find the base cell given a FaceIJK. Valid ijk+ lookup coordinates are from (0, 0, 0) to (2, 2, 2). C name <c>_faceIjkToBaseCell</c>.
        /// </summary>
        /// <param name="h">The face number and a resolution 0 ijk+ coordinate in that face's face-centered ijk coordinate system.</param>
        /// <returns>The base cell located at that coordinate.</returns>
        internal static int FaceIjkToBaseCell(FaceIjk h)
        {
            return BaseCellTables.faceIjkBaseCells[h.Face, h.Coord.I, h.Coord.J, h.Coord.K].BaseCell;
        }

        /// <summary>
        /// Find the base cell rotation given a FaceIJK. Valid ijk+ lookup coordinates are from (0, 0, 0) to (2, 2, 2). C name <c>_faceIjkToBaseCellCCWrot60</c>.
        /// </summary>
        /// <param name="h">The face number and a resolution 0 ijk+ coordinate in that face's face-centered ijk coordinate system.</param>
        /// <returns>The number of 60 degree counter-clockwise rotations to rotate into the coordinate system of the base cell at that coordinate.</returns>
        internal static int FaceIjkToBaseCellCcwRot60(FaceIjk h)
        {
            return BaseCellTables.faceIjkBaseCells[h.Face, h.Coord.I, h.Coord.J, h.Coord.K].CcwRot60;
        }

        /// <summary>
        /// Find the FaceIJK given a base cell. C name <c>_baseCellToFaceIjk</c>.
        /// </summary>
        /// <param name="baseCell">The base cell number.</param>
        /// <returns>The home face and ijk coordinates of the base cell.</returns>
        internal static FaceIjk BaseCellToFaceIjk(int baseCell)
        {
            return BaseCellTables.baseCellData[baseCell].HomeFijk;
        }

        /// <summary>
        /// Given a base cell and the face it appears on, return the number of 60 degree counter-clockwise rotations for that base cell's coordinate system. C name <c>_baseCellToCCWrot60</c>.
        /// </summary>
        /// <param name="baseCell">The base cell number.</param>
        /// <param name="face">The face number.</param>
        /// <returns>The number of rotations, or <see cref="InvalidRotations"/> if the base cell is not found on the given face.</returns>
        /// <remarks>
        /// The C source tests <c>face &gt; NUM_ICOSA_FACES</c>, which lets face 20 through to an out of range read. The port rejects it.
        /// </remarks>
        internal static int BaseCellToCcwRot60(int baseCell, int face)
        {
            if (face < 0 || face >= Constants.NUM_ICOSA_FACES)
            {
                return InvalidRotations;
            }

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    for (var k = 0; k < 3; k++)
                    {
                        if (BaseCellTables.faceIjkBaseCells[face, i, j, k].BaseCell == baseCell)
                        {
                            return BaseCellTables.faceIjkBaseCells[face, i, j, k].CcwRot60;
                        }
                    }
                }
            }

            return InvalidRotations;
        }

        /// <summary>
        /// Return whether or not the tested face is a clockwise offset face. C name <c>_baseCellIsCwOffset</c>.
        /// </summary>
        /// <param name="baseCell">The base cell number.</param>
        /// <param name="testFace">The face to test.</param>
        /// <returns>True if the face is one of the base cell's two clockwise offset faces.</returns>
        internal static bool BaseCellIsCwOffset(int baseCell, int testFace)
        {
            var offsets = BaseCellTables.baseCellData[baseCell].CwOffsetPent;
            return offsets[0] == testFace || offsets[1] == testFace;
        }

        /// <summary>
        /// Return the neighboring base cell in the given direction. C name <c>_getBaseCellNeighbor</c>.
        /// </summary>
        /// <param name="baseCell">The origin base cell number.</param>
        /// <param name="dir">The direction.</param>
        /// <returns>The neighbouring base cell, or <see cref="InvalidBaseCell"/> if there is none in that direction.</returns>
        internal static int GetBaseCellNeighbor(int baseCell, Direction dir)
        {
            return BaseCellTables.baseCellNeighbors[baseCell, (int)dir];
        }

        /// <summary>
        /// Return the direction from the origin base cell to the neighbor. C name <c>_getBaseCellDirection</c>.
        /// </summary>
        /// <param name="originBaseCell">The origin base cell number.</param>
        /// <param name="neighboringBaseCell">The neighbouring base cell number.</param>
        /// <returns>The direction, or <see cref="Direction.Invalid"/> if the base cells are not neighbors.</returns>
        internal static Direction GetBaseCellDirection(int originBaseCell, int neighboringBaseCell)
        {
            for (var dir = Direction.Center; dir < Direction.NumDigits; dir++)
            {
                var testBaseCell = GetBaseCellNeighbor(originBaseCell, dir);
                if (testBaseCell == neighboringBaseCell)
                {
                    return dir;
                }
            }

            return Direction.Invalid;
        }
    }
}
