// <copyright file="OgcInterpolateTransitionModel.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Ogc
{
    using Ethar.GeoPose.TransitionModels;

    /// <summary>
    /// The <c>interpolate</c> transition model under the <c>/geopose/1.0</c> authority.
    /// </summary>
    public class OgcInterpolateTransitionModel : TransitionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OgcInterpolateTransitionModel"/> class.
        /// </summary>
        public OgcInterpolateTransitionModel()
            : base(OgcTransitionModelTypes.Interpolate, OgcConstants.AuthorityName)
        {
        }
    }
}
