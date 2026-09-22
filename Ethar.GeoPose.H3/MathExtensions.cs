// <copyright file="MathExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/mathExtensions.c and src/h3lib/include/mathExtensions.h. Copyright 2017-2018, 2022 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    /// <summary>
    /// Math functions that should have been in math.h but are not. Ported from mathExtensions.c.
    /// </summary>
    internal static class MathExtensions
    {
        /// <summary>
        /// Integer exponentiation by squaring. C name <c>_ipow</c>.
        /// </summary>
        /// <param name="base">The integer base, positive or negative.</param>
        /// <param name="exp">The integer exponent, which should be non-negative.</param>
        /// <returns>The exponentiated value.</returns>
        internal static long Ipow(long @base, long exp)
        {
            long result = 1;
            while (exp != 0)
            {
                if ((exp & 1) != 0)
                {
                    result *= @base;
                }

                exp >>= 1;
                @base *= @base;
            }

            return result;
        }
    }
}
