// <copyright file="DictionaryExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extensions for the Dictionary<![CDATA[<TKey>,<TValue>]]> class to allow safer addition.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// TryAdd extension to safely add an element to a dictionary.
        /// </summary>
        /// <typeparam name="TKey">Key value for the Dictionary.</typeparam>
        /// <typeparam name="TValue">Value to add to the Key in the dictionary.</typeparam>
        /// <param name="input">Input dictionary for the extension.</param>
        /// <param name="key">Dictionary index to try and add.</param>
        /// <param name="value">Dictionary item to add for Key.</param>
        /// <exception cref="ArgumentNullException">Input dictionary is null.</exception>
        public static void TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> input, TKey key, TValue value)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (!input.ContainsKey(key))
            {
                input.Add(key, value);
            }
        }
    }
}