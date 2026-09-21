// <copyright file="NedVector.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Geodesy
{
    using System;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// A displacement in a Local Tangent Plane North-East-Down (LTP-NED) frame, in meters.
    /// </summary>
    /// <remarks>
    /// LTP-NED is right-handed: X points North, Y points East and Z points Down along the ellipsoid normal.
    /// It is the frame used by the Ethar authority's <c>LTP-NED</c> frame specification. Convert to ENU with <see cref="ToEnu"/>.
    /// </remarks>
    public struct NedVector : IEquatable<NedVector>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NedVector"/> struct.
        /// </summary>
        /// <param name="north">The North component in meters.</param>
        /// <param name="east">The East component in meters.</param>
        /// <param name="down">The Down component in meters.</param>
        public NedVector(double north, double east, double down)
        {
            this.North = north;
            this.East = east;
            this.Down = down;
        }

        /// <summary>
        /// Gets or sets the North component in meters.
        /// </summary>
        public double North { get; set; }

        /// <summary>
        /// Gets or sets the East component in meters.
        /// </summary>
        public double East { get; set; }

        /// <summary>
        /// Gets or sets the Down component in meters.
        /// </summary>
        public double Down { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="NedVector"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator ==(NedVector a, NedVector b) => a.Equals(b);

        /// <summary>
        /// Performs an inequality comparison on two objects of type <see cref="NedVector"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are not equal.</returns>
        public static bool operator !=(NedVector a, NedVector b) => !(a == b);

        /// <summary>
        /// Converts this displacement to the East-North-Up convention.
        /// </summary>
        /// <returns>The same displacement expressed as East, North, Up.</returns>
        public EnuVector ToEnu() => new EnuVector(this.East, this.North, -this.Down);

        /// <inheritdoc/>
        public bool Equals(NedVector other)
        {
            return this.North.Equals(other.North) && this.East.Equals(other.East) && this.Down.Equals(other.Down);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is NedVector equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.North.GetHashCode();
                hashcode = (hashcode * 397) ^ this.East.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Down.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"North:{InvariantNumber.Format(this.North)}, East:{InvariantNumber.Format(this.East)}, Down:{InvariantNumber.Format(this.Down)}";
        }
    }
}
