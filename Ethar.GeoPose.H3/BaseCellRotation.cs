// <copyright file="BaseCellRotation.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/baseCells.c. Copyright 2016-2020 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    /// <summary>
    /// Base cell at a given ijk and the rotations required into its system. Ported from the C struct <c>BaseCellRotation</c>.
    /// </summary>
    internal readonly struct BaseCellRotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCellRotation"/> struct.
        /// </summary>
        /// <param name="baseCell">The base cell number.</param>
        /// <param name="ccwRot60">The number of counter-clockwise 60 degree rotations relative to the current face.</param>
        public BaseCellRotation(int baseCell, int ccwRot60)
        {
            this.BaseCell = baseCell;
            this.CcwRot60 = ccwRot60;
        }

        /// <summary>
        /// Gets the base cell number.
        /// </summary>
        public int BaseCell { get; }

        /// <summary>
        /// Gets the number of counter-clockwise 60 degree rotations relative to the current face.
        /// </summary>
        public int CcwRot60 { get; }
    }
}
