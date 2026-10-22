// <copyright file="CompassHeading.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Conventions
{
    /// <summary>
    /// Conversions between compass bearings and GeoPose Basic-YPR yaw.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A compass bearing is measured clockwise from true North: North 0°, East 90°, South 180°, West 270°.
    /// Device compasses, map bearings and most user interfaces use this convention.
    /// </para>
    /// <para>
    /// GeoPose Basic-YPR yaw (OGC GeoPose 1.0, /req/basic-ypr/angles) is a rotation of the LTP-ENU frame about its Up axis.
    /// ENU is right-handed, so positive yaw is counter-clockwise viewed from above, turning East towards North. The standard assigns
    /// pitch to the y axis and roll to the x axis, the aircraft convention in which x is the longitudinal (forward) axis, so the
    /// un-rotated inner frame faces East (its x axis) and a yaw of 90° faces North.
    /// </para>
    /// <para>
    /// The two conventions are related by <c>yaw = 90 - bearing</c>, which is its own inverse. The standard does not state the
    /// sign convention or the forward axis in words; the reading above follows from its axis assignment and from the right-handed
    /// frame, and is the reading these helpers implement.
    /// </para>
    /// </remarks>
    public static class CompassHeading
    {
        /// <summary>
        /// Converts a compass bearing to a GeoPose Basic-YPR yaw.
        /// </summary>
        /// <param name="bearingDegrees">The bearing in degrees, clockwise from true North.</param>
        /// <returns>The yaw in degrees, in the range [-180, 180), counter-clockwise from East.</returns>
        public static double ToGeoPoseYaw(double bearingDegrees) => Angles.Normalize180(90.0 - bearingDegrees);

        /// <summary>
        /// Converts a GeoPose Basic-YPR yaw to a compass bearing.
        /// </summary>
        /// <param name="yawDegrees">The yaw in degrees, counter-clockwise from East.</param>
        /// <returns>The bearing in degrees, clockwise from true North, in the range [0, 360).</returns>
        public static double ToCompassBearing(double yawDegrees) => Angles.Normalize360(90.0 - yawDegrees);

        /// <summary>
        /// Computes the heading offset of an engine frame: the compass bearing of the engine's +Z (forward) axis.
        /// </summary>
        /// <remarks>
        /// Engines such as Unity start their world frame at the device's initial pose, so the world +Z axis points wherever the device
        /// faced at start-up. Sampling the device's true heading together with the device's current yaw in the engine frame gives the
        /// bearing of the engine's +Z axis, which is the value <see cref="LeftHandedYUpConversions"/> and
        /// <see cref="LeftHandedYUpFrame"/> call the heading offset.
        /// </remarks>
        /// <param name="deviceTrueHeadingDegrees">The device's true heading in degrees, clockwise from true North.</param>
        /// <param name="deviceEngineYawDegrees">The device's yaw about the engine Up axis in degrees, positive clockwise viewed from above (the left-handed convention).</param>
        /// <returns>The heading offset in degrees, in the range [0, 360).</returns>
        public static double EngineHeadingOffset(double deviceTrueHeadingDegrees, double deviceEngineYawDegrees)
        {
            return Angles.Normalize360(deviceTrueHeadingDegrees - deviceEngineYawDegrees);
        }
    }
}
