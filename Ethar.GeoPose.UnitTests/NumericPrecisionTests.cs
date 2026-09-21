using Ethar.GeoPose.Authority;
using Ethar.GeoPose.Authority.Ogc;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.Geodesy;
using Ethar.GeoPose.JsonConversion;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    /// <summary>
    /// Proves how integer, single and double precision inputs flow through the library, and that double precision is preserved end to end.
    /// </summary>
    [TestFixture]
    internal class NumericPrecisionTests
    {
        private const double SeventeenDigits = -0.9050939692261301;

        [OneTimeSetUp]
        public void RegisterAuthorities()
        {
            GeoPoseAuthorities.RegisterDefaults();
        }

        [Test]
        public void IntegerInputsAreExactDoubles()
        {
            var position = new TangentPointPosition(47, -122, 11);
            var angles = new YawPitchRollAngles(90, 45, 0);

            Assert.That(position.Latitude, Is.EqualTo(47.0));
            Assert.That(position.Longitude, Is.EqualTo(-122.0));
            Assert.That(angles.Yaw, Is.EqualTo(90.0));
            Assert.That(JsonConvert.SerializeObject(position), Is.EqualTo("{\"lat\":47.0,\"lon\":-122.0,\"h\":11.0}"));
        }

        [Test]
        public void JsonIntegerLiteralsDeserializeAsDoubles()
        {
            var sdu = JsonConvert.DeserializeObject<BasicYawPitchRollSdu>("{\"position\":{\"lat\":47,\"lon\":-122,\"h\":11},\"angles\":{\"yaw\":5,\"pitch\":0,\"roll\":0}}");

            Assert.That(sdu.Position, Is.EqualTo(new TangentPointPosition(47, -122, 11)));
            Assert.That(sdu.Angles, Is.EqualTo(new YawPitchRollAngles(5, 0, 0)));
        }

        [Test]
        public void SinglePrecisionInputsAreWidenedNotRepaired()
        {
            // A caller that measured in float has already lost the precision; the library keeps exactly what it was given.
            var fromFloat = new TangentPointPosition(48.8566f, 2.3522f, 35.5f);

            Assert.That(fromFloat.Latitude, Is.EqualTo((double)48.8566f));
            Assert.That(fromFloat.Latitude, Is.Not.EqualTo(48.8566));
            Assert.That(Math.Abs(fromFloat.Latitude - 48.8566), Is.GreaterThan(1e-7).And.LessThan(1e-5));
            Assert.That(fromFloat.HeightInMeters, Is.EqualTo(35.5), "values exactly representable in float are exact");
        }

        [Test]
        public void SinglePrecisionLatitudeLosesAboutHalfAMetre()
        {
            var exact = new TangentPointPosition(48.8566, 2.3522, 0);
            var truncated = new TangentPointPosition((float)48.8566, (float)2.3522, 0);

            var error = GeodeticConverter.GeodeticToEnu(truncated, exact).Length;

            Assert.That(error, Is.GreaterThan(0.001).And.LessThan(1.0), $"float latitude error was {error} m");
        }

        [Test]
        public void SinglePrecisionEcefCannotResolveBelowAQuarterMetre()
        {
            var ecef = GeodeticConverter.GeodeticToEcef(new TangentPointPosition(51.5074, -0.1278, 0));

            var asFloat = (float)ecef.X;
            var ulp = MathF.BitIncrement(asFloat) - asFloat;

            Assert.That(ulp, Is.GreaterThanOrEqualTo(0.25f), "the spacing between adjacent floats at ECEF magnitudes");
        }

        [Test]
        public void DoublePrecisionEcefRoundTripIsSubMicrometre()
        {
            var original = new TangentPointPosition(51.5074, -0.1278, 35.25);

            var back = GeodeticConverter.EcefToGeodetic(GeodeticConverter.GeodeticToEcef(original));
            var error = GeodeticConverter.GeodeticToEnu(back, original).Length;

            Assert.That(error, Is.LessThan(1e-6));
        }

        [Test]
        public void EnuRoundTripAtContinentalDistanceIsSubMillimetre()
        {
            var london = new TangentPointPosition(51.5074, -0.1278, 0);
            var sydney = new TangentPointPosition(-33.8688, 151.2093, 58);

            var offset = sydney.EnuOffsetFrom(london);
            var back = london.OffsetBy(offset);

            Assert.That(offset.Length, Is.GreaterThan(10_000_000));
            Assert.That(GeodeticConverter.GeodeticToEnu(back, sydney).Length, Is.LessThan(1e-3));
        }

        [Test]
        public void SeventeenSignificantDigitsSurviveJson()
        {
            var sdu = new BasicQuaternionSdu(new TangentPointPosition(47.7, -122.3, 11.5), new UnitQuaternion(0.20056154657066608, -0.08111602541464237, 0.36606032744426537, SeventeenDigits));

            var back = JsonConvert.DeserializeObject<BasicQuaternionSdu>(JsonConvert.SerializeObject(sdu));

            Assert.That(back.Quaternion.W, Is.EqualTo(SeventeenDigits));
            Assert.That(back.Quaternion.X, Is.EqualTo(0.20056154657066608));
            Assert.That(back, Is.EqualTo(sdu));
        }

        [Test]
        public void SeventeenSignificantDigitsSurviveParameterStrings()
        {
            Assert.That(InvariantNumber.Parse(InvariantNumber.Format(SeventeenDigits), "w"), Is.EqualTo(SeventeenDigits));
            Assert.That(InvariantNumber.ParseArray(InvariantNumber.FormatArray(SeventeenDigits, 1e-300, 1e300), "a", 3), Is.EqualTo(new[] { SeventeenDigits, 1e-300, 1e300 }));
        }

        [Test]
        public void SeventeenSignificantDigitsSurviveBothAuthorities()
        {
            var ethar = new AdvancedSdu(1, new UnitQuaternion(0.1, 0.2, 0.3, SeventeenDigits), new Authority.FrameSpecifications.QuaternionOrientedLtpEnuSpecification(new TangentPointPosition(48.8566123456789, 2.3522987654321, 35.123456789), new UnitQuaternion(SeventeenDigits, 0, 0, 0.1)));
            var ogc = new AdvancedSdu(1, new UnitQuaternion(0.1, 0.2, 0.3, SeventeenDigits), new OgcTranslateRotateSpecification(new UnitVector3(1e-9, 2.5, SeventeenDigits), new UnitQuaternion(SeventeenDigits, 0.20056154657066608, 0, 0)));

            Assert.That(JsonConvert.DeserializeObject<AdvancedSdu>(JsonConvert.SerializeObject(ethar)), Is.EqualTo(ethar));
            Assert.That(JsonConvert.DeserializeObject<AdvancedSdu>(JsonConvert.SerializeObject(ogc)), Is.EqualTo(ogc));
        }

        [Test]
        public void OgcArrayParametersAcceptIntegerAndExponentForms()
        {
            var json = JObject.Parse("{\"authority\":\"/geopose/1.0\",\"id\":\"RotateTranslate\",\"parameters\":\"translation=[1, -2, 3]&rotation=[1, 0, 0, 5e-1]\"}");

            var spec = (Authority.FrameSpecifications.TranslateRotateSpecification)new OgcGeoPoseAuthority().ConvertJsonToFrameSpec(json);

            Assert.That(spec.Translation, Is.EqualTo(new UnitVector3(1, -2, 3)));
            Assert.That(spec.Rotation, Is.EqualTo(new UnitQuaternion(0, 0, 0.5, 1)));
        }

        [Test]
        public void ExtremeButValidCoordinatesAreAccepted()
        {
            var poleEast = new TangentPointPosition(90, 180, -500);
            var poleWest = new TangentPointPosition(-90, -180, 9000);

            Assert.That(poleEast.Validate().IsValid, Is.True);
            Assert.That(poleWest.Validate().IsValid, Is.True);
            Assert.That(new TangentPointPosition(90.0000001, 0, 0).Validate().IsValid, Is.False);
            Assert.That(new TangentPointPosition(0, -180.0000001, 0).Validate().IsValid, Is.False);
        }
    }
}
