// <copyright file="TangentPointPositionExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using System;
    using System.Text;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.JsonConversion;
    using Ethar.GeoPose.Validation;

    /// <summary>
    /// Extensions for <see cref="TangentPointPosition"/>.
    /// </summary>
    public static class TangentPointPositionExtensions
    {
        /// <summary>
        /// Validates that the latitude and longitude are within range.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The validation result.</returns>
        public static FrameSpecificationValidationResult Validate(this TangentPointPosition position)
        {
            var isValid = true;
            var message = new StringBuilder();

            if (position.Latitude < -90 || position.Latitude > 90)
            {
                isValid = false;
                message.Append($"Latitude value {InvariantNumber.Format(position.Latitude)} is outside of the valid range.{Environment.NewLine}");
            }

            if (position.Longitude < -180 || position.Longitude > 180)
            {
                isValid = false;
                message.Append($"Longitude value {InvariantNumber.Format(position.Longitude)} is outside of the valid range.");
            }

            return isValid ? FrameSpecificationValidationResult.Valid : new FrameSpecificationValidationResult(isValid, message.ToString());
        }

        /// <summary>
        /// Builds the Ethar authority parameter string for the position, culture-invariant:
        /// <c>latitude=…&amp;longitude=…&amp;heightInMeters=…</c>.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The parameter string.</returns>
        public static string BuildParamString(this TangentPointPosition position)
        {
            return $"latitude={InvariantNumber.Format(position.Latitude)}&longitude={InvariantNumber.Format(position.Longitude)}&heightInMeters={InvariantNumber.Format(position.HeightInMeters)}";
        }
    }
}
