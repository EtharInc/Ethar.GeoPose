// <copyright file="EtharTransitionModelJsonConverter.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.JsonConversion
{
    using Ethar.GeoPose.Authority.TransitionModels;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A utility class that converts JObjects to transition models.
    /// </summary>
    internal class EtharTransitionModelJsonConverter
    {
        /// <summary>
        /// Converts json to a <see cref="NoneTransitionModel"/>.
        /// </summary>
        /// <param name="jObject">The json to convert.</param>
        /// <returns>A <see cref="NoneTransitionModel"/>.</returns>
        internal static NoneTransitionModel ConvertJsonToNoneTransitionModel(JObject jObject) => new NoneTransitionModel();

        /// <summary>
        /// Converts a <see cref="NoneTransitionModel"/> to json.
        /// </summary>
        /// <param name="model">The <see cref="NoneTransitionModel"/> to convert.</param>
        /// <returns>A json representation of the <see cref="NoneTransitionModel"/>.</returns>
        internal static JObject ConvertNoneTransitionModelToJson(NoneTransitionModel model)
        {
            var jObj = new JObject
            {
                { "authority", model.Authority },
                { "id", model.Id },
                { "parameters", string.Empty },
            };

            return jObj;
        }

        /// <summary>
        /// Converts json to a <see cref="InterpolatedTransitionModel"/>.
        /// </summary>
        /// <param name="jObject">The json to convert.</param>
        /// <returns>A <see cref="InterpolatedTransitionModel"/>.</returns>
        internal static InterpolatedTransitionModel ConvertJsonToInterpolatedTransitionModel(JObject jObject) => new InterpolatedTransitionModel();

        /// <summary>
        /// Converts a <see cref="InterpolatedTransitionModel"/> to json.
        /// </summary>
        /// <param name="model">The <see cref="InterpolatedTransitionModel"/> to convert.</param>
        /// <returns>A json representation of the <see cref="InterpolatedTransitionModel"/>.</returns>
        internal static JObject ConvertInterpolatedTransitionModelToJson(InterpolatedTransitionModel model)
        {
            var jObj = new JObject
            {
                { "authority", model.Authority },
                { "id", model.Id },
                { "parameters", string.Empty },
            };

            return jObj;
        }
    }
}
