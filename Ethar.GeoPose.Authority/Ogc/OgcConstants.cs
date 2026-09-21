// <copyright file="OgcConstants.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Ogc
{
    /// <summary>
    /// Identifiers used by the OGC GeoPose 1.0 example instances (https://schemas.opengis.net/geopose/1.0/instances/).
    /// </summary>
    /// <remarks>
    /// The standard leaves authority strings, frame ids and parameter grammar to each authority. The OGC's own instance files, which
    /// the conformance test suite and other implementations use, all carry the authority <c>/geopose/1.0</c> and the ids and grammar
    /// below. <see cref="OgcGeoPoseAuthority"/> implements that authority so those files, and payloads from implementations that follow
    /// them, can be exchanged with this library.
    /// </remarks>
    public static class OgcConstants
    {
        /// <summary>
        /// The authority string used by the OGC example instances.
        /// </summary>
        public const string AuthorityName = "/geopose/1.0";
    }
}
