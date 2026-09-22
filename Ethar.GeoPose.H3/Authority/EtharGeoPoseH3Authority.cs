// <copyright file="EtharGeoPoseH3Authority.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.H3.Authority
{
    using System;
    using Ethar.GeoPose.Authority;
    using Ethar.GeoPose.Exceptions;
    using Ethar.GeoPose.Interfaces;
    using Ethar.GeoPose.JsonConversion;
    using Ethar.GeoPose.TransitionModels;
    using Ethar.GeoPose.Validation;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The GeoPose authority that reads and writes the <see cref="H3CellSpecification"/> frame. Register it with <see cref="Register"/> before serializing an Advanced GeoPose that uses the frame.
    /// </summary>
    /// <remarks>
    /// The authority defines frame specifications only. Transition models belong to the main Ethar authority, so the transition model conversions throw.
    /// </remarks>
    public class EtharGeoPoseH3Authority : IAuthority
    {
        /// <inheritdoc/>
        public string AuthorityName => H3AuthorityConstants.AuthorityName;

        /// <summary>
        /// Registers the authority with the <see cref="AuthorityProvider"/>.
        /// </summary>
        public static void Register()
        {
            AuthorityProvider.RegisterAuthority(new EtharGeoPoseH3Authority());
        }

        /// <summary>
        /// Removes the authority from the <see cref="AuthorityProvider"/>.
        /// </summary>
        public static void Unregister()
        {
            AuthorityProvider.UnregisterAuthority(H3AuthorityConstants.AuthorityName);
        }

        /// <inheritdoc/>
        public JObject ConvertFrameSpecToJson(IFrameSpecification frameSpec)
        {
            if (frameSpec is H3CellSpecification h3Cell)
            {
                return new JObject
                {
                    ["authority"] = h3Cell.Authority,
                    ["id"] = h3Cell.Id,
                    ["parameters"] = h3Cell.BuildParameters(),
                };
            }

            throw new NotImplementedException("The frame specification " + frameSpec?.Id + " does not exist in the authority " + this.AuthorityName);
        }

        /// <inheritdoc/>
        public IFrameSpecification ConvertJsonToFrameSpec(JObject jsonObject)
        {
            if (!ValidationUtilities.ValidateJsonObjectParameters<H3CellSpecification>(jsonObject, out var parameters))
            {
                throw new FrameSpecificationInvalidException("The frame specification could not be validated.");
            }

            var cellText = parameters.GetParameter(H3AuthorityConstants.CellParameter);
            if (!H3Index.TryParse(cellText, out var cell))
            {
                throw new FrameSpecificationInvalidException("Parameter " + H3AuthorityConstants.CellParameter + " has the value '" + cellText + "', which is not a valid H3 cell index.");
            }

            var height = InvariantNumber.Parse(parameters.GetParameter(H3AuthorityConstants.HeightParameter), H3AuthorityConstants.HeightParameter);
            return new H3CellSpecification(cell, height);
        }

        /// <inheritdoc/>
        public TransitionModel ConvertJsonToTransitionModel(JObject jsonObject)
        {
            throw new TransitionModelInvalidException("The authority " + this.AuthorityName + " defines frame specifications only. Use the Ethar or OGC authority for transition models.");
        }

        /// <inheritdoc/>
        public JObject ConvertTransitionModelToJson(TransitionModel transitionModel)
        {
            throw new TransitionModelInvalidException("The authority " + this.AuthorityName + " defines frame specifications only. Use the Ethar or OGC authority for transition models.");
        }

        /// <inheritdoc/>
        public bool IsFrameSpecificationExtrinsic(IFrameSpecification frameSpec)
        {
            return frameSpec is H3CellSpecification;
        }
    }
}
