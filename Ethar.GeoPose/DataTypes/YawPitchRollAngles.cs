// <copyright file="YawPitchRollAngles.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Ethar.GeoPose.JsonConversion;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents yaw, pitch, and roll angles specified in decimal degrees.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Frame and order follow OGC GeoPose 1.0 (21-056r11) requirement /req/basic-ypr/angles: three consecutive rotations of a
    /// reference frame aligned East-North-Up (x East, y North, z Up, right-handed) about the local, already rotated, axes z (yaw),
    /// then y (pitch), then x (roll), in that order. All three are signed decimal degrees.
    /// </para>
    /// <para>
    /// The frame is right-handed, so a positive yaw is counter-clockwise viewed from above and turns East towards North. With pitch on y
    /// and roll on x the x axis is the reference (forward) direction: an all-zero orientation faces East and a yaw of 90° faces North.
    /// Compass bearings are clockwise from North, the opposite sense; convert with <see cref="Ethar.GeoPose.Conventions.CompassHeading"/>.
    /// Convert to a unit quaternion with <see cref="Ethar.GeoPose.Conventions.OrientationConversions.ToQuaternion(YawPitchRollAngles)"/>.
    /// </para>
    /// <para>
    /// Left-handed, Y-up engines such as Unity cannot use these values directly as Euler angles: the axes and the sense of rotation both
    /// differ. Map through <see cref="Ethar.GeoPose.Conventions.LeftHandedYUpFrame"/> instead.
    /// </para>
    /// <para>
    /// Requirements derived from figure 8 in section 7.2.1 of version 1.0 of the GeoPose standard https://docs.ogc.org/is/21-056r11/21-056r11.html.
    /// </para>
    /// </remarks>
    public struct YawPitchRollAngles : IEquatable<YawPitchRollAngles>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YawPitchRollAngles"/> struct.
        /// </summary>
        /// <param name="yaw">The yaw in decimal degrees.</param>
        /// <param name="pitch">The pitch in decimal degrees.</param>
        /// <param name="roll">The roll in decimal degrees.</param>
        public YawPitchRollAngles(double yaw, double pitch, double roll)
        {
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.Roll = roll;
        }

        /// <summary>
        /// Gets or sets yaw specified in decimal degrees.
        /// </summary>
        [JsonProperty("yaw")]
        public double Yaw { get; set; }

        /// <summary>
        /// Gets or sets pitch specified in decimal degrees.
        /// </summary>
        [JsonProperty("pitch")]
        public double Pitch { get; set; }

        /// <summary>
        /// Gets or sets roll specified in decimal degrees.
        /// </summary>
        [JsonProperty("roll")]
        public double Roll { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="YawPitchRollAngles"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(YawPitchRollAngles a, YawPitchRollAngles b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="YawPitchRollAngles"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(YawPitchRollAngles a, YawPitchRollAngles b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(YawPitchRollAngles other)
        {
            return this.Yaw.Equals(other.Yaw)
                && this.Pitch.Equals(other.Pitch)
                && this.Roll.Equals(other.Roll);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is YawPitchRollAngles equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Yaw.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Pitch.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Roll.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Yaw:{InvariantNumber.Format(this.Yaw)}, Pitch:{InvariantNumber.Format(this.Pitch)}, Roll:{InvariantNumber.Format(this.Roll)}";
        }
    }
}
