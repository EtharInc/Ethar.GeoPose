// <copyright file="OgcNoneTransitionModel.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Ogc
{
    using Ethar.GeoPose.TransitionModels;

    /// <summary>
    /// The <c>none</c> transition model under the <c>/geopose/1.0</c> authority.
    /// </summary>
    public class OgcNoneTransitionModel : TransitionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OgcNoneTransitionModel"/> class.
        /// </summary>
        public OgcNoneTransitionModel()
            : base(OgcTransitionModelTypes.None, OgcConstants.AuthorityName)
        {
        }
    }
}
