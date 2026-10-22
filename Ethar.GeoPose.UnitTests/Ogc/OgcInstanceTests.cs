using Ethar.GeoPose.Authority;
using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.Authority.Ogc;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.FrameSpecifications;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests.Ogc
{
    /// <summary>
    /// Round trips the official OGC GeoPose 1.0 example instances (https://schemas.opengis.net/geopose/1.0/instances/) through the library
    /// with the <c>/geopose/1.0</c> authority registered.
    /// </summary>
    [TestFixture]
    internal class OgcInstanceTests
    {
        private static readonly string FixtureDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", "Ogc");

        [OneTimeSetUp]
        public void RegisterAuthorities()
        {
            GeoPoseAuthorities.RegisterDefaults();
        }

        [TestCase("GeoPose.Basic.YPR.Instance.00.json", 5.514456741060452, -0.43610515937237904, 0.0)]
        [TestCase("GeoPose.Basic.YPR.Instance.03.json", 5.527127708845192, -0.44220204512692407, 0.0)]
        public void BasicYprInstancesRoundTrip(string file, double yaw, double pitch, double roll)
        {
            var sdu = JsonConvert.DeserializeObject<BasicYawPitchRollSdu>(Load(file));

            Assert.That(sdu.Position, Is.EqualTo(new TangentPointPosition(47.7, -122.3, 11.5)));
            Assert.That(sdu.Angles, Is.EqualTo(new YawPitchRollAngles(yaw, pitch, roll)));
            Assert.That(JsonConvert.DeserializeObject<BasicYawPitchRollSdu>(JsonConvert.SerializeObject(sdu)), Is.EqualTo(sdu));
        }

        [TestCase("GeoPose.Basic.Quaternion.Instance.00.json")]
        [TestCase("GeoPose.Basic.Strict_Quaternion.Instance.02.json")]
        public void BasicQuaternionInstancesRoundTripWithFullPrecision(string file)
        {
            var json = Load(file);
            var sdu = JsonConvert.DeserializeObject<BasicQuaternionSdu>(json);

            var expected = JObject.Parse(json)["quaternion"];
            Assert.That(sdu.Quaternion.X, Is.EqualTo((double)expected["x"]));
            Assert.That(sdu.Quaternion.W, Is.EqualTo((double)expected["w"]));
            Assert.That(JsonConvert.DeserializeObject<BasicQuaternionSdu>(JsonConvert.SerializeObject(sdu)), Is.EqualTo(sdu));
        }

        [Test]
        public void AdvancedInstanceParsesTheOgcLtpEnuFrame()
        {
            var sdu = JsonConvert.DeserializeObject<AdvancedSdu>(Load("GeoPose.Advanced.Instance.json"));

            Assert.That(sdu.ValidTime, Is.EqualTo(1630560671227L));
            Assert.That(sdu.FrameSpecification, Is.InstanceOf<OgcLtpEnuSpecification>());
            var frame = (LtpEnuSpecification)sdu.FrameSpecification;
            Assert.That(frame.Authority, Is.EqualTo("/geopose/1.0"));
            Assert.That(frame.Id, Is.EqualTo("LTP-ENU"));
            Assert.That(frame.Position.Longitude, Is.EqualTo(-122.3));
            Assert.That(frame.Position.Latitude, Is.EqualTo(47.7));
            Assert.That(frame.Position.HeightInMeters, Is.EqualTo(11.0));
            Assert.That(sdu.Quaternion.W, Is.EqualTo(-0.9050939692261301));

            var roundTrip = JsonConvert.DeserializeObject<AdvancedSdu>(JsonConvert.SerializeObject(sdu));
            Assert.That(roundTrip, Is.EqualTo(sdu));
        }

        [Test]
        public void AdvancedInstanceSerializesWithTheOgcGrammar()
        {
            var sdu = JsonConvert.DeserializeObject<AdvancedSdu>(Load("GeoPose.Advanced.Instance.json"));

            var json = JObject.Parse(JsonConvert.SerializeObject(sdu));

            Assert.That((string)json["frameSpecification"]["authority"], Is.EqualTo("/geopose/1.0"));
            Assert.That((string)json["frameSpecification"]["id"], Is.EqualTo("LTP-ENU"));
            Assert.That((string)json["frameSpecification"]["parameters"], Is.EqualTo("longitude=-122.3&latitude=47.7&height=11"));
        }

        [Test]
        public void ChainInstanceParsesPathFormIdsAndArrayParameters()
        {
            var sdu = JsonConvert.DeserializeObject<ChainSdu>(Load("GeoPose.Composite.Chain.Instance.json"));

            Assert.That(sdu.OuterFrame.Id, Is.EqualTo("/Extrinsic/LTP-ENU"));
            Assert.That(sdu.OuterFrame, Is.InstanceOf<OgcLtpEnuSpecification>());
            Assert.That(sdu.FrameChain.Count, Is.EqualTo(3));
            var link = (TranslateRotateSpecification)sdu.FrameChain[0];
            Assert.That(link.Id, Is.EqualTo("/Intrinsic/Translate-Rotate"));
            Assert.That(link.Translation, Is.EqualTo(UnitVector3.Zero));

            // rotation=[0.69291, 0.69291, 0.14097, 0.14097] is w, x, y, z.
            Assert.That(link.Rotation.W, Is.EqualTo(0.69291));
            Assert.That(link.Rotation.X, Is.EqualTo(0.69291));
            Assert.That(link.Rotation.Y, Is.EqualTo(0.14097));
            Assert.That(link.Rotation.Z, Is.EqualTo(0.14097));

            Assert.That(sdu.Validate(), Is.True, "the outer frame is extrinsic");
            Assert.That(JsonConvert.DeserializeObject<ChainSdu>(JsonConvert.SerializeObject(sdu)), Is.EqualTo(sdu));
        }

        [Test]
        public void ChainInstanceSerializesArraysInTheOgcForm()
        {
            var sdu = JsonConvert.DeserializeObject<ChainSdu>(Load("GeoPose.Composite.Chain.Instance.json"));

            var json = JObject.Parse(JsonConvert.SerializeObject(sdu));

            Assert.That((string)json["frameChain"][0]["parameters"], Is.EqualTo("translation=[0, 0, 0]&rotation=[0.69291, 0.69291, 0.14097, 0.14097]"));
        }

        [Test]
        public void GraphInstanceParsesAndValidates()
        {
            var sdu = JsonConvert.DeserializeObject<GraphSdu>(Load("GeoPose.Composite.Graph.Instance.json"));

            Assert.That(sdu.FrameList.Count, Is.EqualTo(7));
            Assert.That(sdu.TransformList.Count, Is.EqualTo(6));
            Assert.That(sdu.TransformList[3], Is.EqualTo(new FrameTransformIndexPair(new[] { 0, 4 })));
            Assert.That(sdu.Validate(), Is.True);
            Assert.That(JsonConvert.DeserializeObject<GraphSdu>(JsonConvert.SerializeObject(sdu)), Is.EqualTo(sdu));
        }

        [Test]
        public void RegularSeriesInstanceParsesTheNoneTransitionModel()
        {
            var sdu = JsonConvert.DeserializeObject<RegularSeriesSdu>(Load("GeoPose.Composite.Sequence.Series.Regular.Instance.json"));

            Assert.That(sdu.Header.TransitionModel, Is.InstanceOf<OgcNoneTransitionModel>());
            Assert.That(sdu.Header.PoseCount, Is.EqualTo(2));
            Assert.That(sdu.InterPoseDuration.NumericDuration, Is.EqualTo(1000L));
            Assert.That(sdu.InnerFrameSeries.Count(), Is.EqualTo(3));
            Assert.That(sdu.InnerFrameSeries.First().Id, Is.EqualTo("RotateTranslate"));
            Assert.That(((TranslateRotateSpecification)sdu.InnerFrameSeries.Last()).Translation.X, Is.EqualTo(1.0));
            Assert.That(JsonConvert.DeserializeObject<RegularSeriesSdu>(JsonConvert.SerializeObject(sdu)), Is.EqualTo(sdu));
        }

        [Test]
        public void IrregularSeriesInstanceRoundTrips()
        {
            var sdu = JsonConvert.DeserializeObject<IrregularSeriesSdu>(Load("GeoPose.Composite.Sequence.Series.Irregular.Instance.json"));

            Assert.That(sdu.InnerFrameAndTimeSeries.Count(), Is.EqualTo(3));
            Assert.That(sdu.InnerFrameAndTimeSeries.First().ValidTime, Is.EqualTo(1630560671429L));
            Assert.That(JsonConvert.DeserializeObject<IrregularSeriesSdu>(JsonConvert.SerializeObject(sdu)), Is.EqualTo(sdu));
        }

        [Test]
        public void StreamInstancesParseTheInterpolateTransitionModel()
        {
            var header = JsonConvert.DeserializeObject<StreamHeaderSdu>(Load("GeoPose.Composite.Sequence.StreamHeader.Instance.json"));
            var element = JsonConvert.DeserializeObject<StreamElementSdu>(Load("GeoPose.Composite.Sequence.StreamElement.Instance.json"));
            var stream = JsonConvert.DeserializeObject<StreamRecordSdu>(Load("GeoPose.Composite.Sequence.Stream.Instance.json"));

            Assert.That(header.TransitionModel, Is.InstanceOf<OgcInterpolateTransitionModel>());
            Assert.That(header.TransitionModel.Id, Is.EqualTo("interpolate"));
            Assert.That(((TranslateRotateSpecification)element.StreamElement.Frame).Rotation.W, Is.EqualTo(-0.90510));
            Assert.That(stream.StreamElements.Count(), Is.EqualTo(3));
            Assert.That(JsonConvert.DeserializeObject<StreamHeaderSdu>(JsonConvert.SerializeObject(header)), Is.EqualTo(header));
            Assert.That(JsonConvert.DeserializeObject<StreamElementSdu>(JsonConvert.SerializeObject(element)), Is.EqualTo(element));
            Assert.That(JsonConvert.DeserializeObject<StreamRecordSdu>(JsonConvert.SerializeObject(stream)), Is.EqualTo(stream));
        }

        [Test]
        public void EveryInstanceFileIsCoveredByAFixture()
        {
            var files = Directory.GetFiles(FixtureDirectory, "GeoPose.*.json").Select(Path.GetFileName).ToList();

            Assert.That(files.Count, Is.EqualTo(20));
        }

        private static string Load(string file) => File.ReadAllText(Path.Combine(FixtureDirectory, file));
    }
}
