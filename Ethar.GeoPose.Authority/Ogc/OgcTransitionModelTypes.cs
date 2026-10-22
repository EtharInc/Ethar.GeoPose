// <copyright file="OgcTransitionModelTypes.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Ogc
{
    /// <summary>
    /// Transition model ids used by the OGC GeoPose 1.0 example instances.
    /// </summary>
    public static class OgcTransitionModelTypes
    {
        /// <summary>
        /// No transition between poses.
        /// </summary>
        public const string None = "none";

        /// <summary>
        /// Interpolated transition between poses. The instances name it <c>interpolate</c>; the Ethar authority's equivalent is <c>interpolated</c>.
        /// </summary>
        public const string Interpolate = "interpolate";
    }
}
