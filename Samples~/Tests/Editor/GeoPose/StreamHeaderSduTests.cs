using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.Authority.TransitionModels;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    [TestFixture]
    public class StreamHeaderSduTests : UnitTestBase
    {
        [TestCase("\"/Ethar.GeoPose/1.0\"", "\"LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"")]
        public void CanCorrectlyDeserializeStreamHeaderSdu(string authority, string outerFrameId, string outerFrameParams)
        {
            var json =
                "{" +
                    $"\"transitionModel\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": \"none\"," +
                        $"\"parameters\": \"\"" +
                    "}," +
                    "\"outerFrame\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": {outerFrameId}," +
                        $"\"parameters\": {outerFrameParams}" +
                    "}," +
                "}";

            var sdu = JsonConvert.DeserializeObject<StreamHeaderSdu>(json);

            Assert.That(sdu.TransitionModel is NoneTransitionModel);

            Assert.That(sdu.OuterFrame is LtpEnuSpecification);
            var outerFrame = (LtpEnuSpecification)sdu.OuterFrame;

            Assert.That(outerFrame.Position.Latitude, Is.EqualTo(48));
            Assert.That(outerFrame.Position.Longitude, Is.EqualTo(-122));
            Assert.That(outerFrame.Position.HeightInMeters, Is.EqualTo(5));
        }

        [TestCase("/Ethar.GeoPose/1.0", "LTP-ENU", "\"latitude=48&longitude=-122&heightInMeters=5\"")]
        public void CanCorrectlySerializeStreamHeaderSdu(string authority, string outerFrameId, string outerFrameParams)
        {
            var outerFrame = new LtpEnuSpecification(new TangentPointPosition() { Latitude = 48, Longitude = -122, HeightInMeters = 5 });
            var sdu = new StreamHeaderSdu(new NoneTransitionModel(), outerFrame);

            var expected =
                "{" +
                    $"\"transitionModel\":" +
                    "{" +
                        $"\"authority\":\"{authority}\"," +
                        $"\"id\":\"none\"," +
                        $"\"parameters\":\"\"" +
                    "}," +
                    "\"outerFrame\":" +
                    "{" +
                        $"\"authority\":\"{authority}\"," +
                        $"\"id\":\"{outerFrameId}\"," +
                        $"\"parameters\":{outerFrameParams}" +
                    "}" +
                "}";

            var json = JsonConvert.SerializeObject(sdu);
            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase("/Ethar.GeoPose/1.0", "LTP-ENU", "{\"latitude\":48,\"longitude\":-122,\"heightInMeters\":5}")]
        public void CanCorrectlyMakeARoundTripConversion(string authority, string outerFrameId, string outerFrameParams)
        {
            var position = JsonConvert.DeserializeObject<TangentPointPosition>(outerFrameParams);
            var outerFrame = new LtpEnuSpecification(position);
            var sdu = new StreamHeaderSdu(new NoneTransitionModel(), outerFrame);
            var json = JsonConvert.SerializeObject(sdu);

            var converted = JsonConvert.DeserializeObject<StreamHeaderSdu>(json);
            Assert.That(converted, Is.EqualTo(sdu));
        }

        [TestCase("/Ethar.GeoPose/1.0", "LTP-ENU", "{\"latitude\":48,\"longitude\":-122,\"heightInMeters\":5}")]
        public void CanConvertStreamHeaderSduToString(string authority, string outerFrameId, string outerFrameParams)
        {
            var position = JsonConvert.DeserializeObject<TangentPointPosition>(outerFrameParams);
            var outerFrame = new LtpEnuSpecification(position);
            var sdu = new StreamHeaderSdu(new NoneTransitionModel(), outerFrame);

            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("TransitionModel:[Authority:/Ethar.GeoPose/1.0, Id:none], OuterFrame:[Authority:/Ethar.GeoPose/1.0, Id:LTP-ENU]"));
        }

        [Test]
        public void CanConvertStreamHeaderSduNew()
        {
            var sdu = new StreamHeaderSdu();
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("TransitionModel:[], OuterFrame:[]"));
        }
    }
}
