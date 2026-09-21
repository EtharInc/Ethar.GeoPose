// <copyright file="EnuVector.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Geodesy
{
    using System;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// A displacement in a Local Tangent Plane East-North-Up (LTP-ENU) frame, in meters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// LTP-ENU is the inner frame of the GeoPose Basic targets and the frame in which the Basic yaw, pitch and roll angles are defined.
    /// It is right-handed: X points East, Y points North and Z points Up along the ellipsoid normal at the tangent point.
    /// </para>
    /// <para>
    /// Positive rotation about Up (yaw) is therefore counter-clockwise when viewed from above, turning East towards North.
    /// Engines that use a left-handed, Y-up frame must map both the axes and the sense of rotation; see
    /// <see cref="Ethar.GeoPose.Conventions.LeftHandedYUpConversions"/>.
    /// </para>
    /// </remarks>
    public struct EnuVector : IEquatable<EnuVector>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnuVector"/> struct.
        /// </summary>
        /// <param name="east">The East component in meters.</param>
        /// <param name="north">The North component in meters.</param>
        /// <param name="up">The Up component in meters.</param>
        public EnuVector(double east, double north, double up)
        {
            this.East = east;
            this.North = north;
            this.Up = up;
        }

        /// <summary>
        /// Gets the zero displacement.
        /// </summary>
        public static EnuVector Zero => default;

        /// <summary>
        /// Gets or sets the East component in meters.
        /// </summary>
        public double East { get; set; }

        /// <summary>
        /// Gets or sets the North component in meters.
        /// </summary>
        public double North { get; set; }

        /// <summary>
        /// Gets or sets the Up component in meters.
        /// </summary>
        public double Up { get; set; }

        /// <summary>
        /// Gets the Euclidean length of the displacement in meters.
        /// </summary>
        public double Length => Math.Sqrt((this.East * this.East) + (this.North * this.North) + (this.Up * this.Up));

        /// <summary>
        /// Gets the length of the horizontal (East, North) part of the displacement in meters.
        /// </summary>
        public double HorizontalLength => Math.Sqrt((this.East * this.East) + (this.North * this.North));

        /// <summary>
        /// Adds two displacements.
        /// </summary>
        /// <param name="a">The first displacement.</param>
        /// <param name="b">The second displacement.</param>
        /// <returns>The component-wise sum.</returns>
        public static EnuVector operator +(EnuVector a, EnuVector b) => new EnuVector(a.East + b.East, a.North + b.North, a.Up + b.Up);

        /// <summary>
        /// Subtracts one displacement from another.
        /// </summary>
        /// <param name="a">The displacement to subtract from.</param>
        /// <param name="b">The displacement to subtract.</param>
        /// <returns>The component-wise difference.</returns>
        public static EnuVector operator -(EnuVector a, EnuVector b) => new EnuVector(a.East - b.East, a.North - b.North, a.Up - b.Up);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="EnuVector"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator ==(EnuVector a, EnuVector b) => a.Equals(b);

        /// <summary>
        /// Performs an inequality comparison on two objects of type <see cref="EnuVector"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are not equal.</returns>
        public static bool operator !=(EnuVector a, EnuVector b) => !(a == b);

        /// <summary>
        /// Converts this displacement to the North-East-Down convention used by the LTP-NED frame.
        /// </summary>
        /// <returns>The same displacement expressed as North, East, Down.</returns>
        public NedVector ToNed() => new NedVector(this.North, this.East, -this.Up);

        /// <inheritdoc/>
        public bool Equals(EnuVector other)
        {
            return this.East.Equals(other.East) && this.North.Equals(other.North) && this.Up.Equals(other.Up);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is EnuVector equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.East.GetHashCode();
                hashcode = (hashcode * 397) ^ this.North.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Up.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"East:{InvariantNumber.Format(this.East)}, North:{InvariantNumber.Format(this.North)}, Up:{InvariantNumber.Format(this.Up)}";
        }
    }
}
