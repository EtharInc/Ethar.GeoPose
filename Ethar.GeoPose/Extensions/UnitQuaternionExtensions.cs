// <copyright file="UnitQuaternionExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.DataTypes;

    /// <summary>
    /// Extensions for the <see cref="UnitQuaternion"/> data type.
    /// </summary>
    public static class UnitQuaternionExtensions
    {
        /// <summary>
        /// Builds a parameter string for a <see cref="UnitQuaternion"/>. The string will have "rotation" prepended to the members.
        /// </summary>
        /// <param name="quat">The <see cref="UnitQuaternion"/> to generate the parameter string for.</param>
        /// <returns>A string representing the <see cref="UnitQuaternion"/>.</returns>
        public static string BuildRotationParamString(this UnitQuaternion quat) => $"rotation.x={quat.X}&rotation.y={quat.Y}&rotation.z={quat.Z}&rotation.w={quat.W}";

        /// <summary>
        /// Builds a parameter string for a <see cref="UnitQuaternion"/>. The string will have "orientation" prepended to the members.
        /// </summary>
        /// <param name="quat">The <see cref="UnitQuaternion"/> to generate the parameter string for.</param>
        /// <returns>A string representing the <see cref="UnitQuaternion"/>.</returns>
        public static string BuildOrientationParamString(this UnitQuaternion quat) => $"orientation.x={quat.X}&orientation.y={quat.Y}&orientation.z={quat.Z}&orientation.w={quat.W}";
    }
}
