// <copyright file="UnitVector3Extensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// Extensions for <see cref="UnitVector3"/>.
    /// </summary>
    public static class UnitVector3Extensions
    {
        /// <summary>
        /// Builds the Ethar authority translation parameter string, culture-invariant: <c>translation.x=…&amp;translation.y=…&amp;translation.z=…</c>.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The parameter string.</returns>
        public static string BuildTranslationParamString(this UnitVector3 vector)
        {
            return $"translation.x={InvariantNumber.Format(vector.X)}&translation.y={InvariantNumber.Format(vector.Y)}&translation.z={InvariantNumber.Format(vector.Z)}";
        }
    }
}
