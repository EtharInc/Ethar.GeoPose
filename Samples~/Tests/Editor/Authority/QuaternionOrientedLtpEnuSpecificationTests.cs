using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.DataTypes;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.Authority.UnitTests
{
    [TestFixture]
    internal class QuaternionOrientedLtpEnuSpecificationTests : UnitTestBase
    {
        [TestCase("\"/Ethar.GeoPose/1.0\"", "\"Quaternion-LTP-ENU\"", "\"longitude=-122&latitude=48&heightInMeters=5&orientation.x=0.692&orientation.y=0.691&orientation.z=0.141&orientation.w=0.14\"")]
        public void CanCorrectlyDeserializeYprLtpEnuSpecification(string authority, string id, string parameters)
        {
            var json =
                "{" +
                    $"\"authority\": {authority}," +
                    $"\"id\": {id}," +
                    $"\"parameters\": {parameters}" +
                "}";

            var ltpEnuSpec = JsonConvert.DeserializeObject<QuaternionOrientedLtpEnuSpecification>(json);

            Assert.That(ltpEnuSpec.Position.Latitude, Is.EqualTo(48));
            Assert.That(ltpEnuSpec.Position.Longitude, Is.EqualTo(-122));
            Assert.That(ltpEnuSpec.Position.HeightInMeters, Is.EqualTo(5));
            Assert.That(ltpEnuSpec.Orientation.X, Is.EqualTo(0.692f));
            Assert.That(ltpEnuSpec.Orientation.Y, Is.EqualTo(0.691f));
            Assert.That(ltpEnuSpec.Orientation.Z, Is.EqualTo(0.141f));
            Assert.That(ltpEnuSpec.Orientation.W, Is.EqualTo(0.14f));
        }

        [TestCase("/Ethar.GeoPose/1.0", "Quaternion-LTP-ENU", "\"latitude=48&longitude=-122&heightInMeters=5&orientation.x=0.692&orientation.y=0.691&orientation.z=0.141&orientation.w=0.14\"")]
        public void CanCorrectlySerializeYprLtpEnuSpecification(string authority, string id, string parameters)
        {
            var ltpEnuSpec = new QuaternionOrientedLtpEnuSpecification(new TangentPointPosition()
            { Latitude = 48, Longitude = -122, HeightInMeters = 5 }, new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f });

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
            var ltpEnuSpec = new QuaternionOrientedLtpEnuSpecification(new TangentPointPosition()
            { Latitude = 48, Longitude = -122, HeightInMeters = 5 }, new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f });

            var json = JsonConvert.SerializeObject(ltpEnuSpec);
            var converted = JsonConvert.DeserializeObject<QuaternionOrientedLtpEnuSpecification>(json);
            Assert.That(ltpEnuSpec, Is.EqualTo(converted));
        }
    }
}
