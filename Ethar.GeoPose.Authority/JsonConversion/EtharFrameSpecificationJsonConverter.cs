// <copyright file="EtharFrameSpecificationJsonConverter.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.JsonConversion
{
    using Ethar.GeoPose.Authority.FrameSpecifications;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Extensions;
    using Ethar.GeoPose.Validation;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A utility class that converts JObjects to frame specifications.
    /// </summary>
    internal class EtharFrameSpecificationJsonConverter
    {
        /// <summary>
        /// Converts json to a <see cref="LtpEnuSpecification"/>.
        /// </summary>
        /// <param name="jObject">The json to convert.</param>
        /// <returns>A <see cref="LtpEnuSpecification"/>.</returns>
        internal static LtpEnuSpecification ConvertJsonToLtpEnuFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<LtpEnuSpecification>(jObject, out var queryString))
            {
                var lat = float.Parse(queryString.GetParameter("latitude"));
                var lon = float.Parse(queryString.GetParameter("longitude"));
                var height = float.Parse(queryString.GetParameter("heightInMeters"));

                return new LtpEnuSpecification(new TangentPointPosition() { Latitude = lat, Longitude = lon, HeightInMeters = height });
            }

            return null;
        }

        /// <summary>
        /// Converts a <see cref="LtpEnuSpecification"/> to json.
        /// </summary>
        /// <param name="spec">The <see cref="LtpEnuSpecification"/> to convert.</param>
        /// <returns>A json representation of the <see cref="LtpEnuSpecification"/>.</returns>
        internal static JObject ConvertLtpEnuFrameSpecificationToJson(LtpEnuSpecification spec)
        {
            var paramString = spec.Position.BuildParamString();

            var jObj = new JObject
            {
                { "authority", spec.Authority },
                { "id", spec.Id },
                { "parameters", paramString },
            };

            return jObj;
        }

        /// <summary>
        /// Converts json to a <see cref="LtpNedSpecification"/>.
        /// </summary>
        /// <param name="jObject">The json to convert.</param>
        /// <returns>A <see cref="LtpNedSpecification"/>.</returns>
        internal static LtpNedSpecification ConvertJsonToLtpNedFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<LtpNedSpecification>(jObject, out var queryString))
            {
                var lat = float.Parse(queryString.GetParameter("latitude"));
                var lon = float.Parse(queryString.GetParameter("longitude"));
                var height = float.Parse(queryString.GetParameter("heightInMeters"));

                return new LtpNedSpecification(new TangentPointPosition() { Latitude = lat, Longitude = lon, HeightInMeters = height });
            }

            return null;
        }

        /// <summary>
        /// Converts a <see cref="LtpNedSpecification"/> to json.
        /// </summary>
        /// <param name="spec">The <see cref="LtpNedSpecification"/> to convert.</param>
        /// <returns>A json representation of the <see cref="LtpNedSpecification"/>.</returns>
        internal static JObject ConvertLtpNedFrameSpecificationToJson(LtpNedSpecification spec)
        {
            var paramString = spec.Position.BuildParamString();

            var jObj = new JObject
            {
                { "authority", spec.Authority },
                { "id", spec.Id },
                { "parameters", paramString },
            };

            return jObj;
        }

        /// <summary>
        /// Converts json to a <see cref="YawPitchRollOrientedLtpEnuSpecification"/>.
        /// </summary>
        /// <param name="jObject">The json to convert.</param>
        /// <returns>A <see cref="YawPitchRollOrientedLtpEnuSpecification"/>.</returns>
        internal static YawPitchRollOrientedLtpEnuSpecification ConvertJsonToYprOrientedLtpEnuFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<YawPitchRollOrientedLtpEnuSpecification>(jObject, out var queryString))
            {
                var lat = float.Parse(queryString.GetParameter("latitude"));
                var lon = float.Parse(queryString.GetParameter("longitude"));
                var height = float.Parse(queryString.GetParameter("heightInMeters"));
                var yaw = float.Parse(queryString.GetParameter("orientation.yaw"));
                var pitch = float.Parse(queryString.GetParameter("orientation.pitch"));
                var roll = float.Parse(queryString.GetParameter("orientation.roll"));

                return new YawPitchRollOrientedLtpEnuSpecification(new TangentPointPosition() { Latitude = lat, Longitude = lon, HeightInMeters = height }, new YawPitchRollAngles() { Yaw = yaw, Pitch = pitch, Roll = roll });
            }

            return null;
        }

        /// <summary>
        /// Converts a <see cref="YawPitchRollOrientedLtpEnuSpecification"/> to json.
        /// </summary>
        /// <param name="spec">The <see cref="YawPitchRollOrientedLtpEnuSpecification"/> to convert.</param>
        /// <returns>A json representation of the <see cref="YawPitchRollOrientedLtpEnuSpecification"/>.</returns>
        internal static JObject ConvertYprOrientedLtpEnuFrameSpecificationToJson(YawPitchRollOrientedLtpEnuSpecification spec)
        {
            var positionParamString = spec.Position.BuildParamString();
            var orientationParamString = spec.Orientation.BuildOrientationParamString();

            var jObj = new JObject
            {
                { "authority", spec.Authority },
                { "id", spec.Id },
                { "parameters", string.Concat(positionParamString, "&", orientationParamString) },
            };

            return jObj;
        }

        /// <summary>
        /// Converts json to a <see cref="QuaternionOrientedLtpEnuSpecification"/>.
        /// </summary>
        /// <param name="jObject">The json to convert.</param>
        /// <returns>A <see cref="QuaternionOrientedLtpEnuSpecification"/>.</returns>
        internal static QuaternionOrientedLtpEnuSpecification ConvertJsonToQuaternionOrientedLtpEnuFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<QuaternionOrientedLtpEnuSpecification>(jObject, out var queryString))
            {
                var lat = float.Parse(queryString.GetParameter("latitude"));
                var lon = float.Parse(queryString.GetParameter("longitude"));
                var height = float.Parse(queryString.GetParameter("heightInMeters"));
                var x = float.Parse(queryString.GetParameter("orientation.x"));
                var y = float.Parse(queryString.GetParameter("orientation.y"));
                var z = float.Parse(queryString.GetParameter("orientation.z"));
                var w = float.Parse(queryString.GetParameter("orientation.w"));

                return new QuaternionOrientedLtpEnuSpecification(new TangentPointPosition() { Latitude = lat, Longitude = lon, HeightInMeters = height }, new UnitQuaternion() { X = x, Y = y, Z = z, W = w });
            }

            return null;
        }

        /// <summary>
        /// Converts a <see cref="QuaternionOrientedLtpEnuSpecification"/> to json.
        /// </summary>
        /// <param name="spec">The <see cref="QuaternionOrientedLtpEnuSpecification"/> to convert.</param>
        /// <returns>A json representation of the <see cref="QuaternionOrientedLtpEnuSpecification"/>.</returns>
        internal static JObject ConvertQuaternionOrientedLtpEnuFrameSpecificationToJson(QuaternionOrientedLtpEnuSpecification spec)
        {
            var positionParamString = spec.Position.BuildParamString();
            var orientationParamString = spec.Orientation.BuildOrientationParamString();

            var jObj = new JObject
            {
                { "authority", spec.Authority },
                { "id", spec.Id },
                { "parameters", string.Concat(positionParamString, "&", orientationParamString) },
            };

            return jObj;
        }

        /// <summary>
        /// Converts json to a <see cref="TranslateRotateSpecification"/>.
        /// </summary>
        /// <param name="jObject">The json to convert.</param>
        /// <returns>A <see cref="TranslateRotateSpecification"/>.</returns>
        internal static TranslateRotateSpecification ConvertJsonToTranslateRotateFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<TranslateRotateSpecification>(jObject, out var queryString))
            {
                var translationX = JsonConvert.DeserializeObject<float>(queryString.GetParameter("translation.x"));
                var translationY = JsonConvert.DeserializeObject<float>(queryString.GetParameter("translation.y"));
                var translationZ = JsonConvert.DeserializeObject<float>(queryString.GetParameter("translation.z"));
                var rotationX = JsonConvert.DeserializeObject<float>(queryString.GetParameter("rotation.x"));
                var rotationY = JsonConvert.DeserializeObject<float>(queryString.GetParameter("rotation.y"));
                var rotationZ = JsonConvert.DeserializeObject<float>(queryString.GetParameter("rotation.z"));
                var rotationW = JsonConvert.DeserializeObject<float>(queryString.GetParameter("rotation.w"));

                return new TranslateRotateSpecification(new UnitVector3() { X = translationX, Y = translationY, Z = translationZ }, new UnitQuaternion() { X = rotationX, Y = rotationY, Z = rotationZ, W = rotationW });
            }

            return null;
        }

        /// <summary>
        /// Converts a <see cref="TranslateRotateSpecification"/> to json.
        /// </summary>
        /// <param name="spec">The <see cref="TranslateRotateSpecification"/> to convert.</param>
        /// <returns>A json representation of the <see cref="TranslateRotateSpecification"/>.</returns>
        internal static JObject ConvertTranslateRotateFrameSpecificationToJson(TranslateRotateSpecification spec)
        {
            var positionParamString = spec.Translation.BuildTranslationParamString();
            var orientationParamString = spec.Rotation.BuildRotationParamString();

            var jObj = new JObject
            {
                { "authority", spec.Authority },
                { "id", spec.Id },
                { "parameters", string.Concat(positionParamString, "&", orientationParamString) },
            };

            return jObj;
        }
    }
}