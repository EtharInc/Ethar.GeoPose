// <copyright file="CoordIjk.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/include/coordijk.h. Copyright 2016-2018, 2020-2022, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using System;
    using System.Globalization;
    using Ethar.GeoPose.H3.Tables;

    /// <summary>
    /// IJK hexagon coordinates. Each axis is spaced 120 degrees apart. Ported from the C struct <c>CoordIJK</c> and the inline functions of coordijk.h.
    /// </summary>
    /// <remarks>
    /// The struct is mutable so that the ported maths can update a coordinate in place, as the C code does through a pointer.
    /// The local IJ and cube coordinate conversions from coordijk.h are not ported because local IJ is out of scope.
    /// </remarks>
    internal struct CoordIjk
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoordIjk"/> struct.
        /// </summary>
        /// <param name="i">The i component.</param>
        /// <param name="j">The j component.</param>
        /// <param name="k">The k component.</param>
        public CoordIjk(int i, int j, int k)
        {
            this.I = i;
            this.J = j;
            this.K = k;
        }

        /// <summary>
        /// Gets or sets the i component.
        /// </summary>
        public int I { get; set; }

        /// <summary>
        /// Gets or sets the j component.
        /// </summary>
        public int J { get; set; }

        /// <summary>
        /// Gets or sets the k component.
        /// </summary>
        public int K { get; set; }

        /// <summary>
        /// Returns whether or not two ijk coordinates contain exactly the same component values. C name <c>_ijkMatches</c>.
        /// </summary>
        /// <param name="c1">The first set of ijk coordinates.</param>
        /// <param name="c2">The second set of ijk coordinates.</param>
        /// <returns>True if the two addresses match.</returns>
        public static bool IjkMatches(CoordIjk c1, CoordIjk c2)
        {
            return c1.I == c2.I && c1.J == c2.J && c1.K == c2.K;
        }

        /// <summary>
        /// Add two ijk coordinates. C name <c>_ijkAdd</c>.
        /// </summary>
        /// <param name="h1">The first set of ijk coordinates.</param>
        /// <param name="h2">The second set of ijk coordinates.</param>
        /// <returns>The sum of the two sets of ijk coordinates.</returns>
        public static CoordIjk IjkAdd(CoordIjk h1, CoordIjk h2)
        {
            return new CoordIjk(h1.I + h2.I, h1.J + h2.J, h1.K + h2.K);
        }

        /// <summary>
        /// Subtract two ijk coordinates. C name <c>_ijkSub</c>.
        /// </summary>
        /// <param name="h1">The first set of ijk coordinates.</param>
        /// <param name="h2">The second set of ijk coordinates.</param>
        /// <returns>The difference of the two sets of ijk coordinates (h1 - h2).</returns>
        public static CoordIjk IjkSub(CoordIjk h1, CoordIjk h2)
        {
            return new CoordIjk(h1.I - h2.I, h1.J - h2.J, h1.K - h2.K);
        }

        /// <summary>
        /// Uniformly scale ijk coordinates by a scalar. Works in place. C name <c>_ijkScale</c>.
        /// </summary>
        /// <param name="c">The ijk coordinates to scale.</param>
        /// <param name="factor">The scaling factor.</param>
        public static void IjkScale(ref CoordIjk c, int factor)
        {
            c.I *= factor;
            c.J *= factor;
            c.K *= factor;
        }

        /// <summary>
        /// Normalizes ijk coordinates by setting the components to the smallest possible values. Works in place. C name <c>_ijkNormalize</c>.
        /// </summary>
        /// <param name="c">The ijk coordinates to normalize.</param>
        /// <remarks>
        /// This function does not protect against signed integer overflow. The caller must ensure that none of the pairwise differences of the components overflow.
        /// </remarks>
        public static void IjkNormalize(ref CoordIjk c)
        {
            // remove any negative values
            if (c.I < 0)
            {
                c.J -= c.I;
                c.K -= c.I;
                c.I = 0;
            }

            if (c.J < 0)
            {
                c.I -= c.J;
                c.K -= c.J;
                c.J = 0;
            }

            if (c.K < 0)
            {
                c.I -= c.K;
                c.J -= c.K;
                c.K = 0;
            }

            // remove the min value if needed
            var min = c.I;
            if (c.J < min)
            {
                min = c.J;
            }

            if (c.K < min)
            {
                min = c.K;
            }

            if (min > 0)
            {
                c.I -= min;
                c.J -= min;
                c.K -= min;
            }
        }

        /// <summary>
        /// Find the center point in 2D cartesian coordinates of a hex. C name <c>_ijkToHex2d</c>.
        /// </summary>
        /// <param name="h">The ijk coordinates of the hex.</param>
        /// <returns>The 2D cartesian coordinates of the hex center point.</returns>
        public static Vec2d IjkToHex2d(CoordIjk h)
        {
            var i = h.I - h.K;
            var j = h.J - h.K;
            return new Vec2d(i - (0.5 * j), j * Constants.M_SQRT3_2);
        }

        /// <summary>
        /// Determine the containing hex in ijk+ coordinates for a 2D cartesian coordinate vector (from DGGRID). C name <c>_hex2dToCoordIJK</c>.
        /// </summary>
        /// <param name="v">The 2D cartesian coordinate vector.</param>
        /// <returns>The ijk+ coordinates of the containing hex.</returns>
        public static CoordIjk Hex2dToCoordIjk(Vec2d v)
        {
            var h = default(CoordIjk);

            // quantize into the ij system and then normalize
            h.K = 0;

            var a1 = Math.Abs(v.X);
            var a2 = Math.Abs(v.Y);

            // first do a reverse conversion
            var x2 = a2 * Constants.M_RSIN60;
            var x1 = a1 + (x2 / 2.0);

            // check if we have the center of a hex
            var m1 = (int)x1;
            var m2 = (int)x2;

            // otherwise round correctly
            var r1 = x1 - m1;
            var r2 = x2 - m2;

            if (r1 < 0.5)
            {
                if (r1 < 1.0 / 3.0)
                {
                    if (r2 < (1.0 + r1) / 2.0)
                    {
                        h.I = m1;
                        h.J = m2;
                    }
                    else
                    {
                        h.I = m1;
                        h.J = m2 + 1;
                    }
                }
                else
                {
                    if (r2 < 1.0 - r1)
                    {
                        h.J = m2;
                    }
                    else
                    {
                        h.J = m2 + 1;
                    }

                    if (1.0 - r1 <= r2 && r2 < 2.0 * r1)
                    {
                        h.I = m1 + 1;
                    }
                    else
                    {
                        h.I = m1;
                    }
                }
            }
            else
            {
                if (r1 < 2.0 / 3.0)
                {
                    if (r2 < 1.0 - r1)
                    {
                        h.J = m2;
                    }
                    else
                    {
                        h.J = m2 + 1;
                    }

                    if ((2.0 * r1) - 1.0 < r2 && r2 < 1.0 - r1)
                    {
                        h.I = m1;
                    }
                    else
                    {
                        h.I = m1 + 1;
                    }
                }
                else
                {
                    if (r2 < r1 / 2.0)
                    {
                        h.I = m1 + 1;
                        h.J = m2;
                    }
                    else
                    {
                        h.I = m1 + 1;
                        h.J = m2 + 1;
                    }
                }
            }

            // now fold across the axes if necessary
            if (v.X < 0.0)
            {
                if (h.J % 2 == 0)
                {
                    // even
                    long axisi = h.J / 2;
                    long diff = h.I - axisi;
                    h.I = (int)(h.I - (2.0 * diff));
                }
                else
                {
                    long axisi = (h.J + 1) / 2;
                    long diff = h.I - axisi;
                    h.I = (int)(h.I - ((2.0 * diff) + 1));
                }
            }

            if (v.Y < 0.0)
            {
                h.I = h.I - (((2 * h.J) + 1) / 2);
                h.J = -1 * h.J;
            }

            IjkNormalize(ref h);
            return h;
        }

        /// <summary>
        /// Determines the H3 digit corresponding to a unit vector or the zero vector in ijk coordinates. C name <c>_unitIjkToDigit</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates; must be a unit vector or zero vector.</param>
        /// <returns>The H3 digit (0-6) corresponding to the ijk unit vector or zero vector, or <see cref="Direction.Invalid"/> on failure.</returns>
        public static Direction UnitIjkToDigit(CoordIjk ijk)
        {
            var c = ijk;
            IjkNormalize(ref c);

            var digit = Direction.Invalid;
            for (var i = Direction.Center; i < Direction.NumDigits; i++)
            {
                if (IjkMatches(c, CoordIjkTables.UNIT_VECS[(int)i]))
                {
                    digit = i;
                    break;
                }
            }

            return digit;
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the indexing parent of a cell in a counter-clockwise aperture 7 grid. Works in place. C name <c>_upAp7</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates.</param>
        public static void UpAp7(ref CoordIjk ijk)
        {
            // convert to CoordIJ
            var i = ijk.I - ijk.K;
            var j = ijk.J - ijk.K;

            ijk.I = Lround(((3 * i) - j) * Constants.M_ONESEVENTH);
            ijk.J = Lround((i + (2 * j)) * Constants.M_ONESEVENTH);
            ijk.K = 0;
            IjkNormalize(ref ijk);
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the indexing parent of a cell in a clockwise aperture 7 grid. Works in place. C name <c>_upAp7r</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates.</param>
        public static void UpAp7r(ref CoordIjk ijk)
        {
            // convert to CoordIJ
            var i = ijk.I - ijk.K;
            var j = ijk.J - ijk.K;

            ijk.I = Lround(((2 * i) + j) * Constants.M_ONESEVENTH);
            ijk.J = Lround(((3 * j) - i) * Constants.M_ONESEVENTH);
            ijk.K = 0;
            IjkNormalize(ref ijk);
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the hex centered on the indicated hex at the next finer aperture 7 counter-clockwise resolution. Works in place. C name <c>_downAp7</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates.</param>
        public static void DownAp7(ref CoordIjk ijk)
        {
            // res r unit vectors in res r+1
            Combine(ref ijk, new CoordIjk(3, 0, 1), new CoordIjk(1, 3, 0), new CoordIjk(0, 1, 3));
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the hex centered on the indicated hex at the next finer aperture 7 clockwise resolution. Works in place. C name <c>_downAp7r</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates.</param>
        public static void DownAp7r(ref CoordIjk ijk)
        {
            // res r unit vectors in res r+1
            Combine(ref ijk, new CoordIjk(3, 1, 0), new CoordIjk(0, 3, 1), new CoordIjk(1, 0, 3));
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the hex centered on the indicated hex at the next finer aperture 3 counter-clockwise resolution. Works in place. C name <c>_downAp3</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates.</param>
        public static void DownAp3(ref CoordIjk ijk)
        {
            // res r unit vectors in res r+1
            Combine(ref ijk, new CoordIjk(2, 0, 1), new CoordIjk(1, 2, 0), new CoordIjk(0, 1, 2));
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the hex centered on the indicated hex at the next finer aperture 3 clockwise resolution. Works in place. C name <c>_downAp3r</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates.</param>
        public static void DownAp3r(ref CoordIjk ijk)
        {
            // res r unit vectors in res r+1
            Combine(ref ijk, new CoordIjk(2, 1, 0), new CoordIjk(0, 2, 1), new CoordIjk(1, 0, 2));
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the hex in the specified digit direction from the specified ijk coordinates. Works in place. C name <c>_neighbor</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates.</param>
        /// <param name="digit">The digit direction from the original ijk coordinates.</param>
        public static void Neighbor(ref CoordIjk ijk, Direction digit)
        {
            if (digit > Direction.Center && digit < Direction.NumDigits)
            {
                ijk = IjkAdd(ijk, CoordIjkTables.UNIT_VECS[(int)digit]);
                IjkNormalize(ref ijk);
            }
        }

        /// <summary>
        /// Rotates ijk coordinates 60 degrees counter-clockwise. Works in place. C name <c>_ijkRotate60ccw</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates.</param>
        public static void IjkRotate60ccw(ref CoordIjk ijk)
        {
            // unit vector rotations
            Combine(ref ijk, new CoordIjk(1, 1, 0), new CoordIjk(0, 1, 1), new CoordIjk(1, 0, 1));
        }

        /// <summary>
        /// Rotates ijk coordinates 60 degrees clockwise. Works in place. C name <c>_ijkRotate60cw</c>.
        /// </summary>
        /// <param name="ijk">The ijk coordinates.</param>
        public static void IjkRotate60cw(ref CoordIjk ijk)
        {
            // unit vector rotations
            Combine(ref ijk, new CoordIjk(1, 0, 1), new CoordIjk(1, 1, 0), new CoordIjk(0, 1, 1));
        }

        /// <summary>
        /// Rotates an indexing digit 60 degrees counter-clockwise. C name <c>_rotate60ccw</c>.
        /// </summary>
        /// <param name="digit">Indexing digit (between 1 and 6 inclusive).</param>
        /// <returns>The rotated digit. Other digits are returned unchanged.</returns>
        public static Direction Rotate60ccw(Direction digit)
        {
            return CoordIjkTables.ROTATE60CCW[(int)digit];
        }

        /// <summary>
        /// Rotates an indexing digit 60 degrees clockwise. C name <c>_rotate60cw</c>.
        /// </summary>
        /// <param name="digit">Indexing digit (between 1 and 6 inclusive).</param>
        /// <returns>The rotated digit. Other digits are returned unchanged.</returns>
        public static Direction Rotate60cw(Direction digit)
        {
            return CoordIjkTables.ROTATE60CW[(int)digit];
        }

        /// <summary>
        /// Finds the distance between the two coordinates. C name <c>ijkDistance</c>.
        /// </summary>
        /// <param name="c1">The first set of ijk coordinates.</param>
        /// <param name="c2">The second set of ijk coordinates.</param>
        /// <returns>The grid distance.</returns>
        public static int IjkDistance(CoordIjk c1, CoordIjk c2)
        {
            var diff = IjkSub(c1, c2);
            IjkNormalize(ref diff);
            return Math.Max(Math.Abs(diff.I), Math.Max(Math.Abs(diff.J), Math.Abs(diff.K)));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", this.I, this.J, this.K);
        }

        /// <summary>
        /// Replaces <paramref name="ijk"/> with the normalized sum of the three basis vectors scaled by its components. This is the shared body of the aperture and rotation functions.
        /// </summary>
        /// <param name="ijk">The ijk coordinates, updated in place.</param>
        /// <param name="iVec">The image of the i unit vector.</param>
        /// <param name="jVec">The image of the j unit vector.</param>
        /// <param name="kVec">The image of the k unit vector.</param>
        private static void Combine(ref CoordIjk ijk, CoordIjk iVec, CoordIjk jVec, CoordIjk kVec)
        {
            IjkScale(ref iVec, ijk.I);
            IjkScale(ref jVec, ijk.J);
            IjkScale(ref kVec, ijk.K);

            ijk = IjkAdd(iVec, jVec);
            ijk = IjkAdd(ijk, kVec);
            IjkNormalize(ref ijk);
        }

        /// <summary>
        /// Rounds to the nearest integer with halves away from zero, as the C <c>lround</c> does. <see cref="Math.Round(double)"/> would round halves to even.
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <returns>The rounded integer.</returns>
        private static int Lround(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}
