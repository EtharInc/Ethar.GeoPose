// <copyright file="EtharFrameSpecificationJsonConverter.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.JsonConversion
{
    using System.Collections.Specialized;
    using Ethar.GeoPose.Authority.FrameSpecifications;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Extensions;
    using Ethar.GeoPose.JsonConversion;
    using Ethar.GeoPose.Validation;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Converts the Ethar authority's frame specifications to and from their JSON representation.
    /// </summary>
    /// <remarks>
    /// All numbers are read and written with the invariant culture. On input the OGC parameter name <c>height</c> is accepted as an
    /// alias of <c>heightInMeters</c>; on output the Ethar names are always written.
    /// </remarks>
    internal class EtharFrameSpecificationJsonConverter
    {
        /// <summary>
        /// Converts a JSON object to an LTP-ENU frame specification.
        /// </summary>
        /// <param name="jObject">The JSON object.</param>
        /// <returns>The frame specification, or null if the parameters could not be validated.</returns>
        internal static LtpEnuSpecification ConvertJsonToLtpEnuFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<LtpEnuSpecification>(jObject, out var queryString))
            {
                return new LtpEnuSpecification(ReadPosition(queryString));
            }

            return null;
        }

        /// <summary>
        /// Converts an LTP-ENU frame specification to a JSON object.
        /// </summary>
        /// <param name="spec">The frame specification.</param>
        /// <returns>The JSON object.</returns>
        internal static JObject ConvertLtpEnuFrameSpecificationToJson(LtpEnuSpecification spec)
        {
            return BuildJson(spec.Authority, spec.Id, spec.Position.BuildParamString());
        }

        /// <summary>
        /// Converts a JSON object to an LTP-NED frame specification.
        /// </summary>
        /// <param name="jObject">The JSON object.</param>
        /// <returns>The frame specification, or null if the parameters could not be validated.</returns>
        internal static LtpNedSpecification ConvertJsonToLtpNedFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<LtpNedSpecification>(jObject, out var queryString))
            {
                return new LtpNedSpecification(ReadPosition(queryString));
            }

            return null;
        }

        /// <summary>
        /// Converts an LTP-NED frame specification to a JSON object.
        /// </summary>
        /// <param name="spec">The frame specification.</param>
        /// <returns>The JSON object.</returns>
        internal static JObject ConvertLtpNedFrameSpecificationToJson(LtpNedSpecification spec)
        {
            return BuildJson(spec.Authority, spec.Id, spec.Position.BuildParamString());
        }

        /// <summary>
        /// Converts a JSON object to a yaw, pitch, roll oriented LTP-ENU frame specification.
        /// </summary>
        /// <param name="jObject">The JSON object.</param>
        /// <returns>The frame specification, or null if the parameters could not be validated.</returns>
        internal static YawPitchRollOrientedLtpEnuSpecification ConvertJsonToYprOrientedLtpEnuFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<YawPitchRollOrientedLtpEnuSpecification>(jObject, out var queryString))
            {
                var orientation = new YawPitchRollAngles(
                    ReadNumber(queryString, "orientation.yaw"),
                    ReadNumber(queryString, "orientation.pitch"),
                    ReadNumber(queryString, "orientation.roll"));
                return new YawPitchRollOrientedLtpEnuSpecification(ReadPosition(queryString), orientation);
            }

            return null;
        }

        /// <summary>
        /// Converts a yaw, pitch, roll oriented LTP-ENU frame specification to a JSON object.
        /// </summary>
        /// <param name="spec">The frame specification.</param>
        /// <returns>The JSON object.</returns>
        internal static JObject ConvertYprOrientedLtpEnuFrameSpecificationToJson(YawPitchRollOrientedLtpEnuSpecification spec)
        {
            return BuildJson(spec.Authority, spec.Id, string.Concat(spec.Position.BuildParamString(), "&", spec.Orientation.BuildOrientationParamString()));
        }

        /// <summary>
        /// Converts a JSON object to a quaternion oriented LTP-ENU frame specification.
        /// </summary>
        /// <param name="jObject">The JSON object.</param>
        /// <returns>The frame specification, or null if the parameters could not be validated.</returns>
        internal static QuaternionOrientedLtpEnuSpecification ConvertJsonToQuaternionOrientedLtpEnuFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<QuaternionOrientedLtpEnuSpecification>(jObject, out var queryString))
            {
                var orientation = new UnitQuaternion(
                    ReadNumber(queryString, "orientation.x"),
                    ReadNumber(queryString, "orientation.y"),
                    ReadNumber(queryString, "orientation.z"),
                    ReadNumber(queryString, "orientation.w"));
                return new QuaternionOrientedLtpEnuSpecification(ReadPosition(queryString), orientation);
            }

            return null;
        }

        /// <summary>
        /// Converts a quaternion oriented LTP-ENU frame specification to a JSON object.
        /// </summary>
        /// <param name="spec">The frame specification.</param>
        /// <returns>The JSON object.</returns>
        internal static JObject ConvertQuaternionOrientedLtpEnuFrameSpecificationToJson(QuaternionOrientedLtpEnuSpecification spec)
        {
            return BuildJson(spec.Authority, spec.Id, string.Concat(spec.Position.BuildParamString(), "&", spec.Orientation.BuildOrientationParamString()));
        }

        /// <summary>
        /// Converts a JSON object to a translate-rotate frame specification.
        /// </summary>
        /// <param name="jObject">The JSON object.</param>
        /// <returns>The frame specification, or null if the parameters could not be validated.</returns>
        internal static TranslateRotateSpecification ConvertJsonToTranslateRotateFrameSpecification(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<TranslateRotateSpecification>(jObject, out var queryString))
            {
                var translation = new UnitVector3(
                    ReadNumber(queryString, "translation.x"),
                    ReadNumber(queryString, "translation.y"),
                    ReadNumber(queryString, "translation.z"));
                var rotation = new UnitQuaternion(
                    ReadNumber(queryString, "rotation.x"),
                    ReadNumber(queryString, "rotation.y"),
                    ReadNumber(queryString, "rotation.z"),
                    ReadNumber(queryString, "rotation.w"));
                return new TranslateRotateSpecification(translation, rotation);
            }

            return null;
        }

        /// <summary>
        /// Converts a translate-rotate frame specification to a JSON object.
        /// </summary>
        /// <param name="spec">The frame specification.</param>
        /// <returns>The JSON object.</returns>
        internal static JObject ConvertTranslateRotateFrameSpecificationToJson(TranslateRotateSpecification spec)
        {
            return BuildJson(spec.Authority, spec.Id, string.Concat(spec.Translation.BuildTranslationParamString(), "&", spec.Rotation.BuildRotationParamString()));
        }

        private static JObject BuildJson(string authority, string id, string parameters)
        {
            return new JObject
            {
                { "authority", authority },
                { "id", id },
                { "parameters", parameters },
            };
        }

        private static TangentPointPosition ReadPosition(NameValueCollection queryString)
        {
            var heightText = queryString["heightInMeters"] ?? queryString["height"];
            return new TangentPointPosition(
                ReadNumber(queryString, "latitude"),
                ReadNumber(queryString, "longitude"),
                InvariantNumber.Parse(heightText ?? queryString.GetParameter("heightInMeters"), "heightInMeters"));
        }

        private static double ReadNumber(NameValueCollection queryString, string parameterName)
        {
            return InvariantNumber.Parse(queryString.GetParameter(parameterName), parameterName);
        }
    }
}
