// <copyright file="UnitQuaternionExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// Extensions for <see cref="UnitQuaternion"/>.
    /// </summary>
    public static class UnitQuaternionExtensions
    {
        /// <summary>
        /// Builds the Ethar authority rotation parameter string, culture-invariant: <c>rotation.x=…&amp;rotation.y=…&amp;rotation.z=…&amp;rotation.w=…</c>.
        /// </summary>
        /// <param name="quat">The quaternion.</param>
        /// <returns>The parameter string.</returns>
        public static string BuildRotationParamString(this UnitQuaternion quat)
        {
            return $"rotation.x={InvariantNumber.Format(quat.X)}&rotation.y={InvariantNumber.Format(quat.Y)}&rotation.z={InvariantNumber.Format(quat.Z)}&rotation.w={InvariantNumber.Format(quat.W)}";
        }

        /// <summary>
        /// Builds the Ethar authority orientation parameter string, culture-invariant: <c>orientation.x=…&amp;orientation.y=…&amp;orientation.z=…&amp;orientation.w=…</c>.
        /// </summary>
        /// <param name="quat">The quaternion.</param>
        /// <returns>The parameter string.</returns>
        public static string BuildOrientationParamString(this UnitQuaternion quat)
        {
            return $"orientation.x={InvariantNumber.Format(quat.X)}&orientation.y={InvariantNumber.Format(quat.Y)}&orientation.z={InvariantNumber.Format(quat.Z)}&orientation.w={InvariantNumber.Format(quat.W)}";
        }
    }
}
