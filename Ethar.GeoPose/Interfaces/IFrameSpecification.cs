// <copyright file="IFrameSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Interfaces
{
    using Newtonsoft.Json;

    /// <summary>
    /// A Frame Specificaton interface that completely and uniquely defines a reference frame.
    ///
    /// Derived from Figure 8 from section 7.2.1 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public interface IFrameSpecification
    {
        /// <summary>
        /// Gets the frame specification's authority.
        /// </summary>
        [JsonProperty("authority")]
        string Authority { get; }

        /// <summary>
        /// Gets the frame specification's ID.
        /// </summary>
        [JsonProperty("id")]
        string Id { get; }
    }
}