// <copyright file="LeftHandedYUpVector.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Conventions
{
    using System;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// A vector in a left-handed, Y-up engine frame such as Unity's world space, in meters.
    /// </summary>
    /// <remarks>
    /// X points right, Y points up and Z points forward. Positive rotation about Y is clockwise when viewed from above.
    /// This is the opposite sense to the right-handed LTP-ENU frame; use <see cref="LeftHandedYUpConversions"/> to convert.
    /// </remarks>
    public struct LeftHandedYUpVector : IEquatable<LeftHandedYUpVector>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeftHandedYUpVector"/> struct.
        /// </summary>
        /// <param name="x">The X (right) component.</param>
        /// <param name="y">The Y (up) component.</param>
        /// <param name="z">The Z (forward) component.</param>
        public LeftHandedYUpVector(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Gets the zero vector.
        /// </summary>
        public static LeftHandedYUpVector Zero => default;

        /// <summary>
        /// Gets or sets the X (right) component.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y (up) component.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the Z (forward) component.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The component-wise sum.</returns>
        public static LeftHandedYUpVector operator +(LeftHandedYUpVector a, LeftHandedYUpVector b) => new LeftHandedYUpVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        /// <summary>
        /// Subtracts one vector from another.
        /// </summary>
        /// <param name="a">The vector to subtract from.</param>
        /// <param name="b">The vector to subtract.</param>
        /// <returns>The component-wise difference.</returns>
        public static LeftHandedYUpVector operator -(LeftHandedYUpVector a, LeftHandedYUpVector b) => new LeftHandedYUpVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="LeftHandedYUpVector"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator ==(LeftHandedYUpVector a, LeftHandedYUpVector b) => a.Equals(b);

        /// <summary>
        /// Performs an inequality comparison on two objects of type <see cref="LeftHandedYUpVector"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are not equal.</returns>
        public static bool operator !=(LeftHandedYUpVector a, LeftHandedYUpVector b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(LeftHandedYUpVector other)
        {
            return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is LeftHandedYUpVector equatable && this.Equals(equatable);
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
