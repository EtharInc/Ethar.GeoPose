// <copyright file="ValidationUtilities.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Validation
{
    using System;
    using System.Collections.Specialized;
    using Ethar.GeoPose.Exceptions;
    using Ethar.GeoPose.Interfaces;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A collection of GeoPose validation routines.
    /// </summary>
    public static class ValidationUtilities
    {
        /// <summary>
        /// Validate the inbound <see cref="JObject"/> json definition for expected parameters.
        /// </summary>
        /// <typeparam name="T">A Frame Specification implementing the IFrameSpecification interface, likely derrived from BaseFrameSpecification.</typeparam>
        /// <param name="jObject">The inbound JSON object in <see cref="JObject"/> format.</param>
        /// <param name="queryString">A <see cref="NameValueCollection"/> of the json Object values.</param>
        /// <returns>Returns true if the inbound JSON was valid.</returns>
        /// <exception cref="NullReferenceException">Raises a <see cref="NullReferenceException"/> on input values that are missing.</exception>
        /// <exception cref="FrameSpecificationInvalidException">Raises a <see cref="FrameSpecificationInvalidException"/> if the Frame specification ID is incorrect.</exception>
        /// <exception cref="AuthorityNotSupportedException">Raises a <see cref="AuthorityNotSupportedException"/> if the specified authority was not found in the incoming frame.</exception>
        public static bool ValidateJsonObjectParameters<T>(JObject jObject, out NameValueCollection queryString)
            where T : IFrameSpecification, new()
        {
            if (jObject is null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            var frameSpecification = new T();

            if (frameSpecification == null)
            {
                throw new FrameSpecificationInvalidException(
                    $"Frame specification could not be validated, check it has an overload with no constructor parameters");
            }

            if (string.IsNullOrEmpty(frameSpecification.Id))
            {
                throw new ArgumentException($"Frame Specification ID is missing in the FrameSpecification, is it initialized by default?");
            }

            if (string.IsNullOrEmpty((string)jObject["id"]))
            {
                throw new NullReferenceException($"Missing value for id.");
            }

            if (!string.IsNullOrEmpty(frameSpecification.Id) && !string.Equals((string)jObject["id"], frameSpecification.Id, StringComparison.OrdinalIgnoreCase))
            {
                throw new FrameSpecificationInvalidException(
                    $"Frame specification ID should be {frameSpecification.Id}");
            }

            if (string.IsNullOrEmpty(frameSpecification.Authority))
            {
                throw new ArgumentException($"The Authority Name is missing in the FrameSpecification, is it initialized by default?");
            }

            if (string.IsNullOrEmpty((string)jObject["authority"]))
            {
                throw new NullReferenceException($"Missing value for authority.");
            }

            if (!string.Equals((string)jObject["authority"], frameSpecification.Authority, StringComparison.OrdinalIgnoreCase))
            {
                throw new AuthorityNotSupportedException($"Authority must be {frameSpecification.Authority}.");
            }

            if (string.IsNullOrEmpty((string)jObject["parameters"]))
            {
                throw new NullReferenceException($"Missing value for parameters.");
            }

            queryString = GetQueryParameters((string)jObject["parameters"]);

            return true;
        }

        /// <summary>
        /// Resolve the parameters in a URL Querystring.
        /// </summary>
        /// <param name="queryString">input query string.</param>
        /// <returns>A <see cref="NameValueCollection"/> of Query string parameters and values.</returns>
        public static NameValueCollection GetQueryParameters(string queryString)
        {
            NameValueCollection queryParameters = new NameValueCollection();
            string[] querySegments = queryString.Split('&');
            foreach (string segment in querySegments)
            {
                string[] parts = segment.Split('=');
                if (parts.Length > 0)
                {
                    string key = parts[0].Trim(new char[] { '?', ' ' });
                    string val = parts[1].Trim();

                    queryParameters.Add(key, val);
                }
            }

            return queryParameters;
        }

        /// <summary>
        /// Utility to retrieve Frame Specification Parameter data from a deserialised <see cref="NameValueCollection"/>.
        /// </summary>
        /// <param name="collection">The input <see cref="NameValueCollection"/> containing deserialised <see cref="IFrameSpecification"/> parameter data.</param>
        /// <param name="parameterName">The name of the parameter to retrieve from the collection.</param>
        /// <returns>Returns the string value from the collection if found.</returns>
        /// <exception cref="ArgumentException">Raises a <see cref="ArgumentException"/> on input values that are missing.</exception>
        /// <exception cref="FrameSpecificationInvalidException">Raises a <see cref="FrameSpecificationInvalidException"/> if the specified Parameter Name could not be retrieved from the Frame Specification parameters.</exception>
        public static string GetParameter(this NameValueCollection collection, string parameterName)
        {
            if (collection == null || collection.Count == 0)
            {
                throw new ArgumentException($"Specified collection is null or not initialised");
            }

            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentException($"'{nameof(parameterName)}' cannot be null or empty.", nameof(parameterName));
            }

            if (collection[parameterName] == null)
            {
                throw new FrameSpecificationInvalidException(
                    $"Parameter {parameterName} was not found in the input collection, is the Frame Specification invalid?");
            }

            return collection[parameterName];
        }
    }
}