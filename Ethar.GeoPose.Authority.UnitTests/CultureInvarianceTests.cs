using System.Globalization;
using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.Authority.Ogc;
using Ethar.GeoPose.Authority.TransitionModels;
using Ethar.GeoPose.Conventions;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Exceptions;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.Geodesy;
using Ethar.GeoPose.JsonConversion;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Ethar.GeoPose.Authority.UnitTests
{
    /// <summary>
    /// Proves that nothing the library writes or reads depends on the thread culture. Every test runs once per culture in
    /// <see cref="Cultures"/>, covering period-decimal, comma-decimal, Arabic-comma and non-Latin-digit locales, and the invariant culture.
    /// The "hazard" tests show that the runtime really does misparse under those cultures, so a passing invariant test is meaningful.
    /// </summary>
    [TestFixture]
    internal class CultureInvarianceTests
    {
        /// <summary>
        /// Locales the tests run under. Comma-decimal locales are where version 1 broke; the others guard against regressions in either direction.
        /// </summary>
        public static readonly string[] Cultures =
        {
            "de-DE", "fr-FR", "es-ES", "it-IT", "nl-NL", "pt-BR", "ru-RU", "tr-TR", "sv-SE", "pl-PL", "cs-CZ",
            "ar-SA", "fa-IR", "hi-IN", "ja-JP", "zh-CN", "en-US", "en-GB", "",
        };

        private static readonly TangentPointPosition Paris = new TangentPointPosition(48.8566, 2.3522, 35.5);
        private static readonly YawPitchRollAngles Angles = new YawPitchRollAngles(-12.5, 0.25, 90);
        private static readonly UnitQuaternion Quaternion = new UnitQuaternion(0.20056154657066608, -0.08111602541464237, 0.36606032744426537, -0.9050939692261301);
        private static readonly UnitVector3 Translation = new UnitVector3(1.5, -2.5, 0);

        private CultureInfo originalCulture;
        private CultureInfo originalUiCulture;

        [OneTimeSetUp]
        public void RegisterAuthorities()
        {
            GeoPoseAuthorities.RegisterDefaults();
        }

        [SetUp]
        public void RememberCulture()
        {
            this.originalCulture = CultureInfo.CurrentCulture;
            this.originalUiCulture = CultureInfo.CurrentUICulture;
        }

        [TearDown]
        public void RestoreCulture()
        {
            CultureInfo.CurrentCulture = this.originalCulture;
            CultureInfo.CurrentUICulture = this.originalUiCulture;
        }

        // ---- Hazard: the runtime is culture-sensitive, so the invariance tests below are not vacuous. ----

        [Test]
        public void HazardCommaDecimalCulturesMisreadPeriodDecimals()
        {
            Use("de-DE");

            Assert.That((48.85).ToString(), Is.EqualTo("48,85"), "German formats with a comma");
            Assert.That(double.Parse("48.85"), Is.EqualTo(4885), "German reads a period as a thousands separator");
            Assert.That(double.TryParse("48.85", NumberStyles.Float, CultureInfo.CurrentCulture, out var strict) && strict == 48.85, Is.False);
        }

        [Test]
        public void HazardFrenchFormatsWithCommaAndNarrowSpace()
        {
            Use("fr-FR");

            Assert.That((1234.5).ToString(), Does.Contain(","));
            Assert.That(double.TryParse("0.5", out var parsed) && parsed == 0.5, Is.False, "French does not read a period as the decimal separator");
        }

        // ---- Position ----

        [TestCaseSource(nameof(Cultures))]
        public void PositionParameterStringIsInvariant(string culture)
        {
            Use(culture);

            Assert.That(Paris.BuildParamString(), Is.EqualTo("latitude=48.8566&longitude=2.3522&heightInMeters=35.5"));
            Assert.That(ParamStringBuilder.BuildParamString(Paris), Is.EqualTo("lat=48.8566&lon=2.3522&h=35.5"));
            Assert.That(Paris.ToString(), Is.EqualTo("Latitude:48.8566, Longitude:2.3522, HeightInMeters:35.5"));
        }

        [TestCaseSource(nameof(Cultures))]
        public void PositionJsonIsInvariant(string culture)
        {
            Use(culture);

            var json = JsonConvert.SerializeObject(Paris);

            Assert.That(json, Is.EqualTo("{\"lat\":48.8566,\"lon\":2.3522,\"h\":35.5}"));
            Assert.That(JsonConvert.DeserializeObject<TangentPointPosition>(json), Is.EqualTo(Paris));
        }

        [TestCaseSource(nameof(Cultures))]
        public void GeodesyIsInvariant(string culture)
        {
            Use(culture);

            var offset = new TangentPointPosition(48.8576, 2.3522, 35.5).EnuOffsetFrom(Paris);

            Assert.That(offset.North, Is.EqualTo(111.2).Within(0.1));
            Assert.That(offset.ToString(), Does.StartWith("East:").And.Not.Match(@"\d,\d"), "ToString must not use the culture's decimal separator");
            Assert.That(Paris.ToEcef().ToString(), Does.Not.Match(@"\d,\d"));
        }

        // ---- Orientation ----

        [TestCaseSource(nameof(Cultures))]
        public void OrientationParameterStringsAreInvariant(string culture)
        {
            Use(culture);

            Assert.That(Angles.BuildOrientationParamString(), Is.EqualTo("orientation.yaw=-12.5&orientation.pitch=0.25&orientation.roll=90"));
            Assert.That(Quaternion.BuildOrientationParamString(), Is.EqualTo("orientation.x=0.20056154657066608&orientation.y=-0.08111602541464237&orientation.z=0.36606032744426537&orientation.w=-0.9050939692261301"));
            Assert.That(Quaternion.BuildRotationParamString(), Does.StartWith("rotation.x=0.20056154657066608&"));
            Assert.That(Translation.BuildTranslationParamString(), Is.EqualTo("translation.x=1.5&translation.y=-2.5&translation.z=0"));
            Assert.That(Angles.ToString(), Is.EqualTo("Yaw:-12.5, Pitch:0.25, Roll:90"));
            Assert.That(Quaternion.ToString(), Is.EqualTo("X:0.20056154657066608, Y:-0.08111602541464237, Z:0.36606032744426537, W:-0.9050939692261301"));
        }

        [TestCaseSource(nameof(Cultures))]
        public void OrientationConversionsAreInvariant(string culture)
        {
            Use(culture);

            var quaternion = Angles.ToQuaternion();
            var back = quaternion.ToYawPitchRoll();

            Assert.That(back.Yaw, Is.EqualTo(Angles.Yaw).Within(1e-9));
            Assert.That(CompassHeading.ToGeoPoseYaw(45.5), Is.EqualTo(44.5));
            Assert.That(new EnuVector(1.5, 2.5, 0).ToLeftHandedYUp(90).X, Is.EqualTo(-2.5).Within(1e-12));
        }

        [TestCaseSource(nameof(Cultures))]
        public void BasicSdusRoundTripAsInvariantJson(string culture)
        {
            Use(culture);

            var ypr = new BasicYawPitchRollSdu(Angles, Paris);
            var quat = new BasicQuaternionSdu(Paris, Quaternion);

            var yprJson = JsonConvert.SerializeObject(ypr);
            var quatJson = JsonConvert.SerializeObject(quat);

            Assert.That(yprJson, Is.EqualTo("{\"position\":{\"lat\":48.8566,\"lon\":2.3522,\"h\":35.5},\"angles\":{\"yaw\":-12.5,\"pitch\":0.25,\"roll\":90.0}}"));
            Assert.That(quatJson, Does.Contain("\"w\":-0.9050939692261301"));
            Assert.That(JsonConvert.DeserializeObject<BasicYawPitchRollSdu>(yprJson), Is.EqualTo(ypr));
            Assert.That(JsonConvert.DeserializeObject<BasicQuaternionSdu>(quatJson), Is.EqualTo(quat));
        }

        // ---- Structural data units carrying frame specifications, both authorities ----

        [TestCaseSource(nameof(Cultures))]
        public void EtharAdvancedSduRoundTrips(string culture)
        {
            Use(culture);
            var sdu = new AdvancedSdu(1630560671227L, Quaternion, new YawPitchRollOrientedLtpEnuSpecification(Paris, Angles));

            var json = JsonConvert.SerializeObject(sdu);

            Assert.That(json, Does.Contain("latitude=48.8566&longitude=2.3522&heightInMeters=35.5&orientation.yaw=-12.5&orientation.pitch=0.25&orientation.roll=90"));
            Assert.That(JsonConvert.DeserializeObject<AdvancedSdu>(json), Is.EqualTo(sdu));
        }

        [TestCaseSource(nameof(Cultures))]
        public void EtharChainWithTranslateRotateRoundTrips(string culture)
        {
            Use(culture);
            var sdu = new ChainSdu(
                7,
                new LtpEnuSpecification(Paris),
                new List<Ethar.GeoPose.FrameSpecifications.BaseFrameSpecification> { new TranslateRotateSpecification(Translation, Quaternion), new QuaternionOrientedLtpEnuSpecification(Paris, Quaternion) });

            var json = JsonConvert.SerializeObject(sdu);

            Assert.That(json, Does.Contain("translation.x=1.5&translation.y=-2.5&translation.z=0&rotation.x=0.20056154657066608"));
            Assert.That(JsonConvert.DeserializeObject<ChainSdu>(json), Is.EqualTo(sdu));
        }

        [TestCaseSource(nameof(Cultures))]
        public void OgcAdvancedSduRoundTrips(string culture)
        {
            Use(culture);
            var json = "{\"frameSpecification\":{\"authority\":\"/geopose/1.0\",\"id\":\"LTP-ENU\",\"parameters\":\"longitude=-122.3000000&latitude=47.7000000&height=11.000\"},\"quaternion\":{\"x\":0.2,\"y\":-0.08,\"z\":0.37,\"w\":-0.9},\"validTime\":1630560671227}";

            var sdu = JsonConvert.DeserializeObject<AdvancedSdu>(json);
            var written = JsonConvert.SerializeObject(sdu);

            Assert.That(((LtpEnuSpecification)sdu.FrameSpecification).Position, Is.EqualTo(new TangentPointPosition(47.7, -122.3, 11)));
            Assert.That(written, Does.Contain("\"parameters\":\"longitude=-122.3&latitude=47.7&height=11\""));
            Assert.That(JsonConvert.DeserializeObject<AdvancedSdu>(written), Is.EqualTo(sdu));
        }

        [TestCaseSource(nameof(Cultures))]
        public void OgcTranslateRotateRoundTrips(string culture)
        {
            Use(culture);
            var json = "{\"authority\":\"/geopose/1.0\",\"id\":\"RotateTranslate\",\"parameters\":\"translation=[0.5, 0.0, -1.25]&rotation=[-0.90510, 0.20057, -0.08112, 0.36605]\"}";
            var authority = new OgcGeoPoseAuthority();

            var spec = (TranslateRotateSpecification)authority.ConvertJsonToFrameSpec(JObject.Parse(json));
            var written = authority.ConvertFrameSpecToJson(spec);

            Assert.That(spec.Translation, Is.EqualTo(new UnitVector3(0.5, 0, -1.25)));
            Assert.That(spec.Rotation, Is.EqualTo(new UnitQuaternion(0.20057, -0.08112, 0.36605, -0.90510)));
            Assert.That((string)written["parameters"], Is.EqualTo("translation=[0.5, 0, -1.25]&rotation=[-0.9051, 0.20057, -0.08112, 0.36605]"));
        }

        [TestCaseSource(nameof(Cultures))]
        public void RegularSeriesWithTransitionModelRoundTrips(string culture)
        {
            Use(culture);
            var header = new SeriesHeader(new InterpolatedTransitionModel(), 2, 1000, 2000, "sha");
            var sdu = new RegularSeriesSdu(header, new GeoPoseDuration { NumericDuration = 500 }, new LtpEnuSpecification(Paris), new List<Ethar.GeoPose.FrameSpecifications.BaseFrameSpecification> { new TranslateRotateSpecification(Translation, Quaternion) }, new SeriesTrailer(2, "sha"));

            var json = JsonConvert.SerializeObject(sdu);

            Assert.That(json, Does.Contain("\"interPoseDuration\":500"));
            Assert.That(JsonConvert.DeserializeObject<RegularSeriesSdu>(json), Is.EqualTo(sdu));
        }

        // ---- Rejection instead of silent corruption ----

        [TestCaseSource(nameof(Cultures))]
        public void CommaDecimalParametersAreRejected(string culture)
        {
            Use(culture);
            var json = "{\"authority\":\"/Ethar.GeoPose/1.0\",\"id\":\"LTP-ENU\",\"parameters\":\"latitude=48,8566&longitude=2,3522&heightInMeters=35\"}";

            Assert.Throws<FrameSpecificationInvalidException>(() => new EtharGeoPoseAuthority().ConvertJsonToFrameSpec(JObject.Parse(json)));
        }

        [TestCaseSource(nameof(Cultures))]
        public void InvariantNumberParsesAndFormatsIdentically(string culture)
        {
            Use(culture);

            Assert.That(InvariantNumber.Parse("-122.3000000", "lon"), Is.EqualTo(-122.3));
            Assert.That(InvariantNumber.Parse("1e3", "v"), Is.EqualTo(1000));
            Assert.That(InvariantNumber.Format(-0.9050939692261301), Is.EqualTo("-0.9050939692261301"));
            Assert.That(InvariantNumber.FormatArray(0, 1.5, -2), Is.EqualTo("[0, 1.5, -2]"));
            Assert.That(InvariantNumber.ParseArray("[0.0, 1.5 ,-2]", "v", 3), Is.EqualTo(new[] { 0.0, 1.5, -2.0 }));
        }

        [Test]
        public void EtharAuthorityAcceptsHeightAsAnAliasOnInput()
        {
            var json = "{\"authority\":\"/Ethar.GeoPose/1.0\",\"id\":\"LTP-ENU\",\"parameters\":\"latitude=48.8566&longitude=2.3522&height=35.5\"}";

            var spec = (LtpEnuSpecification)new EtharGeoPoseAuthority().ConvertJsonToFrameSpec(JObject.Parse(json));

            Assert.That(spec.Position.HeightInMeters, Is.EqualTo(35.5));
        }

        private static void Use(string culture)
        {
            var info = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(culture);
            CultureInfo.CurrentCulture = info;
            CultureInfo.CurrentUICulture = info;
        }
    }
}
