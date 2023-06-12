// <copyright file="InterpolatedTransitionModel.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.TransitionModels
{
    using Ethar.GeoPose.TransitionModels;

    /// <summary>
    /// A class that represents an interpolated transition model.
    /// </summary>
    public class InterpolatedTransitionModel : TransitionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedTransitionModel"/> class.
        /// </summary>
        public InterpolatedTransitionModel()
            : base("interpolated", Constants.AuthorityName)
        {
        }
    }
}
