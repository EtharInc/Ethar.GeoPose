// <copyright file="ParamStringBuilder.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.JsonConversion
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;

    /// <summary>
    /// Builds a query-string style parameter string from the JSON-annotated properties of an object, culture-invariant.
    /// </summary>
    public class ParamStringBuilder
    {
        /// <summary>
        /// Builds a parameter string of the form <c>name=value&amp;name=value</c> from every property carrying a <see cref="JsonPropertyAttribute"/>.
        /// Nested objects are flattened. Numbers are formatted with the invariant culture.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>The parameter string.</returns>
        public static string BuildParamString<T>(T obj)
        {
            var props = typeof(T)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null)
                .Select(p =>
                {
                    if (p.PropertyType.IsPrimitive || p.PropertyType == typeof(string) || p.PropertyType == typeof(decimal))
                    {
                        return $"{p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName}={FormatValue(p.GetValue(obj))}";
                    }
                    else
                    {
                        return BuildParamString(p.GetValue(obj));
                    }
                }).ToList();

            var paramString = string.Join("&", props);
            return paramString;
        }

        private static string FormatValue(object value)
        {
            switch (value)
            {
                case null:
                    return string.Empty;
                case double d:
                    return InvariantNumber.Format(d);
                case float f:
                    return f.ToString("R", CultureInfo.InvariantCulture);
                case IFormattable formattable:
                    return formattable.ToString(null, CultureInfo.InvariantCulture);
                default:
                    return value.ToString();
            }
        }
    }
}
