using System;
using Ethar.GeoPose.Authority;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Exceptions;
using Ethar.GeoPose.FrameSpecifications;
using Ethar.GeoPose.Interfaces;
using Ethar.GeoPose.StructuralDataUnits;
using Ethar.GeoPose.TransitionModels;
using Ethar.GeoPose.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ethar.GeoPose.Examples
{
    /// <summary>
    /// This example shows how to implement the various classes required for a developer to implement their own custom frame specifications managed by a custom Authority.
    /// </summary>
    public class Example_AuthorityImplementation
    {
        public static string SerializeExample()
        {
            var auth = new ExampleAuthority();

            // We have to register the authority for serial/deserialization to work for custom frame specifications.
            AuthorityProvider.RegisterAuthority(auth);

            var sdu = new AdvancedSdu(1,
                new UnitQuaternion(1, 2, 1, 2),
                new ExampleExtrinsicFrameSpec());

            // Now that the authority is implemented and registered we can serialize/deserialize custom frame specs.
            return JsonConvert.SerializeObject(sdu);
        }

        public static AdvancedSdu DeserializeExample()
        {
            var auth = new ExampleAuthority();

            // We have to register the authority for serial/deserialization to work for custom frame specifications.
            AuthorityProvider.RegisterAuthority(auth);

            var json =
                "{" +
                    "\"frameSpecification\":" +
                    "{" +
                        $"\"authority\": \"{Constants.AuthorityName}\"," +
                        $"\"id\": \"ExampleExtrinsicFrameSpec\"," +
                        $"\"parameters\": \"latitude=1&longitude=2\"" +
                    "}," +
                    "\"quaternion\":" +
                    "{" +
                        $"\"x\": 1," +
                        $"\"y\": 0," +
                        $"\"z\": 0," +
                        $"\"w\": 1" +
                    "}," +
                    $"\"validTime\": 1" +
                "}";

            // Now that the authority is implemented and registered we can serialize/deserialize custom frame specs.
            return JsonConvert.DeserializeObject<AdvancedSdu>(json);
        }
    }

    /// <summary>
    /// This class is an example implementation of an authority. An authority is a GeoPose concept wherein individuals/groups can implement their
    /// own frame specification definitions. An implementor may want to implement their own authority if the basic SDUs do not satisfy the
    /// implementors geospatial data requirements.
    /// </summary>
    internal class ExampleAuthority : IAuthority
    {
        private IExplicitFrameSpecificationValidator validator = new ExampleAuthorityFrameSpecificationValidator();

        public string AuthorityName => Constants.AuthorityName;

        public JObject ConvertFrameSpecToJson(IFrameSpecification frameSpec)
        {
            switch (frameSpec)
            {
                case ExampleExtrinsicFrameSpec extrinsic:
                    return ExampleFrameSpecificationConverter.ConvertExampleExtrinsicSpecToJObject(extrinsic);
                case ExampleIntrinsicFrameSpec intrinsic:
                    return ExampleFrameSpecificationConverter.ConvertExampleIntrinsicSpecToJObject(intrinsic); ;
                default:
                    throw new NotImplementedException($"The frame specification does not exist in the authority {AuthorityName}");
            }
        }

        public IFrameSpecification ConvertJsonToFrameSpec(JObject jsonObject)
        {
            var id = (string)jsonObject["id"];
            if (string.IsNullOrEmpty(id))
            {
                throw new NotImplementedException();
            }

            IFrameSpecification frameSpec;
            FrameSpecificationValidationResult validationResult;

            switch (id)
            {
                // Implementors could add optional validation here to ensure that an invalid frame specification does not get created.
                // This is not required but an example is shown below.
                case "ExampleExtrinsicFrameSpec":
                    frameSpec = ExampleFrameSpecificationConverter.ConvertJObjectToExampleExtrinsicFrameSpec(jsonObject);
                    validationResult = this.validator.Validate(frameSpec);
                    return validationResult.IsValid ? frameSpec : throw new FrameSpecificationInvalidException(validationResult.Message);
                case "ExampleIntrinsicFrameSpec":
                    frameSpec = ExampleFrameSpecificationConverter.ConvertJObjectToExampleIntrinsicFrameSpec(jsonObject);
                    validationResult = this.validator.Validate(frameSpec);
                    return validationResult.IsValid ? frameSpec : throw new FrameSpecificationInvalidException(validationResult.Message);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <remarks>
        /// This example does not have transition models, though the conversion could be implemented in the same way as the frame specifications.
        /// </remarks>>
        public TransitionModel ConvertJsonToTransitionModel(JObject jsonObject)
        {
            throw new NotImplementedException();
        }

        public JObject ConvertTransitionModelToJson(TransitionModel transitionModel)
        {
            throw new NotImplementedException();
        }

        public bool IsFrameSpecificationExtrinsic(IFrameSpecification frameSpec)
        {
            switch (frameSpec)
            {
                case ExampleExtrinsicFrameSpec _:
                    return true;
                case ExampleIntrinsicFrameSpec _:
                    return false;
                default:
                    throw new NotImplementedException("Frame specification does not exist in the authority.");
            }
        }
    }

    /// <summary>
    /// This class represents an example frame specification that has a latitude and longitude. This is different than one of the basic SDUs because
    /// it has no rotation. This specification is extrinsic because it is referenced from the earth rather than from another frame specification.
    /// </summary>
    /// <example>
    /// {
    ///     id = "ExampleExtrinsicFrameSpec",
    ///     authority = "ExampleAuthority/1.0",
    ///     parameters = "latitude=1&longitude=2"
    /// }
    /// </example>
    internal class ExampleExtrinsicFrameSpec : BaseFrameSpecification
    {
        public ExampleExtrinsicFrameSpec()
            : base("ExampleExtrinsicFrameSpecification", Constants.AuthorityName)
        {
        }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    /// <summary>
    /// This class represents an example frame specification that has a translation relative to another frame specification in meters
    /// with the x value representing east, the y value representing north, and z representing up. This type of frame specification could
    /// be useful if you have an object that should always be positioned relative to another.
    /// </summary>
    /// <example>
    /// {
    ///     id = "ExampleIntrinsicFrameSpec",
    ///     authority = "ExampleAuthority/1.0",
    ///     parameters = "translation.x=1&translation.y=2&translation.z=3"
    /// }
    /// </example>
    internal class ExampleIntrinsicFrameSpec : BaseFrameSpecification
    {
        public ExampleIntrinsicFrameSpec()
            : base("ExampleIntrinsicFrameSpecification", Constants.AuthorityName)
        {
        }

        public UnitVector3 Translation { get; set; }
    }

    /// <summary>
    /// This class converts the frame specifications that this authority implements to and from json. It is not required to use this
    /// pattern for your conversion, this is just one of many ways that it can be implemented.
    /// </summary>
    internal class ExampleFrameSpecificationConverter
    {
        public static ExampleExtrinsicFrameSpec ConvertJObjectToExampleExtrinsicFrameSpec(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<ExampleExtrinsicFrameSpec>(jObject, out var queryString))
            {
                var lat = float.Parse(queryString.GetParameter("latitude"));
                var lon = float.Parse(queryString.GetParameter("longitude"));

                return new ExampleExtrinsicFrameSpec { Latitude = lat, Longitude = lon };
            }

            return null;
        }

        public static JObject ConvertExampleExtrinsicSpecToJObject(ExampleExtrinsicFrameSpec spec)
        {
            var paramString = $"latitude={spec.Latitude}&longitude={spec.Longitude}";

            var jObj = new JObject
            {
                { "authority", spec.Authority },
                { "id", spec.Id },
                { "parameters", paramString },
            };

            return jObj;
        }

        public static ExampleIntrinsicFrameSpec ConvertJObjectToExampleIntrinsicFrameSpec(JObject jObject)
        {
            if (ValidationUtilities.ValidateJsonObjectParameters<ExampleIntrinsicFrameSpec>(jObject, out var queryString))
            {
                var x = float.Parse(queryString.GetParameter("translation.x"));
                var y = float.Parse(queryString.GetParameter("translation.y"));
                var z = float.Parse(queryString.GetParameter("translation.z"));
                var vector = new UnitVector3(x, y, z);

                return new ExampleIntrinsicFrameSpec { Translation = vector };
            }

            return null;
        }

        public static JObject ConvertExampleIntrinsicSpecToJObject(ExampleIntrinsicFrameSpec spec)
        {
            var paramString = $"translation.x={spec.Translation.X}&translation.y={spec.Translation.Y}&translation.z={spec.Translation.Z}";

            var jObj = new JObject
            {
                { "authority", spec.Authority },
                { "id", spec.Id },
                { "parameters", paramString },
            };

            return jObj;
        }
    }

    /// <summary>
    /// This example shows a basic validator. This class is not required to implement and is given as a reference in case implementors desire validation for frame specifications.
    /// </summary>
    internal class ExampleAuthorityFrameSpecificationValidator : IExplicitFrameSpecificationValidator
    {
        public FrameSpecificationValidationResult Validate(IFrameSpecification frame)
        {
            switch (frame)
            {
                case ExampleExtrinsicFrameSpec extrinsic:
                    if (extrinsic.Latitude > 90 || extrinsic.Latitude < -90) return new FrameSpecificationValidationResult(false, "Latitude must be between -90 and 90");
                    return FrameSpecificationValidationResult.Valid;
                default:
                    return FrameSpecificationValidationResult.Valid;
            }
        }
    }

    internal static class Constants
    {
        public static string AuthorityName => "ExampleAuthority/1.0";
    }
}
