// <copyright file="IAuthority.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority
{
    using System;
    using Ethar.GeoPose.Exceptions;
    using Ethar.GeoPose.Interfaces;
    using Ethar.GeoPose.TransitionModels;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Interface that represents a GeoPose authority.
    /// </summary>
    public interface IAuthority
    {
        /// <summary>
        /// Gets the name of the authority.
        /// </summary>
        string AuthorityName { get; }

        /// <summary>
        /// Converts json to an <see cref="IFrameSpecification"/> from the authority.
        /// </summary>
        /// <param name="jsonObject">The json to convert.</param>
        /// <returns>An <see cref="IFrameSpecification"/> from the authority.</returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when the frame specification has not been implemented or does not exist in the authority.
        /// </exception>
        IFrameSpecification ConvertJsonToFrameSpec(JObject jsonObject);

        /// <summary>
        /// Converts an <see cref="IFrameSpecification"/> from the authority to json.
        /// </summary>
        /// <param name="frameSpec">The <see cref="IFrameSpecification"/> to convert.</param>
        /// <returns>A json representation of an <see cref="IFrameSpecification"/> from the authority.</returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when the frame specification has not been implemented or does not exist in the authority.
        /// </exception>
        /// <exception cref="FrameSpecificationInvalidException">
        /// May be thrown if the frame specification contains invalid parameters.
        /// </exception>
        JObject ConvertFrameSpecToJson(IFrameSpecification frameSpec);

        /// <summary>
        /// Converts json to an <see cref="TransitionModel"/> from the authority.
        /// </summary>
        /// <param name="jsonObject">The json to convert.</param>
        /// <returns>An <see cref="TransitionModel"/> from the authority.</returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when the transition model has not been implemented or does not exist in the authority.
        /// </exception>
        TransitionModel ConvertJsonToTransitionModel(JObject jsonObject);

        /// <summary>
        /// Converts an <see cref="TransitionModel"/> from the authority to json.
        /// </summary>
        /// <param name="transitionModel">The <see cref="TransitionModel"/> to convert.</param>
        /// <returns>A json representation of an <see cref="TransitionModel"/> from the authority.</returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when the transition model has not been implemented or does not exist in the authority.
        /// </exception>
        /// <exception cref="TransitionModelInvalidException">
        /// May be thrown if the transition model contains invalid parameters.
        /// </exception>
        JObject ConvertTransitionModelToJson(TransitionModel transitionModel);

        /// <summary>
        /// Method to check if the <see cref="IFrameSpecification"/> is extrinsic.
        /// </summary>
        /// <param name="frameSpec">The <see cref="IFrameSpecification"/> to check.</param>
        /// <returns>True if the <see cref="IFrameSpecification"/> is extrinsic, false otherwise.</returns>
        bool IsFrameSpecificationExtrinsic(IFrameSpecification frameSpec);
    }
}