// <copyright file="IEnumerableTExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Extensions for IEnumerable<![CDATA[<T>]]> to generate hash codes.
    /// </summary>
    public static class IEnumerableTExtensions
    {
        /// <summary>
        /// Gets a hashcode for a collection of items.
        /// </summary>
        /// <typeparam name="T">The type of object in the collection.</typeparam>
        /// <param name="enumerable">The collection to generate a hashcode for.</param>
        /// <returns>A hashcode for a collection of items.</returns>
        /// <remarks>This method will currently not work properly for a collection of collections.</remarks>
        public static int GetHashCodeForCollection<T>(this IEnumerable<T> enumerable)
        {
            unchecked
            {
                var hashcode = 397;
                foreach (var item in enumerable)
                {
                    hashcode = (hashcode * 397) ^ item?.GetHashCode() ?? 0;
                }

                return hashcode;
            }
        }
    }
}
