// <copyright file="YawPitchRollAnglesExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.DataTypes;

    /// <summary>
    /// Extensions for the <see cref="YawPitchRollAngles"/> data type.
    /// </summary>
    public static class YawPitchRollAnglesExtensions
    {
        /// <summary>
        /// Builds a parameter string for a <see cref="YawPitchRollAngles"/>.
        /// </summary>
        /// <param name="ypr">The <see cref="YawPitchRollAngles"/> to generate the parameter string for.</param>
        /// <returns>A string representing the <see cref="YawPitchRollAngles"/>.</returns>
        public static string BuildOrientationParamString(this YawPitchRollAngles ypr) => $"orientation.yaw={ypr.Yaw}&orientation.pitch={ypr.Pitch}&orientation.roll={ypr.Roll}";
    }
}
