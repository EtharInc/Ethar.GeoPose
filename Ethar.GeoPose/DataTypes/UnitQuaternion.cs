// <copyright file="UnitQuaternion.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A data type that represents a quaternion for serialization.
    /// </summary>
    public struct UnitQuaternion : IEquatable<UnitQuaternion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitQuaternion"/> struct.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="z">The z value.</param>
        /// <param name="w">The w value.</param>
        public UnitQuaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitQuaternion"/> struct from the given vector and rotation parts.
        /// </summary>
        /// <param name="vectorPart">The vector part of the Quaternion.</param>
        /// <param name="scalarPart">The rotation part of the Quaternion.</param>
        public UnitQuaternion(UnitVector3 vectorPart, float scalarPart)
        {
            this.X = vectorPart.X;
            this.Y = vectorPart.Y;
            this.Z = vectorPart.Z;
            this.W = scalarPart;
        }

        /// <summary>
        /// Gets a Quaternion representing no rotation.
        /// </summary>
        public static UnitQuaternion Identity
        {
            get { return new UnitQuaternion(0, 0, 0, 1); }
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
        /// Gets or sets the w value.
        /// </summary>
        [JsonProperty("w")]
        public float W { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="UnitQuaternion"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(UnitQuaternion a, UnitQuaternion b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="UnitQuaternion"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(UnitQuaternion a, UnitQuaternion b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(UnitQuaternion other)
        {
            return this.X.Equals(other.X)
                && this.Y.Equals(other.Y)
                && this.Z.Equals(other.Z)
                && this.W.Equals(other.W);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is UnitQuaternion equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.X.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Y.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Z.GetHashCode();
                hashcode = (hashcode * 397) ^ this.W.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"X:{this.X}, Y:{this.Y}, Z:{this.Z}, W:{this.W}";
        }
    }
}
