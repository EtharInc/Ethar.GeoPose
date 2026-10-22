// <copyright file="EulerConvention.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Conventions
{
    using System;

    /// <summary>
    /// The unit an angle is expressed in.
    /// </summary>
    public enum AngleUnit
    {
        /// <summary>
        /// Decimal degrees, the unit the GeoPose standard requires.
        /// </summary>
        Degrees = 0,

        /// <summary>
        /// Radians.
        /// </summary>
        Radians = 1,
    }

    /// <summary>
    /// An axis of the frame a rotation is applied about.
    /// </summary>
    public enum RotationAxis
    {
        /// <summary>
        /// The x axis (East in LTP-ENU).
        /// </summary>
        X = 0,

        /// <summary>
        /// The y axis (North in LTP-ENU).
        /// </summary>
        Y = 1,

        /// <summary>
        /// The z axis (Up in LTP-ENU).
        /// </summary>
        Z = 2,
    }

    /// <summary>
    /// Describes how a yaw, pitch, roll triple is turned into a rotation: which axis each angle rotates about, whether the rotations are
    /// intrinsic (about the already rotated local axes) or extrinsic (about the fixed frame axes), and the angle unit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The three axes must be distinct (a Tait-Bryan sequence). <see cref="GeoPoseStandard"/> is what OGC GeoPose 1.0 requires and what the
    /// OGC GeoPoseSandbox implements; use it unless you are exchanging data with a source known to use something else.
    /// </para>
    /// <para>
    /// <see cref="OgcInstanceFiles"/> exists because the Basic-YPR and Basic-Quaternion example files published at
    /// schemas.opengis.net/geopose/1.0/instances describe the same pose only when the angles are read as radians with pitch about the x axis.
    /// That is not the standard's text; it is offered so that data produced the same way can be converted correctly and so the discrepancy
    /// is reproducible in tests.
    /// </para>
    /// </remarks>
    public struct EulerConvention : IEquatable<EulerConvention>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EulerConvention"/> struct.
        /// </summary>
        /// <param name="yawAxis">The axis the first rotation (yaw) is about.</param>
        /// <param name="pitchAxis">The axis the second rotation (pitch) is about.</param>
        /// <param name="rollAxis">The axis the third rotation (roll) is about.</param>
        /// <param name="unit">The unit of all three angles.</param>
        /// <param name="isIntrinsic">True for intrinsic rotations (about the rotated local axes), false for extrinsic (about the fixed frame axes).</param>
        /// <exception cref="ArgumentException">Thrown if the three axes are not distinct.</exception>
        public EulerConvention(RotationAxis yawAxis, RotationAxis pitchAxis, RotationAxis rollAxis, AngleUnit unit, bool isIntrinsic = true)
        {
            if (yawAxis == pitchAxis || pitchAxis == rollAxis || yawAxis == rollAxis)
            {
                throw new ArgumentException("The yaw, pitch and roll axes must be three distinct axes.");
            }

            this.YawAxis = yawAxis;
            this.PitchAxis = pitchAxis;
            this.RollAxis = rollAxis;
            this.Unit = unit;
            this.IsIntrinsic = isIntrinsic;
        }

        /// <summary>
        /// Gets the convention required by OGC GeoPose 1.0 /req/basic-ypr/angles and implemented by the OGC GeoPoseSandbox:
        /// intrinsic rotations about local z (yaw), then y (pitch), then x (roll), in degrees.
        /// </summary>
        public static EulerConvention GeoPoseStandard => new EulerConvention(RotationAxis.Z, RotationAxis.Y, RotationAxis.X, AngleUnit.Degrees, true);

        /// <summary>
        /// Gets the convention that reproduces the OGC example instance files: intrinsic rotations about local z (yaw), then x (pitch),
        /// then y (roll), in radians. See the type remarks.
        /// </summary>
        public static EulerConvention OgcInstanceFiles => new EulerConvention(RotationAxis.Z, RotationAxis.X, RotationAxis.Y, AngleUnit.Radians, true);

        /// <summary>
        /// Gets the axis the first rotation (yaw) is about.
        /// </summary>
        public RotationAxis YawAxis { get; }

        /// <summary>
        /// Gets the axis the second rotation (pitch) is about.
        /// </summary>
        public RotationAxis PitchAxis { get; }

        /// <summary>
        /// Gets the axis the third rotation (roll) is about.
        /// </summary>
        public RotationAxis RollAxis { get; }

        /// <summary>
        /// Gets the unit of all three angles.
        /// </summary>
        public AngleUnit Unit { get; }

        /// <summary>
        /// Gets a value indicating whether the rotations are intrinsic (about the rotated local axes) rather than extrinsic.
        /// </summary>
        public bool IsIntrinsic { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="EulerConvention"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator ==(EulerConvention a, EulerConvention b) => a.Equals(b);

        /// <summary>
        /// Performs an inequality comparison on two objects of type <see cref="EulerConvention"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are not equal.</returns>
        public static bool operator !=(EulerConvention a, EulerConvention b) => !(a == b);

        /// <summary>
        /// Converts an angle in this convention's unit to degrees.
        /// </summary>
        /// <param name="value">The angle in this convention's unit.</param>
        /// <returns>The angle in degrees.</returns>
        public double ToDegrees(double value) => this.Unit == AngleUnit.Radians ? Angles.RadiansToDegrees(value) : value;

        /// <summary>
        /// Converts an angle in degrees to this convention's unit.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in this convention's unit.</returns>
        public double FromDegrees(double degrees) => this.Unit == AngleUnit.Radians ? Angles.DegreesToRadians(degrees) : degrees;

        /// <inheritdoc/>
        public bool Equals(EulerConvention other)
        {
            return this.YawAxis == other.YawAxis
                && this.PitchAxis == other.PitchAxis
                && this.RollAxis == other.RollAxis
                && this.Unit == other.Unit
                && this.IsIntrinsic == other.IsIntrinsic;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is EulerConvention equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = (int)this.YawAxis;
                hashcode = (hashcode * 397) ^ (int)this.PitchAxis;
                hashcode = (hashcode * 397) ^ (int)this.RollAxis;
                hashcode = (hashcode * 397) ^ (int)this.Unit;
                hashcode = (hashcode * 397) ^ (this.IsIntrinsic ? 1 : 0);
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{(this.IsIntrinsic ? "Intrinsic" : "Extrinsic")} {this.YawAxis}-{this.PitchAxis}-{this.RollAxis} {this.Unit}";
        }
    }
}
