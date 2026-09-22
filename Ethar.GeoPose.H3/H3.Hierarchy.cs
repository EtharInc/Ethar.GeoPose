// <copyright file="H3.Hierarchy.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/h3Index.c and src/h3lib/lib/iterators.c. Copyright 2016-2021, 2024, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using Ethar.GeoPose.H3.Tables;

    /// <summary>
    /// Parent, children and center child, from h3Index.c and the child iterator of iterators.c.
    /// </summary>
    public static partial class H3
    {
        /// <summary>
        /// Produces the parent index for a given H3 index. C name <c>cellToParent</c>.
        /// </summary>
        /// <param name="h">The H3 index to find the parent of.</param>
        /// <param name="parentRes">The resolution to switch to (parent, grandparent, etc).</param>
        /// <param name="output">The H3 index of the parent, or <see cref="H3Null"/> on failure.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15, or <see cref="H3ErrorCode.ResMismatch"/> if the parent resolution is finer than the cell.</returns>
        public static H3ErrorCode CellToParent(ulong h, int parentRes, out ulong output)
        {
            var childRes = GetResolution(h);
            if (parentRes < 0 || parentRes > Constants.MAX_H3_RES)
            {
                output = H3Null;
                return H3ErrorCode.ResDomain;
            }

            if (parentRes > childRes)
            {
                output = H3Null;
                return H3ErrorCode.ResMismatch;
            }

            if (parentRes == childRes)
            {
                output = h;
                return H3ErrorCode.Success;
            }

            var parentH = SetResolution(h, parentRes);
            for (var i = parentRes + 1; i <= childRes; i++)
            {
                parentH = SetIndexDigit(parentH, i, Direction.Invalid);
            }

            output = parentH;
            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Returns the exact number of children for a cell at a given child resolution. C name <c>cellToChildrenSize</c>.
        /// </summary>
        /// <param name="h">The H3 index to find the number of children of.</param>
        /// <param name="childRes">The child resolution.</param>
        /// <param name="output">The exact number of children, handling hexagons and pentagons correctly.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.ResDomain"/> if the child resolution is coarser than the cell or above 15.</returns>
        public static H3ErrorCode CellToChildrenSize(ulong h, int childRes, out long output)
        {
            if (!HasChildAtRes(h, childRes))
            {
                output = 0;
                return H3ErrorCode.ResDomain;
            }

            var n = childRes - GetResolution(h);

            if (IsPentagon(h))
            {
                output = 1 + (5 * (MathExtensions.Ipow(7, n) - 1) / 6);
            }
            else
            {
                output = MathExtensions.Ipow(7, n);
            }

            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Generates all of the children of a cell at the specified resolution. C name <c>cellToChildren</c>.
        /// </summary>
        /// <param name="h">The H3 index to find the children of.</param>
        /// <param name="childRes">The child resolution to produce.</param>
        /// <param name="children">The array to store the resulting indexes in. Use <see cref="CellToChildrenSize"/> to size it.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, <see cref="H3ErrorCode.ResDomain"/> if the child resolution is invalid for the cell, or <see cref="H3ErrorCode.MemoryBounds"/> if the array is missing or too small. The C library assumes the array is large enough; the port checks.</returns>
        public static H3ErrorCode CellToChildren(ulong h, int childRes, ulong[] children)
        {
            var sizeError = CellToChildrenSize(h, childRes, out var size);
            if (sizeError != H3ErrorCode.Success)
            {
                return sizeError;
            }

            if (children == null || children.Length < size)
            {
                return H3ErrorCode.MemoryBounds;
            }

            var i = 0;
            for (var iter = ChildIterator.InitParent(h, childRes); iter.H != H3Null; iter.Step())
            {
                children[i] = iter.H;
                i++;
            }

            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Produces the center child index for a given H3 index at the specified resolution. C name <c>cellToCenterChild</c>.
        /// </summary>
        /// <param name="h">The H3 index to find the center child of.</param>
        /// <param name="childRes">The resolution to switch to.</param>
        /// <param name="child">The H3 index of the center child, or <see cref="H3Null"/> on failure.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.ResDomain"/> if the child resolution is coarser than the cell or above 15.</returns>
        public static H3ErrorCode CellToCenterChild(ulong h, int childRes, out ulong child)
        {
            if (!HasChildAtRes(h, childRes))
            {
                child = H3Null;
                return H3ErrorCode.ResDomain;
            }

            h = ZeroIndexDigits(h, GetResolution(h) + 1, childRes);
            child = SetResolution(h, childRes);
            return H3ErrorCode.Success;
        }

        /// <summary>
        /// Returns the immediate child index based on the specified cell number. Bit operations only, could generate invalid indexes if not careful (deleted cell under a pentagon). C name <c>makeDirectChild</c>.
        /// </summary>
        /// <param name="h">The H3 index to find the direct child of.</param>
        /// <param name="cellNumber">The id of the direct child (0-6).</param>
        /// <returns>The new H3 index for the child.</returns>
        internal static ulong MakeDirectChild(ulong h, int cellNumber)
        {
            var childRes = GetResolution(h) + 1;
            var childH = SetResolution(h, childRes);
            childH = SetIndexDigit(childH, childRes, (Direction)cellNumber);
            return childH;
        }

        /// <summary>
        /// Zero out index digits from start to end, inclusive. No-op if start is greater than end. C name <c>_zeroIndexDigits</c>.
        /// </summary>
        /// <param name="h">The H3 index.</param>
        /// <param name="start">The first digit to zero.</param>
        /// <param name="end">The last digit to zero.</param>
        /// <returns>The updated index.</returns>
        internal static ulong ZeroIndexDigits(ulong h, int start, int end)
        {
            if (start > end)
            {
                return h;
            }

            ulong m = 0;

            m = ~m;
            m <<= PerDigitOffset * (end - start + 1);
            m = ~m;
            m <<= PerDigitOffset * (Constants.MAX_H3_RES - end);
            m = ~m;

            return h & m;
        }

        /// <summary>
        /// Determines whether one resolution is a valid child resolution for a cell. Each resolution is considered a valid child resolution of itself. C name <c>_hasChildAtRes</c>.
        /// </summary>
        /// <param name="h">The parent cell.</param>
        /// <param name="childRes">The resolution of the child.</param>
        /// <returns>The validity of the child resolution.</returns>
        private static bool HasChildAtRes(ulong h, int childRes)
        {
            var parentRes = GetResolution(h);
            return childRes >= parentRes && childRes <= Constants.MAX_H3_RES;
        }

        /// <summary>
        /// Iterates the children of a cell at a given resolution. Ported from the C struct <c>IterCellsChildren</c> and the functions <c>iterInitParent</c> and <c>iterStepChild</c> of iterators.c.
        /// </summary>
        /// <remarks>
        /// Iteration through the children of a hexagon counts every digit between the parent and child resolution through 0 to 6.
        /// A pentagon has only six children, so the first 1 that appears in the "skip digit" is skipped. The skip digit starts at the child resolution and moves to the coarser digit each time a 1 is skipped there.
        /// </remarks>
        private struct ChildIterator
        {
            /// <summary>
            /// The current child, or <see cref="H3Null"/> when the iteration is over.
            /// </summary>
            public ulong H;

            private int parentRes;

            private int skipDigit;

            /// <summary>
            /// Initializes an iterator over the children of cell <paramref name="h"/> at resolution <paramref name="childRes"/>. C name <c>iterInitParent</c>.
            /// </summary>
            /// <param name="h">The parent cell.</param>
            /// <param name="childRes">The child resolution.</param>
            /// <returns>The iterator, positioned on the first child, or on <see cref="H3Null"/> if the input was invalid.</returns>
            public static ChildIterator InitParent(ulong h, int childRes)
            {
                var iter = default(ChildIterator);
                iter.parentRes = GetResolution(h);

                if (childRes < iter.parentRes || childRes > Constants.MAX_H3_RES || h == H3Null)
                {
                    return NullIter();
                }

                iter.H = ZeroIndexDigits(h, iter.parentRes + 1, childRes);
                iter.H = SetResolution(iter.H, childRes);

                if (IsPentagon(iter.H))
                {
                    // The skip digit skips `1` for pentagons.
                    // The "skipDigit" moves to the left as we count up from the child resolution to the parent resolution.
                    iter.skipDigit = childRes;
                }
                else
                {
                    // if not a pentagon, we can ignore "skip digit" logic
                    iter.skipDigit = -1;
                }

                return iter;
            }

            /// <summary>
            /// Steps to the next child cell. When the iteration is over, <see cref="H"/> is <see cref="H3Null"/>. C name <c>iterStepChild</c>.
            /// </summary>
            public void Step()
            {
                // once h == H3_NULL, the iterator returns an infinite sequence of H3_NULL
                if (this.H == H3Null)
                {
                    return;
                }

                var childRes = GetResolution(this.H);

                this.IncrementResDigit(childRes);

                for (var i = childRes; i >= this.parentRes; i--)
                {
                    if (i == this.parentRes)
                    {
                        // if we're modifying the parent resolution digit, then we're done
                        this = NullIter();
                        return;
                    }

                    // PENTAGON_SKIPPED_DIGIT == 1
                    if (i == this.skipDigit && this.GetResDigit(i) == Direction.PentagonSkipped)
                    {
                        // Then we are iterating through the children of a pentagon cell.
                        // All children of a pentagon have the property that the first nonzero digit between the parent and child resolutions is not 1.
                        // I.e., we never see a sequence like 00001. Thus, we skip the `1` in this digit.
                        this.IncrementResDigit(i);
                        this.skipDigit -= 1;
                        return;
                    }

                    // INVALID_DIGIT == 7
                    if (this.GetResDigit(i) == Direction.Invalid)
                    {
                        // zeros out it[i] and increments it[i-1] by 1
                        this.IncrementResDigit(i);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            private static ChildIterator NullIter()
            {
                var iter = default(ChildIterator);
                iter.H = H3Null;
                iter.parentRes = -1;
                iter.skipDigit = -1;
                return iter;
            }

            /// <summary>
            /// Extracts the <paramref name="res"/> digit (0-7) of the current cell. C name <c>_getResDigit</c>.
            /// </summary>
            /// <param name="res">The digit resolution.</param>
            /// <returns>The digit.</returns>
            private Direction GetResDigit(int res)
            {
                return GetIndexDigit(this.H, res);
            }

            /// <summary>
            /// Increments the digit (0-7) at location <paramref name="res"/>, carrying into the next coarser digit on overflow. C name <c>_incrementResDigit</c>.
            /// </summary>
            /// <param name="res">The digit resolution.</param>
            private void IncrementResDigit(int res)
            {
                ulong val = 1;
                val <<= PerDigitOffset * (Constants.MAX_H3_RES - res);
                this.H = unchecked(this.H + val);
            }
        }
    }
}
