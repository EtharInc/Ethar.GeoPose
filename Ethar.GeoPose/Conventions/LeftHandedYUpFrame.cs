// <copyright file="LeftHandedYUpFrame.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Conventions
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Geodesy;
    using Ethar.GeoPose.JsonConversion;
    using Ethar.GeoPose.StructuralDataUnits;

    /// <summary>
    /// The mapping between a left-handed, Y-up engine world frame and the Earth: a geodetic origin, the engine position of that origin,
    /// and the heading offset of the engine frame. Converts GeoPoses to engine poses and back.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the double precision, engine-independent replacement for the origin mapping carried by the Unity samples. An origin manager
    /// (GPS, QR code, tracked image) measures the three values; everything else is derived here so that every consumer applies the same
    /// axis mapping and the same heading sign.
    /// </para>
    /// <para>
    /// The heading offset is the compass bearing of the engine's +Z axis. For a device-anchored engine frame it is
    /// <c>CompassHeading.EngineHeadingOffset(trueHeading, deviceEngineYaw)</c>. For a QR code or image whose GeoPose yaw is known, it is the
    /// bearing that GeoPose faces minus the engine yaw of the anchor: <c>CompassHeading.ToCompassBearing(poseYaw) - anchorEngineYaw</c>.
    /// </para>
    /// </remarks>
    public struct LeftHandedYUpFrame : IEquatable<LeftHandedYUpFrame>
    {
        private static readonly UnitQuaternion EngineZForwardCorrection = OrientationConversions.FromAxisAngle(UnitVector3.UnitY, 90.0);

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftHandedYUpFrame"/> struct.
        /// </summary>
        /// <param name="origin">The geodetic position of the frame origin (WGS 84, ellipsoidal height).</param>
        /// <param name="engineOrigin">The engine world position that coincides with <paramref name="origin"/>.</param>
        /// <param name="headingOffsetDegrees">The compass bearing of the engine's +Z axis in degrees, clockwise from true North.</param>
        public LeftHandedYUpFrame(TangentPointPosition origin, LeftHandedYUpVector engineOrigin, double headingOffsetDegrees)
        {
            this.Origin = origin;
            this.EngineOrigin = engineOrigin;
            this.HeadingOffsetDegrees = Angles.Normalize360(headingOffsetDegrees);
        }

        /// <summary>
        /// Gets the geodetic position of the frame origin.
        /// </summary>
        public TangentPointPosition Origin { get; }

        /// <summary>
        /// Gets the engine world position that coincides with <see cref="Origin"/>.
        /// </summary>
        public LeftHandedYUpVector EngineOrigin { get; }

        /// <summary>
        /// Gets the compass bearing of the engine's +Z axis in degrees, in the range [0, 360).
        /// </summary>
        public double HeadingOffsetDegrees { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="LeftHandedYUpFrame"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator ==(LeftHandedYUpFrame a, LeftHandedYUpFrame b) => a.Equals(b);

        /// <summary>
        /// Performs an inequality comparison on two objects of type <see cref="LeftHandedYUpFrame"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are not equal.</returns>
        public static bool operator !=(LeftHandedYUpFrame a, LeftHandedYUpFrame b) => !(a == b);

        /// <summary>
        /// Maps a geodetic position to an engine world position.
        /// </summary>
        /// <param name="position">The geodetic position (WGS 84, ellipsoidal height).</param>
        /// <returns>The engine world position in meters.</returns>
        public LeftHandedYUpVector ToEngine(TangentPointPosition position)
        {
            var enu = GeodeticConverter.GeodeticToEnu(position, this.Origin);
            return this.EngineOrigin + enu.ToLeftHandedYUp(this.HeadingOffsetDegrees);
        }

        /// <summary>
        /// Maps a GeoPose orientation in the ENU frame to an engine world rotation.
        /// </summary>
        /// <param name="enuRotation">The unit quaternion in the ENU frame.</param>
        /// <param name="forwardAxis">Which local axis should face the GeoPose reference direction.</param>
        /// <returns>The engine world rotation.</returns>
        public UnitQuaternion ToEngine(UnitQuaternion enuRotation, ForwardAxis forwardAxis = ForwardAxis.GeoPoseX)
        {
            var engine = enuRotation.ToLeftHandedYUp(this.HeadingOffsetDegrees);
            return forwardAxis == ForwardAxis.EngineZ ? engine.Multiply(EngineZForwardCorrection) : engine;
        }

        /// <summary>
        /// Maps a Basic-YPR GeoPose to an engine pose.
        /// </summary>
        /// <param name="sdu">The Basic-YPR structural data unit.</param>
        /// <param name="forwardAxis">Which local axis should face the GeoPose reference direction.</param>
        /// <returns>The engine pose.</returns>
        public EnginePose ToEngine(BasicYawPitchRollSdu sdu, ForwardAxis forwardAxis = ForwardAxis.GeoPoseX)
        {
            return new EnginePose(this.ToEngine(sdu.Position), this.ToEngine(sdu.Angles.ToQuaternion(), forwardAxis));
        }

        /// <summary>
        /// Maps a Basic-Quaternion GeoPose to an engine pose.
        /// </summary>
        /// <param name="sdu">The Basic-Quaternion structural data unit.</param>
        /// <param name="forwardAxis">Which local axis should face the GeoPose reference direction.</param>
        /// <returns>The engine pose.</returns>
        public EnginePose ToEngine(BasicQuaternionSdu sdu, ForwardAxis forwardAxis = ForwardAxis.GeoPoseX)
        {
            return new EnginePose(this.ToEngine(sdu.Position), this.ToEngine(sdu.Quaternion, forwardAxis));
        }

        /// <summary>
        /// Maps an engine world position to a geodetic position.
        /// </summary>
        /// <param name="enginePosition">The engine world position in meters.</param>
        /// <returns>The geodetic position (WGS 84, ellipsoidal height).</returns>
        public TangentPointPosition ToGeodetic(LeftHandedYUpVector enginePosition)
        {
            var enu = (enginePosition - this.EngineOrigin).ToEnu(this.HeadingOffsetDegrees);
            return GeodeticConverter.EnuToGeodetic(enu, this.Origin);
        }

        /// <summary>
        /// Maps an engine world rotation to a GeoPose orientation in the ENU frame.
        /// </summary>
        /// <param name="engineRotation">The engine world rotation.</param>
        /// <param name="forwardAxis">The forward axis convention the engine rotation was built with.</param>
        /// <returns>The unit quaternion in the ENU frame.</returns>
        public UnitQuaternion ToEnuRotation(UnitQuaternion engineRotation, ForwardAxis forwardAxis = ForwardAxis.GeoPoseX)
        {
            var rotation = forwardAxis == ForwardAxis.EngineZ ? engineRotation.Multiply(EngineZForwardCorrection.Conjugate()) : engineRotation;
            return rotation.ToEnu(this.HeadingOffsetDegrees);
        }

        /// <summary>
        /// Maps an engine pose to a Basic-YPR GeoPose.
        /// </summary>
        /// <param name="pose">The engine pose.</param>
        /// <param name="forwardAxis">The forward axis convention the engine rotation was built with.</param>
        /// <returns>The Basic-YPR structural data unit.</returns>
        public BasicYawPitchRollSdu ToBasicYawPitchRoll(EnginePose pose, ForwardAxis forwardAxis = ForwardAxis.GeoPoseX)
        {
            return new BasicYawPitchRollSdu(this.ToEnuRotation(pose.Rotation, forwardAxis).ToYawPitchRoll(), this.ToGeodetic(pose.Position));
        }

        /// <summary>
        /// Maps an engine pose to a Basic-Quaternion GeoPose.
        /// </summary>
        /// <param name="pose">The engine pose.</param>
        /// <param name="forwardAxis">The forward axis convention the engine rotation was built with.</param>
        /// <returns>The Basic-Quaternion structural data unit.</returns>
        public BasicQuaternionSdu ToBasicQuaternion(EnginePose pose, ForwardAxis forwardAxis = ForwardAxis.GeoPoseX)
        {
            return new BasicQuaternionSdu(this.ToGeodetic(pose.Position), this.ToEnuRotation(pose.Rotation, forwardAxis));
        }

        /// <inheritdoc/>
        public bool Equals(LeftHandedYUpFrame other)
        {
            return this.Origin.Equals(other.Origin)
                && this.EngineOrigin.Equals(other.EngineOrigin)
                && this.HeadingOffsetDegrees.Equals(other.HeadingOffsetDegrees);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is LeftHandedYUpFrame equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Origin.GetHashCode();
                hashcode = (hashcode * 397) ^ this.EngineOrigin.GetHashCode();
                hashcode = (hashcode * 397) ^ this.HeadingOffsetDegrees.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Origin:[{this.Origin}], EngineOrigin:[{this.EngineOrigin}], HeadingOffsetDegrees:{InvariantNumber.Format(this.HeadingOffsetDegrees)}";
        }
    }
}
