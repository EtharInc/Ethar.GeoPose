// <copyright file="NoneTransitionModel.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.TransitionModels
{
    using Ethar.GeoPose.TransitionModels;

    /// <summary>
    /// A class that represents an empty transition model as it is not required.
    /// </summary>
    public class NoneTransitionModel : TransitionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoneTransitionModel"/> class.
        /// </summary>
        public NoneTransitionModel()
            : base("none", Constants.AuthorityName)
        {
        }
    }
}
