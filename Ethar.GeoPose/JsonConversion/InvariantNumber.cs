// <copyright file="InvariantNumber.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.JsonConversion
{
    using System;
    using System.Globalization;
    using Ethar.GeoPose.Exceptions;

    /// <summary>
    /// Culture-invariant formatting and parsing of the numbers that appear in frame specification parameter strings.
    /// </summary>
    /// <remarks>
    /// Parameter strings travel between devices, so they must use the JSON number format (period decimal separator, no grouping)
    /// regardless of the current thread culture. A device set to a German, French or most other continental European locales would
    /// otherwise write <c>latitude=48,85</c> and read <c>48.85</c> as 4885.
    /// </remarks>
    public static class InvariantNumber
    {
        private const NumberStyles ParseStyles = NumberStyles.Float;

        /// <summary>
        /// Formats a number in round-trip, culture-invariant form.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The invariant text, for example <c>-122.3</c>.</returns>
        public static string Format(double value) => value.ToString("R", CultureInfo.InvariantCulture);

        /// <summary>
        /// Parses a culture-invariant number.
        /// </summary>
        /// <param name="text">The text, for example <c>-122.3000000</c>.</param>
        /// <param name="parameterName">The parameter name, used in the error message.</param>
        /// <returns>The parsed value.</returns>
        /// <exception cref="FrameSpecificationInvalidException">Thrown if the text is not an invariant number.</exception>
        public static double Parse(string text, string parameterName)
        {
            if (text != null && double.TryParse(text.Trim(), ParseStyles, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            throw new FrameSpecificationInvalidException($"Parameter {parameterName} has the value '{text}', which is not a number. Values must use a period as the decimal separator.");
        }

        /// <summary>
        /// Formats a list of numbers in the bracketed array form used by the OGC instance files, for example <c>[0.0, 0.0, 0.0]</c>.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>The invariant array text.</returns>
        public static string FormatArray(params double[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var parts = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                parts[i] = Format(values[i]);
            }

            return "[" + string.Join(", ", parts) + "]";
        }

        /// <summary>
        /// Parses a bracketed array of culture-invariant numbers, for example <c>[0.0, 0.0, 0.0]</c>. Brackets are optional.
        /// </summary>
        /// <param name="text">The array text.</param>
        /// <param name="parameterName">The parameter name, used in the error message.</param>
        /// <param name="expectedLength">The number of elements the array must contain.</param>
        /// <returns>The parsed values.</returns>
        /// <exception cref="FrameSpecificationInvalidException">Thrown if the text is not an array of the expected length.</exception>
        public static double[] ParseArray(string text, string parameterName, int expectedLength)
        {
            if (text == null)
            {
                throw new FrameSpecificationInvalidException($"Parameter {parameterName} is missing.");
            }

            var trimmed = text.Trim().TrimStart('[').TrimEnd(']');
            var parts = trimmed.Split(',');
            if (parts.Length != expectedLength)
            {
                throw new FrameSpecificationInvalidException($"Parameter {parameterName} must contain {expectedLength} numbers but has {parts.Length}: '{text}'.");
            }

            var values = new double[expectedLength];
            for (var i = 0; i < expectedLength; i++)
            {
                values[i] = Parse(parts[i], parameterName);
            }

            return values;
        }
    }
}
