// <copyright file="ParamStringBuilder.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.JsonConversion
{
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;

    /// <summary>
    /// A utility class that is used to build generic parameter strings.
    /// </summary>
    public class ParamStringBuilder
    {
        /// <summary>
        /// Builds a parameter string for the object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object to build a parameter string for.</param>
        /// <returns>A parameter string representation of the object.</returns>
        /// <remarks>
        /// This works by using reflection to determine which properties have the <see cref="JsonPropertyAttribute"/> and generating a parameter
        /// string from those properties.
        /// </remarks>
        /// <example>
        /// Assume a class with the following member.
        /// <code>
        /// class TestClass
        /// {
        ///     [JsonProperty("p1")]
        ///     int Param1
        /// }
        /// </code>
        /// This method would generate the parameter string: "p1={value of Param1}".
        /// </example>
        public static string BuildParamString<T>(T obj)
        {
            var props = typeof(T)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null)
                .Select(p =>
                {
                    if (p.PropertyType.IsPrimitive || p.PropertyType == typeof(string))
                    {
                        return $"{p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName}={p.GetValue(obj)}";
                    }
                    else
                    {
                        return BuildParamString(p.GetValue(obj));
                    }
                }).ToList();
            var paramString = string.Join("&", props);
            return paramString;
        }
    }
}
