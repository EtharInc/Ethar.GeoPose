// <copyright file="EcefPosition.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Geodesy
{
    using System;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// A position in the Earth-Centered, Earth-Fixed (ECEF) Cartesian frame, in meters.
    /// </summary>
    /// <remarks>
    /// The frame is right-handed: X points from the Earth's centre through the intersection of the equator and the prime meridian,
    /// Z points through the North pole, and Y completes the right-handed set (through 90° East on the equator).
    /// </remarks>
    public struct EcefPosition : IEquatable<EcefPosition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EcefPosition"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate in meters.</param>
        /// <param name="y">The Y coordinate in meters.</param>
        /// <param name="z">The Z coordinate in meters.</param>
        public EcefPosition(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Gets or sets the X coordinate in meters.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate in meters.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the Z coordinate in meters.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="EcefPosition"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator ==(EcefPosition a, EcefPosition b) => a.Equals(b);

        /// <summary>
        /// Performs an inequality comparison on two objects of type <see cref="EcefPosition"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are not equal.</returns>
        public static bool operator !=(EcefPosition a, EcefPosition b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(EcefPosition other)
        {
            return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is EcefPosition equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.X.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Y.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Z.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"X:{InvariantNumber.Format(this.X)}, Y:{InvariantNumber.Format(this.Y)}, Z:{InvariantNumber.Format(this.Z)}";
        }
    }
}
