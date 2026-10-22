// <copyright file="OgcGeoPoseAuthority.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Ogc
{
    using System;
    using System.Collections.Specialized;
    using Ethar.GeoPose.Authority.FrameSpecifications;
    using Ethar.GeoPose.Authority.Validation;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Exceptions;
    using Ethar.GeoPose.Interfaces;
    using Ethar.GeoPose.JsonConversion;
    using Ethar.GeoPose.TransitionModels;
    using Ethar.GeoPose.Validation;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The <c>/geopose/1.0</c> authority: reads and writes the frame specifications and transition models exactly as the OGC GeoPose 1.0
    /// example instances encode them, so that those files and payloads from other implementations can be exchanged with this library.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Grammar, taken from https://schemas.opengis.net/geopose/1.0/instances/: LTP frames carry
    /// <c>longitude=…&amp;latitude=…&amp;height=…</c> (height above the WGS 84 ellipsoid), and translate-rotate frames carry
    /// <c>translation=[x, y, z]&amp;rotation=[w, x, y, z]</c>. The rotation array is w-first, as the standard's prose lists the components;
    /// the JSON <c>quaternion</c> member of the Basic and Advanced targets is x-first. Numbers are culture-invariant.
    /// </para>
    /// <para>
    /// Register with <c>AuthorityProvider.RegisterAuthority(new OgcGeoPoseAuthority())</c>, or call <see cref="GeoPoseAuthorities.RegisterDefaults"/>
    /// to register it together with the Ethar authority.
    /// </para>
    /// </remarks>
    public class OgcGeoPoseAuthority : IAuthority
    {
        private const string LongitudeKey = "longitude";
        private const string LatitudeKey = "latitude";
        private const string HeightKey = "height";
        private const string HeightAliasKey = "heightInMeters";
        private const string TranslationKey = "translation";
        private const string RotationKey = "rotation";

        /// <summary>
        /// Initializes a new instance of the <see cref="OgcGeoPoseAuthority"/> class.
        /// </summary>
        public OgcGeoPoseAuthority()
        {
            this.Validator = new EtharGeoPoseAuthorityExplicitFrameSpecificationValidator();
        }

        /// <inheritdoc/>
        public string AuthorityName => OgcConstants.AuthorityName;

        /// <summary>
        /// Gets the validator applied to parsed frame specifications.
        /// </summary>
        public IExplicitFrameSpecificationValidator Validator { get; }

        /// <inheritdoc/>
        public JObject ConvertFrameSpecToJson(IFrameSpecification frameSpec)
        {
            switch (frameSpec)
            {
                case TranslateRotateSpecification translateRotate:
                    return BuildJson(
                        translateRotate.Id,
                        string.Concat(
                            TranslationKey,
                            "=",
                            InvariantNumber.FormatArray(translateRotate.Translation.X, translateRotate.Translation.Y, translateRotate.Translation.Z),
                            "&",
                            RotationKey,
                            "=",
                            InvariantNumber.FormatArray(translateRotate.Rotation.W, translateRotate.Rotation.X, translateRotate.Rotation.Y, translateRotate.Rotation.Z)));
                case LtpEnuSpecification ltpEnu:
                    return BuildJson(ltpEnu.Id, BuildPositionParameters(ltpEnu.Position));
                case LtpNedSpecification ltpNed:
                    return BuildJson(ltpNed.Id, BuildPositionParameters(ltpNed.Position));
                default:
                    throw new NotImplementedException($"The frame specification {frameSpec?.Id} does not exist in the authority {this.AuthorityName}");
            }
        }

        /// <inheritdoc/>
        public IFrameSpecification ConvertJsonToFrameSpec(JObject jsonObject)
        {
            var id = RequireId(jsonObject);
            var parameters = ValidationUtilities.GetQueryParameters(RequireParameters(jsonObject));

            IFrameSpecification frameSpec;
            switch (id)
            {
                case OgcFrameSpecificationTypes.LtpEnu:
                case OgcFrameSpecificationTypes.ExtrinsicLtpEnu:
                    frameSpec = new OgcLtpEnuSpecification(ReadPosition(parameters), id);
                    break;
                case OgcFrameSpecificationTypes.LtpNed:
                    frameSpec = new OgcLtpNedSpecification(ReadPosition(parameters));
                    break;
                case OgcFrameSpecificationTypes.IntrinsicTranslateRotate:
                case OgcFrameSpecificationTypes.RotateTranslate:
                    var translation = InvariantNumber.ParseArray(parameters[TranslationKey], TranslationKey, 3);
                    var rotation = InvariantNumber.ParseArray(parameters[RotationKey], RotationKey, 4);
                    frameSpec = new OgcTranslateRotateSpecification(
                        new UnitVector3(translation[0], translation[1], translation[2]),
                        new UnitQuaternion(rotation[1], rotation[2], rotation[3], rotation[0]),
                        id);
                    break;
                default:
                    throw new FrameSpecificationInvalidException($"The frame specification id '{id}' is not known to the authority {this.AuthorityName}.");
            }

            var validationResult = this.Validator.Validate(frameSpec);
            return validationResult.IsValid ? frameSpec : throw new FrameSpecificationInvalidException(validationResult.Message);
        }

        /// <inheritdoc/>
        public TransitionModel ConvertJsonToTransitionModel(JObject jsonObject)
        {
            var id = RequireId(jsonObject);
            switch (id)
            {
                case OgcTransitionModelTypes.None:
                    return new OgcNoneTransitionModel();
                case OgcTransitionModelTypes.Interpolate:
                    return new OgcInterpolateTransitionModel();
                default:
                    throw new TransitionModelInvalidException($"The transition model id '{id}' is not known to the authority {this.AuthorityName}.");
            }
        }

        /// <inheritdoc/>
        public JObject ConvertTransitionModelToJson(TransitionModel transitionModel)
        {
            switch (transitionModel)
            {
                case OgcNoneTransitionModel _:
                case OgcInterpolateTransitionModel _:
                    return BuildJson(transitionModel.Id, string.Empty);
                default:
                    throw new NotImplementedException($"The transition model {transitionModel?.Id} does not exist in the authority {this.AuthorityName}");
            }
        }

        /// <inheritdoc/>
        public bool IsFrameSpecificationExtrinsic(IFrameSpecification frameSpec)
        {
            switch (frameSpec)
            {
                case LtpEnuSpecification _:
                case LtpNedSpecification _:
                    return true;
                default:
                    return false;
            }
        }

        private static JObject BuildJson(string id, string parameters)
        {
            return new JObject
            {
                { "authority", OgcConstants.AuthorityName },
                { "id", id },
                { "parameters", parameters },
            };
        }

        private static string BuildPositionParameters(TangentPointPosition position)
        {
            return string.Concat(
                LongitudeKey,
                "=",
                InvariantNumber.Format(position.Longitude),
                "&",
                LatitudeKey,
                "=",
                InvariantNumber.Format(position.Latitude),
                "&",
                HeightKey,
                "=",
                InvariantNumber.Format(position.HeightInMeters));
        }

        private static TangentPointPosition ReadPosition(NameValueCollection parameters)
        {
            var heightText = parameters[HeightKey] ?? parameters[HeightAliasKey];
            if (heightText == null)
            {
                throw new FrameSpecificationInvalidException($"Parameter {HeightKey} was not found in the frame specification.");
            }

            return new TangentPointPosition(
                InvariantNumber.Parse(parameters.GetParameter(LatitudeKey), LatitudeKey),
                InvariantNumber.Parse(parameters.GetParameter(LongitudeKey), LongitudeKey),
                InvariantNumber.Parse(heightText, HeightKey));
        }

        private static string RequireId(JObject jsonObject)
        {
            var id = (string)jsonObject?["id"];
            if (string.IsNullOrEmpty(id))
            {
                throw new FrameSpecificationInvalidException("Missing value for id.");
            }

            return id;
        }

        private static string RequireParameters(JObject jsonObject)
        {
            var parameters = (string)jsonObject?["parameters"];
            if (string.IsNullOrEmpty(parameters))
            {
                throw new FrameSpecificationInvalidException("Missing value for parameters.");
            }

            return parameters;
        }
    }
}
