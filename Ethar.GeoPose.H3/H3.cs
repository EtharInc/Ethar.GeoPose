// <copyright file="H3.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/h3Index.c and src/h3lib/include/h3Index.h. Copyright 2016-2021, 2024, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using System.Globalization;
    using Ethar.GeoPose.H3.Tables;

    /// <summary>
    /// The H3 engine. Public members carry the H3 v4 function names in PascalCase and take the same arguments, so the H3 documentation applies directly.
    /// Angles are radians throughout, exactly as in H3. Use the extension methods in <c>Ethar.GeoPose.Extensions</c> for GeoPose positions in degrees.
    /// </summary>
    public static partial class H3
    {
        /// <summary>
        /// Invalid index used to indicate an error or missing data. Analogous to NaN in floating point. C name <c>H3_NULL</c>.
        /// </summary>
        public const ulong H3Null = 0;

        /// <summary>
        /// The number of bits in an H3 index. C name <c>H3_NUM_BITS</c>.
        /// </summary>
        internal const int NumBits = 64;

        /// <summary>
        /// The bit offset of the max resolution digit in an H3 index. C name <c>H3_MAX_OFFSET</c>.
        /// </summary>
        internal const int MaxOffset = 63;

        /// <summary>
        /// The bit offset of the mode in an H3 index. C name <c>H3_MODE_OFFSET</c>.
        /// </summary>
        internal const int ModeOffset = 59;

        /// <summary>
        /// The bit offset of the base cell in an H3 index. C name <c>H3_BC_OFFSET</c>.
        /// </summary>
        internal const int BcOffset = 45;

        /// <summary>
        /// The bit offset of the resolution in an H3 index. C name <c>H3_RES_OFFSET</c>.
        /// </summary>
        internal const int ResOffset = 52;

        /// <summary>
        /// The bit offset of the reserved bits in an H3 index. C name <c>H3_RESERVED_OFFSET</c>.
        /// </summary>
        internal const int ReservedOffset = 56;

        /// <summary>
        /// The number of bits in a single H3 resolution digit. C name <c>H3_PER_DIGIT_OFFSET</c>.
        /// </summary>
        internal const int PerDigitOffset = 3;

        /// <summary>
        /// 1 in the highest bit, 0's everywhere else. C name <c>H3_HIGH_BIT_MASK</c>.
        /// </summary>
        internal const ulong HighBitMask = 1UL << MaxOffset;

        /// <summary>
        /// 0 in the highest bit, 1's everywhere else. C name <c>H3_HIGH_BIT_MASK_NEGATIVE</c>.
        /// </summary>
        internal const ulong HighBitMaskNegative = ~HighBitMask;

        /// <summary>
        /// 1's in the 4 mode bits, 0's everywhere else. C name <c>H3_MODE_MASK</c>.
        /// </summary>
        internal const ulong ModeMask = 15UL << ModeOffset;

        /// <summary>
        /// 0's in the 4 mode bits, 1's everywhere else. C name <c>H3_MODE_MASK_NEGATIVE</c>.
        /// </summary>
        internal const ulong ModeMaskNegative = ~ModeMask;

        /// <summary>
        /// 1's in the 7 base cell bits, 0's everywhere else. C name <c>H3_BC_MASK</c>.
        /// </summary>
        internal const ulong BcMask = 127UL << BcOffset;

        /// <summary>
        /// 0's in the 7 base cell bits, 1's everywhere else. C name <c>H3_BC_MASK_NEGATIVE</c>.
        /// </summary>
        internal const ulong BcMaskNegative = ~BcMask;

        /// <summary>
        /// 1's in the 4 resolution bits, 0's everywhere else. C name <c>H3_RES_MASK</c>.
        /// </summary>
        internal const ulong ResMask = 15UL << ResOffset;

        /// <summary>
        /// 0's in the 4 resolution bits, 1's everywhere else. C name <c>H3_RES_MASK_NEGATIVE</c>.
        /// </summary>
        internal const ulong ResMaskNegative = ~ResMask;

        /// <summary>
        /// 1's in the 3 reserved bits, 0's everywhere else. C name <c>H3_RESERVED_MASK</c>.
        /// </summary>
        internal const ulong ReservedMask = 7UL << ReservedOffset;

        /// <summary>
        /// 0's in the 3 reserved bits, 1's everywhere else. C name <c>H3_RESERVED_MASK_NEGATIVE</c>.
        /// </summary>
        internal const ulong ReservedMaskNegative = ~ReservedMask;

        /// <summary>
        /// 1's in the 3 bits of the res 15 digit, 0's everywhere else. C name <c>H3_DIGIT_MASK</c>.
        /// </summary>
        internal const ulong DigitMask = 7UL;

        /// <summary>
        /// 0's in the 3 bits of the res 15 digit, 1's everywhere else. C name <c>H3_DIGIT_MASK_NEGATIVE</c>.
        /// </summary>
        internal const ulong DigitMaskNegative = ~DigitMask;

        /// <summary>
        /// H3 index with mode 0, res 0, base cell 0, and 7 for all index digits. Used to initialize the creation of an H3 cell index, which expects all direction digits to be 7 beyond the cell's resolution. C name <c>H3_INIT</c>.
        /// </summary>
        internal const ulong H3Init = 35184372088831UL;

        /// <summary>
        /// Returns the H3 resolution of an H3 index. C name <c>getResolution</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>The resolution of the H3 index argument.</returns>
        public static int GetResolution(ulong h)
        {
            return (int)((h & ResMask) >> ResOffset);
        }

        /// <summary>
        /// Returns the H3 base cell "number" of an H3 cell (hexagon or pentagon). C name <c>getBaseCellNumber</c>.
        /// </summary>
        /// <param name="h">The H3 cell.</param>
        /// <returns>The base cell "number" of the H3 cell argument.</returns>
        public static int GetBaseCellNumber(ulong h)
        {
            return (int)((h & BcMask) >> BcOffset);
        }

        /// <summary>
        /// Returns the index digit at <paramref name="res"/>, which starts with 1 for resolution 1, up to and including resolution 15. C name <c>getIndexDigit</c>.
        /// </summary>
        /// <param name="h">The H3 index (e.g. cell).</param>
        /// <param name="res">Which indexing digit to retrieve, starting with 1. It may exceed the actual resolution of the index, in which case the stored digit is returned. For valid cells this is 7.</param>
        /// <param name="digit">Receives the value of the indexing digit.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.ResDomain"/> if <paramref name="res"/> is not between 1 and 15.</returns>
        public static H3ErrorCode GetIndexDigit(ulong h, int res, out int digit)
        {
            if (res < 1 || res > Constants.MAX_H3_RES)
            {
                digit = 0;
                return H3ErrorCode.ResDomain;
            }

            digit = (int)GetIndexDigit(h, res);
            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Converts a string representation of an H3 index into an H3 index. C name <c>stringToH3</c>.
        /// </summary>
        /// <param name="str">The hexadecimal string representation of an H3 index. Upper and lower case are accepted and surrounding whitespace is ignored. Unlike the C library, which uses <c>sscanf</c>, a <c>0x</c> prefix, a sign or trailing text are rejected.</param>
        /// <param name="output">The H3 index corresponding to the string argument, or <see cref="H3Null"/> on failure.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.Failed"/> if the text is not a hexadecimal number that fits in 64 bits.</returns>
        public static H3ErrorCode StringToH3(string str, out ulong output)
        {
            output = H3Null;
            if (str == null)
            {
                return H3ErrorCode.Failed;
            }

            var trimmed = str.Trim();
            if (trimmed.Length == 0 || !ulong.TryParse(trimmed, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var parsed))
            {
                return H3ErrorCode.Failed;
            }

            output = parsed;
            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Converts an H3 index into its lowercase hexadecimal string representation, without leading zeros, exactly as the C library does. Every valid cell index is 15 characters long. C name <c>h3ToString</c>.
        /// </summary>
        /// <param name="h">The H3 index to convert.</param>
        /// <returns>The string representation of the H3 index.</returns>
        public static string H3ToString(ulong h)
        {
            return h.ToString("x", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns whether or not an H3 index is a valid cell (hexagon or pentagon). C name <c>isValidCell</c>.
        /// </summary>
        /// <param name="h">The H3 index to validate.</param>
        /// <returns>True if the H3 index is valid, false if it is not.</returns>
        public static bool IsValidCell(ulong h)
        {
            // Look for bit patterns that would disqualify an H3Index from being valid. If identified, exit early.
            //
            // For reference the H3 index bit layout:
            //
            // |   Region   | # bits |
            // |------------|--------|
            // | High       |      1 |
            // | Mode       |      4 |
            // | Reserved   |      3 |
            // | Resolution |      4 |
            // | Base Cell  |      7 |
            // | Digit 1    |      3 |
            // | Digit 2    |      3 |
            // | ...        |    ... |
            // | Digit 15   |      3 |
            //
            // Speed benefits come from using bit manipulation instead of loops, whenever possible.
            if (!HasGoodTopBits(h))
            {
                return false;
            }

            // No need to check resolution; any 4 bits give a valid resolution.
            var res = GetResolution(h);

            // Get base cell number and check that it is valid.
            var bc = GetBaseCellNumber(h);
            if (bc >= Constants.NUM_BASE_CELLS)
            {
                return false;
            }

            if (HasAny7UpToRes(h, res))
            {
                return false;
            }

            if (!HasAll7AfterRes(h, res))
            {
                return false;
            }

            if (HasDeletedSubsequence(h, bc))
            {
                return false;
            }

            // If no disqualifications were identified, the index is a valid H3 cell.
            return true;
        }

        /// <summary>
        /// Determines whether or not the given H3 index is a pentagon. C name <c>isPentagon</c>.
        /// </summary>
        /// <param name="h">The H3 index to check.</param>
        /// <returns>True if it is a pentagon, otherwise false.</returns>
        public static bool IsPentagon(ulong h)
        {
            return BaseCells.IsBaseCellPentagon(GetBaseCellNumber(h)) && LeadingNonZeroDigit(h) == Direction.Center;
        }

        /// <summary>
        /// Determines whether the given H3 index is in a Class III resolution (rotated versus the icosahedron and subject to shape distortion adding extra points on icosahedron edges, making them not true hexagons). C name <c>isResClassIII</c>.
        /// </summary>
        /// <param name="h">The H3 index to check.</param>
        /// <returns>True if the resolution is class III, otherwise false.</returns>
        public static bool IsResClassIII(ulong h)
        {
            return GetResolution(h) % 2 == 1;
        }

        /// <summary>
        /// Returns the number of pentagons, which is the same at any resolution. C name <c>pentagonCount</c>.
        /// </summary>
        /// <returns>The count of pentagon indexes, 12.</returns>
        public static int PentagonCount()
        {
            return Constants.NUM_PENTAGONS;
        }

        /// <summary>
        /// Generates all pentagons at the specified resolution. C name <c>getPentagons</c>.
        /// </summary>
        /// <param name="res">The resolution to produce pentagons at.</param>
        /// <param name="output">Output array. Must have at least <see cref="PentagonCount"/> elements.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15, or <see cref="H3ErrorCode.MemoryBounds"/> if the array is missing or too small.</returns>
        public static H3ErrorCode GetPentagons(int res, ulong[] output)
        {
            if (res < 0 || res > Constants.MAX_H3_RES)
            {
                return H3ErrorCode.ResDomain;
            }

            if (output == null || output.Length < Constants.NUM_PENTAGONS)
            {
                return H3ErrorCode.MemoryBounds;
            }

            var i = 0;
            for (var bc = 0; bc < Constants.NUM_BASE_CELLS; bc++)
            {
                if (BaseCells.IsBaseCellPentagon(bc))
                {
                    output[i++] = SetH3Index(res, bc, Direction.Center);
                }
            }

            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Returns the number of resolution 0 cells. C name <c>res0CellCount</c>.
        /// </summary>
        /// <returns>The count of resolution 0 cells, 122.</returns>
        public static int Res0CellCount()
        {
            return Constants.NUM_BASE_CELLS;
        }

        /// <summary>
        /// Generates all base cells. C name <c>getRes0Cells</c>.
        /// </summary>
        /// <param name="output">Output array. Must have at least <see cref="Res0CellCount"/> elements.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.MemoryBounds"/> if the array is missing or too small.</returns>
        public static H3ErrorCode GetRes0Cells(ulong[] output)
        {
            if (output == null || output.Length < Constants.NUM_BASE_CELLS)
            {
                return H3ErrorCode.MemoryBounds;
            }

            for (var bc = 0; bc < Constants.NUM_BASE_CELLS; bc++)
            {
                var baseCell = H3Init;
                baseCell = SetMode(baseCell, Constants.H3_CELL_MODE);
                baseCell = SetBaseCell(baseCell, bc);
                output[bc] = baseCell;
            }

            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Gets the highest bit of the H3 index. C name <c>H3_GET_HIGH_BIT</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>The high bit, 0 or 1.</returns>
        internal static int GetHighBit(ulong h)
        {
            return (int)((h & HighBitMask) >> MaxOffset);
        }

        /// <summary>
        /// Sets the highest bit of the H3 index. C name <c>H3_SET_HIGH_BIT</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="v">The bit value, 0 or 1.</param>
        /// <returns>The updated index.</returns>
        internal static ulong SetHighBit(ulong h, int v)
        {
            return (h & HighBitMaskNegative) | ((ulong)v << MaxOffset);
        }

        /// <summary>
        /// Gets the integer mode of the H3 index. C name <c>H3_GET_MODE</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>The mode, 0 to 15.</returns>
        internal static int GetMode(ulong h)
        {
            return (int)((h & ModeMask) >> ModeOffset);
        }

        /// <summary>
        /// Sets the integer mode of the H3 index. C name <c>H3_SET_MODE</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="v">The mode, 0 to 15.</param>
        /// <returns>The updated index.</returns>
        internal static ulong SetMode(ulong h, int v)
        {
            return (h & ModeMaskNegative) | ((ulong)v << ModeOffset);
        }

        /// <summary>
        /// Sets the integer base cell of the H3 index. C name <c>H3_SET_BASE_CELL</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="bc">The base cell, 0 to 127.</param>
        /// <returns>The updated index.</returns>
        internal static ulong SetBaseCell(ulong h, int bc)
        {
            return (h & BcMaskNegative) | ((ulong)bc << BcOffset);
        }

        /// <summary>
        /// Sets the integer resolution of the H3 index. C name <c>H3_SET_RESOLUTION</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="res">The resolution, 0 to 15.</param>
        /// <returns>The updated index.</returns>
        internal static ulong SetResolution(ulong h, int res)
        {
            return (h & ResMaskNegative) | ((ulong)res << ResOffset);
        }

        /// <summary>
        /// Gets the resolution <paramref name="res"/> integer digit (0-7) of the H3 index. C name <c>H3_GET_INDEX_DIGIT</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="res">The resolution of the digit, 1 to 15.</param>
        /// <returns>The digit.</returns>
        internal static Direction GetIndexDigit(ulong h, int res)
        {
            return (Direction)((h >> ((Constants.MAX_H3_RES - res) * PerDigitOffset)) & DigitMask);
        }

        /// <summary>
        /// Sets the resolution <paramref name="res"/> digit of the H3 index to the integer digit (0-7). C name <c>H3_SET_INDEX_DIGIT</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="res">The resolution of the digit, 1 to 15.</param>
        /// <param name="digit">The digit.</param>
        /// <returns>The updated index.</returns>
        internal static ulong SetIndexDigit(ulong h, int res, Direction digit)
        {
            var shift = (Constants.MAX_H3_RES - res) * PerDigitOffset;
            return (h & ~(DigitMask << shift)) | ((ulong)digit << shift);
        }

        /// <summary>
        /// Gets the value in the reserved space. Should always be zero for valid indexes. C name <c>H3_GET_RESERVED_BITS</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>The reserved bits, 0 to 7.</returns>
        internal static int GetReservedBits(ulong h)
        {
            return (int)((h & ReservedMask) >> ReservedOffset);
        }

        /// <summary>
        /// Sets a value in the reserved space. Setting to non-zero may produce invalid indexes. C name <c>H3_SET_RESERVED_BITS</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="v">The reserved bits, 0 to 7.</param>
        /// <returns>The updated index.</returns>
        internal static ulong SetReservedBits(ulong h, int v)
        {
            return (h & ReservedMaskNegative) | ((ulong)v << ReservedOffset);
        }

        /// <summary>
        /// Initializes an H3 cell index. C name <c>setH3Index</c>.
        /// </summary>
        /// <param name="res">The H3 resolution to initialize the index to.</param>
        /// <param name="baseCell">The H3 base cell to initialize the index to.</param>
        /// <param name="initDigit">The H3 digit (0-7) to initialize all of the index digits to.</param>
        /// <returns>The initialized index.</returns>
        internal static ulong SetH3Index(int res, int baseCell, Direction initDigit)
        {
            var h = H3Init;
            h = SetMode(h, Constants.H3_CELL_MODE);
            h = SetResolution(h, res);
            h = SetBaseCell(h, baseCell);
            for (var r = 1; r <= res; r++)
            {
                h = SetIndexDigit(h, r, initDigit);
            }

            return h;
        }

        /// <summary>
        /// Returns whether or not a resolution is a Class III grid. Odd resolutions are Class III and even resolutions are Class II. C name <c>isResolutionClassIII</c>.
        /// </summary>
        /// <param name="res">The H3 resolution.</param>
        /// <returns>True if the resolution is a Class III grid, false if it is Class II.</returns>
        internal static bool IsResolutionClassIII(int res)
        {
            return res % 2 == 1;
        }

        /// <summary>
        /// Returns the highest resolution non-zero digit in an H3 index. C name <c>_h3LeadingNonZeroDigit</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>The highest resolution non-zero digit, or <see cref="Direction.Center"/> if all digits are zero.</returns>
        internal static Direction LeadingNonZeroDigit(ulong h)
        {
            var res = GetResolution(h);
            for (var r = 1; r <= res; r++)
            {
                var digit = GetIndexDigit(h, r);
                if (digit != Direction.Center)
                {
                    return digit;
                }
            }

            // if we're here it's all 0's
            return Direction.Center;
        }

        /// <summary>
        /// Rotate an H3 index 60 degrees counter-clockwise about a pentagonal center. C name <c>_h3RotatePent60ccw</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>The rotated index.</returns>
        internal static ulong RotatePent60ccw(ulong h)
        {
            // rotate in place; skips any leading 1 digits (k-axis)
            var foundFirstNonZeroDigit = false;
            for (int r = 1, res = GetResolution(h); r <= res; r++)
            {
                // rotate this digit
                h = SetIndexDigit(h, r, CoordIjkTables.ROTATE60CCW[(int)GetIndexDigit(h, r)]);

                // look for the first non-zero digit so we can adjust for deleted k-axes sequence if necessary
                if (!foundFirstNonZeroDigit && GetIndexDigit(h, r) != Direction.Center)
                {
                    foundFirstNonZeroDigit = true;

                    // adjust for deleted k-axes sequence
                    if (LeadingNonZeroDigit(h) == Direction.KAxes)
                    {
                        h = Rotate60ccw(h);
                    }
                }
            }

            return h;
        }

        /// <summary>
        /// Rotate an H3 index 60 degrees clockwise about a pentagonal center. C name <c>_h3RotatePent60cw</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>The rotated index.</returns>
        internal static ulong RotatePent60cw(ulong h)
        {
            // rotate in place; skips any leading 1 digits (k-axis)
            var foundFirstNonZeroDigit = false;
            for (int r = 1, res = GetResolution(h); r <= res; r++)
            {
                // rotate this digit
                h = SetIndexDigit(h, r, CoordIjkTables.ROTATE60CW[(int)GetIndexDigit(h, r)]);

                // look for the first non-zero digit so we can adjust for deleted k-axes sequence if necessary
                if (!foundFirstNonZeroDigit && GetIndexDigit(h, r) != Direction.Center)
                {
                    foundFirstNonZeroDigit = true;

                    // adjust for deleted k-axes sequence
                    if (LeadingNonZeroDigit(h) == Direction.KAxes)
                    {
                        h = Rotate60cw(h);
                    }
                }
            }

            return h;
        }

        /// <summary>
        /// Rotate an H3 index 60 degrees counter-clockwise. C name <c>_h3Rotate60ccw</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>The rotated index.</returns>
        internal static ulong Rotate60ccw(ulong h)
        {
            for (int r = 1, res = GetResolution(h); r <= res; r++)
            {
                var oldDigit = GetIndexDigit(h, r);
                h = SetIndexDigit(h, r, CoordIjkTables.ROTATE60CCW[(int)oldDigit]);
            }

            return h;
        }

        /// <summary>
        /// Rotate an H3 index 60 degrees clockwise. C name <c>_h3Rotate60cw</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>The rotated index.</returns>
        internal static ulong Rotate60cw(ulong h)
        {
            for (int r = 1, res = GetResolution(h); r <= res; r++)
            {
                h = SetIndexDigit(h, r, CoordIjkTables.ROTATE60CW[(int)GetIndexDigit(h, r)]);
            }

            return h;
        }

        /// <summary>
        /// The top 8 bits of any cell should be a specific constant: the 1 high bit 0, the 4 mode bits 0001 (H3_CELL_MODE) and the 3 reserved bits 000. In total the top 8 bits should be 0_0001_000. C name <c>_hasGoodTopBits</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <returns>True if the top bits are those of a cell.</returns>
        private static bool HasGoodTopBits(ulong h)
        {
            h >>= 64 - 8;
            return h == 0b00001000;
        }

        /// <summary>
        /// Check that no digit from 1 to <paramref name="res"/> is 7 (INVALID_DIGIT). C name <c>_hasAny7UptoRes</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="res">The resolution.</param>
        /// <returns>True if any digit up to the resolution is 7.</returns>
        /// <remarks>
        /// The bit trick identifies the lowest 7 without a loop. A 7 causes a carry when computing <c>~d - MLO</c> that survives the mask; digits 0 to 6 do not.
        /// A carry out of a 7 can misidentify a 6 in the next digit up, but by then the lowest 7 has already been found, so the result is still correct.
        /// See https://github.com/uber/h3/pull/496#discussion_r795851046 for the full derivation.
        /// </remarks>
        private static bool HasAny7UpToRes(ulong h, int res)
        {
            const ulong MHI = 0b100100100100100100100100100100100100100100100UL;
            const ulong MLO = MHI >> 2;

            var shift = 3 * (15 - res);
            h >>= shift;
            h <<= shift;
            h = unchecked(h & MHI & (~h - MLO));

            return h != 0;
        }

        /// <summary>
        /// Check that all unused digits after <paramref name="res"/> are set to 7 (INVALID_DIGIT). Bit shifts avoid looping through digits. C name <c>_hasAll7AfterRes</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="res">The resolution.</param>
        /// <returns>True if every digit after the resolution is 7.</returns>
        private static bool HasAll7AfterRes(ulong h, int res)
        {
            // NOTE: res check is needed because we can't shift by 64
            if (res < 15)
            {
                var shift = 19 + (3 * res);

                h = ~h;
                h <<= shift;
                h >>= shift;

                return h == 0;
            }

            return true;
        }

        /// <summary>
        /// Get the index of the first nonzero bit of an H3 index. The C library uses compiler intrinsics where available; this is its portable fallback. C name <c>_firstOneIndex</c>.
        /// </summary>
        /// <param name="h">The H3 index, which must be non-zero in its lower 45 bits.</param>
        /// <returns>The bit position of the highest set bit.</returns>
        private static int FirstOneIndex(ulong h)
        {
            var pos = 63 - 19;
            ulong m = 1;
            while ((h & (m << pos)) == 0)
            {
                pos--;
            }

            return pos;
        }

        /// <summary>
        /// One final validation just for cells whose base cell (res 0) is a pentagon. Pentagon cells start with a sequence of 0's (CENTER_DIGIT's). The first nonzero digit can't be a 1 (the "deleted subsequence", PENTAGON_SKIPPED_DIGIT or K_AXES_DIGIT).
        /// In the lower 45 = 15*3 bits the position of the first 1 bit must not be divisible by 3. C name <c>_hasDeletedSubsequence</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="baseCell">The base cell number, already known to be in range.</param>
        /// <returns>True if the index is under a pentagon and its first nonzero digit is 1.</returns>
        private static bool HasDeletedSubsequence(ulong h, int baseCell)
        {
            if (BaseCells.IsBaseCellPentagon(baseCell))
            {
                h <<= 19;
                h >>= 19;

                if (h == 0)
                {
                    // all zeros: res 15 pentagon
                    return false;
                }

                return FirstOneIndex(h) % 3 == 0;
            }

            return false;
        }
    }
}
