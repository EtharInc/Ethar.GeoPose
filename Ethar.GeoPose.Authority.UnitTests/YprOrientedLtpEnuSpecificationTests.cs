using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.DataTypes;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.Authority.UnitTests
{
    [TestFixture]
    public class YprOrientedLtpEnuSpecificationTests : UnitTestBase
    {
        [TestCase("\"/Ethar.GeoPose/1.0\"", "\"YPR-LTP-ENU\"", "\"longitude=-122&latitude=48&heightInMeters=5&orientation.yaw=0.1&orientation.pitch=0.2&orientation.roll=0.3\"")]
        public void CanCorrectlyDeserializeYprLtpEnuSpecification(string authority, string id, string parameters)
        {
            var json =
                "{" +
                    $"\"authority\": {authority}," +
                    $"\"id\": {id}," +
                    $"\"parameters\": {parameters}" +
                "}";

            var ltpEnuSpec = JsonConvert.DeserializeObject<YawPitchRollOrientedLtpEnuSpecification>(json);

            Assert.That(ltpEnuSpec.Position.Latitude, Is.EqualTo(48));
            Assert.That(ltpEnuSpec.Position.Longitude, Is.EqualTo(-122));
            Assert.That(ltpEnuSpec.Position.HeightInMeters, Is.EqualTo(5));
            Assert.That(ltpEnuSpec.Orientation.Yaw, Is.EqualTo(0.1f));
            Assert.That(ltpEnuSpec.Orientation.Pitch, Is.EqualTo(0.2f));
            Assert.That(ltpEnuSpec.Orientation.Roll, Is.EqualTo(0.3f));
        }

        [TestCase("/Ethar.GeoPose/1.0", "YPR-LTP-ENU", "\"latitude=48&longitude=-122&heightInMeters=5&orientation.yaw=0.1&orientation.pitch=0.2&orientation.roll=0.3\"")]
        public void CanCorrectlySerializeYprLtpEnuSpecification(string authority, string id, string parameters)
        {
            var ltpEnuSpec = new YawPitchRollOrientedLtpEnuSpecification(new TangentPointPosition()
            { Latitude = 48, Longitude = -122, HeightInMeters = 5 }, new YawPitchRollAngles() { Yaw = 0.1f, Pitch = 0.2f, Roll = 0.3f });

            var expected =
                "{" +
                    $"\"authority\":\"{authority}\"," +
                    $"\"id\":\"{id}\"," +
                    $"\"parameters\":{parameters}" +
                "}";

            var json = JsonConvert.SerializeObject(ltpEnuSpec);

            Assert.That(json, Is.EqualTo(expected));
        }

        [Test]
        public void CanCorrectlyMakeARoundTripSerialization()
        {
            var ltpEnuSpec = new YawPitchRollOrientedLtpEnuSpecification(new TangentPointPosition()
            { Latitude = 48, Longitude = -122, HeightInMeters = 5 }, new YawPitchRollAngles() { Yaw = 0.1f, Pitch = 0.2f, Roll = 0.3f });

            var json = JsonConvert.SerializeObject(ltpEnuSpec);
            var converted = JsonConvert.DeserializeObject<YawPitchRollOrientedLtpEnuSpecification>(json);
            Assert.That(ltpEnuSpec, Is.EqualTo(converted));
        }
    }
}
