// <copyright file="UnitQuaternion.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Ethar.GeoPose.JsonConversion;
    using Newtonsoft.Json;

    /// <summary>
    /// A data type that represents a quaternion for serialization.
    /// </summary>
    /// <remarks>
    /// <para>
    /// OGC GeoPose 1.0 (21-056r11) requirement /req/basic-quaternion/quaternion: a rotation-only transformation of a reference frame
    /// aligned East-North-Up, as a unit quaternion whose components' squares sum to one. The standard lists the components as w, x, y, z;
    /// the JSON encoding and this type carry them as x, y, z, w.
    /// </para>
    /// <para>
    /// The quaternion rotates a vector expressed in the inner (posed) frame into the ENU frame, <c>v_enu = q · v_inner · q*</c>,
    /// in the Hamilton convention used by <see cref="Ethar.GeoPose.Conventions.OrientationConversions"/>. The standard does not name an
    /// active or passive convention; this is the reading the library implements and tests. Use
    /// <see cref="Ethar.GeoPose.Conventions.OrientationConversions.IsUnit"/> to validate incoming data and
    /// <see cref="Ethar.GeoPose.Conventions.LeftHandedYUpConversions"/> to map to a left-handed, Y-up engine frame.
    /// </para>
    /// </remarks>
    public struct UnitQuaternion : IEquatable<UnitQuaternion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitQuaternion"/> struct.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="z">The z value.</param>
        /// <param name="w">The w value.</param>
        public UnitQuaternion(double x, double y, double z, double w)
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
        public UnitQuaternion(UnitVector3 vectorPart, double scalarPart)
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
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y value.
        /// </summary>
        [JsonProperty("y")]
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the z value.
        /// </summary>
        [JsonProperty("z")]
        public double Z { get; set; }

        /// <summary>
        /// Gets or sets the w value.
        /// </summary>
        [JsonProperty("w")]
        public double W { get; set; }

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
            return $"X:{InvariantNumber.Format(this.X)}, Y:{InvariantNumber.Format(this.Y)}, Z:{InvariantNumber.Format(this.Z)}, W:{InvariantNumber.Format(this.W)}";
        }
    }
}
