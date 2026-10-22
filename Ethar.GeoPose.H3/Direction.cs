// <copyright file="Direction.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/include/coordijk.h. Copyright 2016-2018, 2020-2022, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    /// <summary>
    /// H3 digit representing an ijk+ axes direction. Values fit in the lowest 3 bits of an integer. Ported from the C enum <c>Direction</c>.
    /// </summary>
    internal enum Direction
    {
        /// <summary>
        /// H3 digit in the centre. C name <c>CENTER_DIGIT</c>.
        /// </summary>
        Center = 0,

        /// <summary>
        /// H3 digit in the k-axes direction. C name <c>K_AXES_DIGIT</c>.
        /// </summary>
        KAxes = 1,

        /// <summary>
        /// H3 digit in the j-axes direction. C name <c>J_AXES_DIGIT</c>.
        /// </summary>
        JAxes = 2,

        /// <summary>
        /// H3 digit in the j == k direction. C name <c>JK_AXES_DIGIT</c>.
        /// </summary>
        JkAxes = JAxes | KAxes,

        /// <summary>
        /// H3 digit in the i-axes direction. C name <c>I_AXES_DIGIT</c>.
        /// </summary>
        IAxes = 4,

        /// <summary>
        /// H3 digit in the i == k direction. C name <c>IK_AXES_DIGIT</c>.
        /// </summary>
        IkAxes = IAxes | KAxes,

        /// <summary>
        /// H3 digit in the i == j direction. C name <c>IJ_AXES_DIGIT</c>.
        /// </summary>
        IjAxes = IAxes | JAxes,

        /// <summary>
        /// H3 digit in the invalid direction. C name <c>INVALID_DIGIT</c>.
        /// </summary>
        Invalid = 7,

        /// <summary>
        /// Valid digits are less than this value. Same value as <see cref="Invalid"/>. C name <c>NUM_DIGITS</c>.
        /// </summary>
        NumDigits = Invalid,

        /// <summary>
        /// Child digit which is skipped for pentagons. C name <c>PENTAGON_SKIPPED_DIGIT</c>.
        /// </summary>
        PentagonSkipped = KAxes,
    }
}
