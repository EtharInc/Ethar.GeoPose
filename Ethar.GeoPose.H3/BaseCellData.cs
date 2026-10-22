// <copyright file="BaseCellData.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/include/baseCells.h. Copyright 2016-2018 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    /// <summary>
    /// Information on a single base cell. Ported from the C struct <c>BaseCellData</c>.
    /// </summary>
    internal readonly struct BaseCellData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCellData"/> struct.
        /// </summary>
        /// <param name="homeFijk">The home face and normalized ijk coordinates on that face.</param>
        /// <param name="isPentagon">Whether the base cell is a pentagon.</param>
        /// <param name="cwOffsetPent0">The first clockwise offset face if a pentagon, otherwise 0. -1 when a pentagon has none.</param>
        /// <param name="cwOffsetPent1">The second clockwise offset face if a pentagon, otherwise 0. -1 when a pentagon has none.</param>
        public BaseCellData(FaceIjk homeFijk, bool isPentagon, int cwOffsetPent0, int cwOffsetPent1)
        {
            this.HomeFijk = homeFijk;
            this.IsPentagon = isPentagon;
            this.CwOffsetPent = new[] { cwOffsetPent0, cwOffsetPent1 };
        }

        /// <summary>
        /// Gets the home face and normalized ijk coordinates on that face.
        /// </summary>
        public FaceIjk HomeFijk { get; }

        /// <summary>
        /// Gets a value indicating whether this base cell is a pentagon.
        /// </summary>
        public bool IsPentagon { get; }

        /// <summary>
        /// Gets the two clockwise offset faces if this base cell is a pentagon. Do not modify the array.
        /// </summary>
        public int[] CwOffsetPent { get; }
    }
}
