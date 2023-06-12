// <copyright file="UnitVector3.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A data type that represents a vector3 for serialization.
    /// </summary>
    public struct UnitVector3 : IEquatable<UnitVector3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitVector3"/> struct.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="z">The z value.</param>
        public UnitVector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Gets a vector whose 3 elements are equal to zero.
        /// </summary>
        /// <returns>
        /// A vector whose three elements are equal to zero (that is, it returns the vector (0,0,0) ).
        /// </returns>
        public static UnitVector3 Zero
        {
            get
            {
                return default;
            }
        }

        /// <summary>
        /// Gets a vector whose 3 elements are equal to one.
        /// </summary>
        /// <returns>
        /// A vector whose three elements are equal to one (that is, it returns the vector (1,1,1) ).
        /// </returns>
        public static UnitVector3 One
        {
            get
            {
                return new UnitVector3(1f, 1f, 1f);
            }
        }

        /// <summary>
        /// Gets the vector (1,0,0).
        /// </summary>
        /// <returns>
        /// The vector (1,0,0).
        /// </returns>
        public static UnitVector3 UnitX
        {
            get
            {
                return new UnitVector3(1f, 0f, 0f);
            }
        }

        /// <summary>
        /// Gets the vector (0,1,0).
        /// </summary>
        /// <returns>
        /// The vector (0,1,0).
        /// </returns>
        public static UnitVector3 UnitY
        {
            get
            {
                return new UnitVector3(0f, 1f, 0f);
            }
        }

        /// <summary>
        /// Gets the vector (0,0,1).
        /// </summary>
        /// <returns>
        /// The vector (0,0,1).
        /// </returns>
        public static UnitVector3 UnitZ
        {
            get
            {
                return new UnitVector3(0f, 0f, 1f);
            }
        }

        /// <summary>
        /// Gets or sets the x value.
        /// </summary>
        [JsonProperty("x")]
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the y value.
        /// </summary>
        [JsonProperty("y")]
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the z value.
        /// </summary>
        [JsonProperty("z")]
        public float Z { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="UnitVector3"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(UnitVector3 a, UnitVector3 b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="UnitVector3"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(UnitVector3 a, UnitVector3 b) => !(a == b);

        /// <summary>
        /// Combines two hash codes, useful for combining hash codes of individual vector elements.
        /// </summary>
        /// <param name="h1">First Hash.</param>
        /// <param name="h2">Second Hash to add.</param>
        /// <returns>Combined Hashcode.</returns>
        public static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        /// <inheritdoc/>
        public bool Equals(UnitVector3 other)
        {
            return this.X.Equals(other.X)
                && this.Y.Equals(other.Y)
                && this.Z.Equals(other.Z);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is UnitVector3 equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = this.X.GetHashCode();
            hashCode = CombineHashCodes(hashCode, this.Y.GetHashCode());
            return CombineHashCodes(hashCode, this.Z.GetHashCode());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"X:{this.X}, Y:{this.Y}, Z:{this.Z}";
        }
    }
}