// <copyright file="EtharTransitionModelTypes.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.TransitionModels
{
    using Ethar.GeoPose.TransitionModels;

    /// <summary>
    /// A class that contains the names of the types of <see cref="TransitionModel"/> in the /Ethar.GeoPose/1.0 authority.
    /// </summary>
    public static class EtharTransitionModelTypes
    {
        /// <summary>
        /// Identifier for no transition model.
        /// </summary>
        public const string None = "none";

        /// <summary>
        /// Identifier for an interpolated transition model.
        /// </summary>
        public const string Interpolated = "interpolated";
    }
}
