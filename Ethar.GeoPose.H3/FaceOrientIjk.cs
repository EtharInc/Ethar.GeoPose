// <copyright file="FaceOrientIjk.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/include/faceijk.h. Copyright 2016-2021, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    /// <summary>
    /// Information to transform into an adjacent face IJK system. Ported from the C struct <c>FaceOrientIJK</c>.
    /// </summary>
    internal readonly struct FaceOrientIjk
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FaceOrientIjk"/> struct.
        /// </summary>
        /// <param name="face">The face number.</param>
        /// <param name="translate">The resolution 0 translation relative to the primary face.</param>
        /// <param name="ccwRot60">The number of 60 degree counter-clockwise rotations relative to the primary face.</param>
        public FaceOrientIjk(int face, CoordIjk translate, int ccwRot60)
        {
            this.Face = face;
            this.Translate = translate;
            this.CcwRot60 = ccwRot60;
        }

        /// <summary>
        /// Gets the face number.
        /// </summary>
        public int Face { get; }

        /// <summary>
        /// Gets the resolution 0 translation relative to the primary face.
        /// </summary>
        public CoordIjk Translate { get; }

        /// <summary>
        /// Gets the number of 60 degree counter-clockwise rotations relative to the primary face.
        /// </summary>
        public int CcwRot60 { get; }
    }
}
