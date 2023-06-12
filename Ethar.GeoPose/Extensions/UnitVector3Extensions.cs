// <copyright file="UnitVector3Extensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.DataTypes;

    /// <summary>
    /// Extensions for the <see cref="UnitVector3"/> data type.
    /// </summary>
    public static class UnitVector3Extensions
    {
        /// <summary>
        /// Builds a parameter string for the <see cref="UnitVector3"/>.
        /// </summary>
        /// <param name="vector">The <see cref="UnitVector3"/> to generate a parameter string for.</param>
        /// <returns>A string representation of the <see cref="UnitVector3"/>.</returns>
        /// <remarks>Convenience method used to generate parameter strings for frame specifications in the /Ethar.GeoPose/1.0 authority.</remarks>
        public static string BuildTranslationParamString(this UnitVector3 vector) => $"translation.x={vector.X}&translation.y={vector.Y}&translation.z={vector.Z}";
    }
}
