// <copyright file="LeftHandedYUpConversions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Conventions
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Geodesy;

    /// <summary>
    /// Conversions between the right-handed LTP-ENU frame and a left-handed, Y-up engine frame (Unity's world space), with a heading offset.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The axis mapping is East to +X, Up to +Y and North to +Z. Because the two frames have opposite handedness, this swap alone is
    /// not enough for rotations: a rotation by θ about an ENU axis becomes a rotation by -θ about the mapped axis. In quaternion terms an
    /// ENU quaternion (x, y, z, w) becomes (-x, -z, -y, w) in the engine frame.
    /// </para>
    /// <para>
    /// The heading offset is the compass bearing of the engine's +Z axis, in degrees, clockwise from true North. Engines that start their
    /// world frame at the device's initial pose have an arbitrary heading offset; a QR code or tracked image with a known GeoPose gives a
    /// measured one. True North lies at engine yaw <c>-heading offset</c>. Every ENU displacement and every ENU rotation is therefore
    /// turned by <c>-heading offset</c> about the engine's Y axis after the axis mapping, and by <c>+heading offset</c> before mapping back.
    /// Use <see cref="CompassHeading.EngineHeadingOffset"/> to obtain the offset from a device compass reading.
    /// </para>
    /// </remarks>
    public static class LeftHandedYUpConversions
    {
        /// <summary>
        /// Maps an ENU displacement to the engine frame.
        /// </summary>
        /// <param name="displacement">The East, North, Up displacement in meters.</param>
        /// <param name="headingOffsetDegrees">The compass bearing of the engine's +Z axis in degrees.</param>
        /// <returns>The displacement in the engine frame.</returns>
        public static LeftHandedYUpVector ToLeftHandedYUp(this EnuVector displacement, double headingOffsetDegrees)
        {
            var theta = Angles.DegreesToRadians(headingOffsetDegrees);
            var cos = Math.Cos(theta);
            var sin = Math.Sin(theta);

            // Axis map (E, N, U) -> (x, z, y), then turn by -offset about Y (clockwise-positive frame).
            return new LeftHandedYUpVector(
                (displacement.East * cos) - (displacement.North * sin),
                displacement.Up,
                (displacement.East * sin) + (displacement.North * cos));
        }

        /// <summary>
        /// Maps an engine-frame displacement back to ENU.
        /// </summary>
        /// <param name="displacement">The displacement in the engine frame.</param>
        /// <param name="headingOffsetDegrees">The compass bearing of the engine's +Z axis in degrees.</param>
        /// <returns>The East, North, Up displacement in meters.</returns>
        public static EnuVector ToEnu(this LeftHandedYUpVector displacement, double headingOffsetDegrees)
        {
            var theta = Angles.DegreesToRadians(headingOffsetDegrees);
            var cos = Math.Cos(theta);
            var sin = Math.Sin(theta);

            return new EnuVector(
                (displacement.X * cos) + (displacement.Z * sin),
                (-displacement.X * sin) + (displacement.Z * cos),
                displacement.Y);
        }

        /// <summary>
        /// Maps a rotation expressed in the ENU frame to the engine frame.
        /// </summary>
        /// <param name="enuRotation">The unit quaternion in the ENU frame, rotating inner-frame vectors into ENU.</param>
        /// <param name="headingOffsetDegrees">The compass bearing of the engine's +Z axis in degrees.</param>
        /// <returns>The unit quaternion in the engine frame, rotating the object's local vectors into engine world space.</returns>
        public static UnitQuaternion ToLeftHandedYUp(this UnitQuaternion enuRotation, double headingOffsetDegrees)
        {
            var mapped = new UnitQuaternion(-enuRotation.X, -enuRotation.Z, -enuRotation.Y, enuRotation.W);
            var turn = OrientationConversions.FromAxisAngle(UnitVector3.UnitY, -headingOffsetDegrees);
            return turn.Multiply(mapped);
        }

        /// <summary>
        /// Maps a rotation expressed in the engine frame back to the ENU frame.
        /// </summary>
        /// <param name="engineRotation">The unit quaternion in the engine frame.</param>
        /// <param name="headingOffsetDegrees">The compass bearing of the engine's +Z axis in degrees.</param>
        /// <returns>The unit quaternion in the ENU frame.</returns>
        public static UnitQuaternion ToEnu(this UnitQuaternion engineRotation, double headingOffsetDegrees)
        {
            var turn = OrientationConversions.FromAxisAngle(UnitVector3.UnitY, headingOffsetDegrees);
            var unturned = turn.Multiply(engineRotation);
            return new UnitQuaternion(-unturned.X, -unturned.Z, -unturned.Y, unturned.W);
        }
    }
}
