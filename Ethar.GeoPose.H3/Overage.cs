// <copyright file="Overage.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/include/faceijk.h. Copyright 2016-2021, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    /// <summary>
    /// Digit representing an overage type, that is whether a face coordinate has crossed onto a neighbouring icosahedron face. Ported from the C enum <c>Overage</c>.
    /// </summary>
    internal enum Overage
    {
        /// <summary>
        /// No overage, on the original face. C name <c>NO_OVERAGE</c>.
        /// </summary>
        NoOverage = 0,

        /// <summary>
        /// On a face edge. Only occurs on substrate grids. C name <c>FACE_EDGE</c>.
        /// </summary>
        FaceEdge = 1,

        /// <summary>
        /// Overage on a new face interior. C name <c>NEW_FACE</c>.
        /// </summary>
        NewFace = 2,
    }
}
