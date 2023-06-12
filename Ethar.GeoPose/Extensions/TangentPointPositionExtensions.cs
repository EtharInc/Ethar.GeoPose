// <copyright file="TangentPointPositionExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using System;
    using System.Text;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Validation;

    /// <summary>
    /// Extensions for the <see cref="TangentPointPosition"/> struct.
    /// </summary>
    public static class TangentPointPositionExtensions
    {
        /// <summary>
        /// Validates that the <see cref="TangentPointPosition"/> contains a valid latitude and longitude.
        /// </summary>
        /// <param name="position">The position to validate.</param>
        /// <returns>A <see cref="FrameSpecificationValidationResult"/> with information on the validity of the <see cref="TangentPointPosition"/>.</returns>
        public static FrameSpecificationValidationResult Validate(this TangentPointPosition position)
        {
            var isValid = true;
            var message = new StringBuilder();

            if (position.Latitude < -90 || position.Latitude > 90)
            {
                isValid = false;
                message.Append($"Latitude value {position.Latitude} is outside of the valid range.{Environment.NewLine}");
            }

            if (position.Longitude < -180 || position.Longitude > 180)
            {
                isValid = false;
                message.Append($"Longitude value {position.Longitude} is outside of the valid range.");
            }

            return isValid ? FrameSpecificationValidationResult.Valid : new FrameSpecificationValidationResult(isValid, message.ToString());
        }

        /// <summary>
        /// Builds a parameter string for various frame specification types.
        /// </summary>
        /// <param name="position">The position to build the parameter string for.</param>
        /// <returns>A parameter string representation of the <see cref="TangentPointPosition"/>.</returns>
        public static string BuildParamString(this TangentPointPosition position) => $"latitude={position.Latitude}&longitude={position.Longitude}&heightInMeters={position.HeightInMeters}";
    }
}
