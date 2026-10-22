// <copyright file="YawPitchRollAnglesExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// Extensions for <see cref="YawPitchRollAngles"/>.
    /// </summary>
    public static class YawPitchRollAnglesExtensions
    {
        /// <summary>
        /// Builds the Ethar authority orientation parameter string, culture-invariant: <c>orientation.yaw=…&amp;orientation.pitch=…&amp;orientation.roll=…</c>.
        /// </summary>
        /// <param name="ypr">The angles.</param>
        /// <returns>The parameter string.</returns>
        public static string BuildOrientationParamString(this YawPitchRollAngles ypr)
        {
            return $"orientation.yaw={InvariantNumber.Format(ypr.Yaw)}&orientation.pitch={InvariantNumber.Format(ypr.Pitch)}&orientation.roll={InvariantNumber.Format(ypr.Roll)}";
        }
    }
}
