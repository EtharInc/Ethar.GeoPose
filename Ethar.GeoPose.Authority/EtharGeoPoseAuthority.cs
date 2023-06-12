// <copyright file="EtharGeoPoseAuthority.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority
{
    using System;
    using Ethar.GeoPose.Authority.FrameSpecifications;
    using Ethar.GeoPose.Authority.JsonConversion;
    using Ethar.GeoPose.Authority.TransitionModels;
    using Ethar.GeoPose.Authority.Validation;
    using Ethar.GeoPose.Exceptions;
    using Ethar.GeoPose.Interfaces;
    using Ethar.GeoPose.TransitionModels;
    using Ethar.GeoPose.Validation;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A class that implements the "/Ethar.GeoPose/1.0" authority.
    /// </summary>
    public class EtharGeoPoseAuthority : IAuthority
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EtharGeoPoseAuthority"/> class.
        /// </summary>
        public EtharGeoPoseAuthority()
        {
            this.Validator = new EtharGeoPoseAuthorityExplicitFrameSpecificationValidator();
        }

        /// <inheritdoc/>
        public string AuthorityName => Constants.AuthorityName;

        /// <summary>
        /// Gets the frame specification validator.
        /// </summary>
        public IExplicitFrameSpecificationValidator Validator { get; }

        /// <inheritdoc/>
        public JObject ConvertFrameSpecToJson(IFrameSpecification frameSpec)
        {
            switch (frameSpec)
            {
                case YawPitchRollOrientedLtpEnuSpecification yprLtpEnu:
                    return EtharFrameSpecificationJsonConverter.ConvertYprOrientedLtpEnuFrameSpecificationToJson(yprLtpEnu);
                case QuaternionOrientedLtpEnuSpecification quatLtpEnu:
                    return EtharFrameSpecificationJsonConverter.ConvertQuaternionOrientedLtpEnuFrameSpecificationToJson(quatLtpEnu);
                case TranslateRotateSpecification translateRotate:
                    return EtharFrameSpecificationJsonConverter.ConvertTranslateRotateFrameSpecificationToJson(translateRotate);
                case LtpEnuSpecification ltpEnu:
                    return EtharFrameSpecificationJsonConverter.ConvertLtpEnuFrameSpecificationToJson(ltpEnu);
                case LtpNedSpecification ltpNed:
                    return EtharFrameSpecificationJsonConverter.ConvertLtpNedFrameSpecificationToJson(ltpNed);
                default:
                    throw new NotImplementedException($"The frame specification does not exist in the authority {this.AuthorityName}");
            }
        }

        /// <inheritdoc/>
        public IFrameSpecification ConvertJsonToFrameSpec(JObject jsonObject)
        {
            var id = (string)jsonObject["id"];
            if (string.IsNullOrEmpty(id))
            {
                // TODO: create exception type for this
                throw new Exception();
            }

            IFrameSpecification frameSpec;
            FrameSpecificationValidationResult validationResult;

            switch (id)
            {
                case EtharFrameSpecificationTypes.LtpEnuSpec:
                    frameSpec = EtharFrameSpecificationJsonConverter.ConvertJsonToLtpEnuFrameSpecification(jsonObject);
                    validationResult = this.Validator.Validate(frameSpec);
                    return validationResult.IsValid ? frameSpec : throw new FrameSpecificationInvalidException(validationResult.Message);
                case EtharFrameSpecificationTypes.LtpNedSpec:
                    frameSpec = EtharFrameSpecificationJsonConverter.ConvertJsonToLtpNedFrameSpecification(jsonObject);
                    validationResult = this.Validator.Validate(frameSpec);
                    return validationResult.IsValid ? frameSpec : throw new FrameSpecificationInvalidException(validationResult.Message);
                case EtharFrameSpecificationTypes.YprOrientedLtpEnuSpec:
                    frameSpec = EtharFrameSpecificationJsonConverter.ConvertJsonToYprOrientedLtpEnuFrameSpecification(jsonObject);
                    validationResult = this.Validator.Validate(frameSpec);
                    return validationResult.IsValid ? frameSpec : throw new FrameSpecificationInvalidException(validationResult.Message);
                case EtharFrameSpecificationTypes.QuaternionOrientedLtpEnuSpec:
                    frameSpec = EtharFrameSpecificationJsonConverter.ConvertJsonToQuaternionOrientedLtpEnuFrameSpecification(jsonObject);
                    validationResult = this.Validator.Validate(frameSpec);
                    return validationResult.IsValid ? frameSpec : throw new FrameSpecificationInvalidException(validationResult.Message);
                case EtharFrameSpecificationTypes.TranslateRotateSpec:
                    frameSpec = EtharFrameSpecificationJsonConverter.ConvertJsonToTranslateRotateFrameSpecification(jsonObject);
                    validationResult = this.Validator.Validate(frameSpec);
                    return validationResult.IsValid ? frameSpec : throw new FrameSpecificationInvalidException(validationResult.Message);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public TransitionModel ConvertJsonToTransitionModel(JObject jsonObject)
        {
            var id = (string)jsonObject["id"];
            if (string.IsNullOrEmpty(id))
            {
                // TODO: create exception type for this
                throw new Exception();
            }

            switch (id)
            {
                case EtharTransitionModelTypes.None:
                    return EtharTransitionModelJsonConverter.ConvertJsonToNoneTransitionModel(jsonObject);
                case EtharTransitionModelTypes.Interpolated:
                    return EtharTransitionModelJsonConverter.ConvertJsonToInterpolatedTransitionModel(jsonObject);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public JObject ConvertTransitionModelToJson(TransitionModel transitionModel)
        {
            switch (transitionModel)
            {
                case NoneTransitionModel none:
                    return EtharTransitionModelJsonConverter.ConvertNoneTransitionModelToJson(none);
                case InterpolatedTransitionModel interpolated:
                    return EtharTransitionModelJsonConverter.ConvertInterpolatedTransitionModelToJson(interpolated);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public bool IsFrameSpecificationExtrinsic(IFrameSpecification frameSpec)
        {
            switch (frameSpec)
            {
                case YawPitchRollOrientedLtpEnuSpecification _:
                case QuaternionOrientedLtpEnuSpecification _:
                case LtpEnuSpecification _:
                case LtpNedSpecification _:
                    return true;
                default:
                    return false;
            }
        }
    }
}