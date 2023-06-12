using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.DataTypes;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.Authority.UnitTests
{
    [TestFixture]
    public class LtpNedSpecificationTests : UnitTestBase
    {
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"LTP-NED\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"")]
        public void CanCorrectlyDeserializeLtpNedSpecification(long validTime, string authority, string id, string parameters)
        {
            var json =
                "{" +
                    $"\"authority\": {authority}," +
                    $"\"id\": {id}," +
                    $"\"parameters\": {parameters}" +
                "}";

            var ltpEnuSpec = JsonConvert.DeserializeObject<LtpNedSpecification>(json);

            Assert.That(ltpEnuSpec.Position.Latitude, Is.EqualTo(48));
            Assert.That(ltpEnuSpec.Position.Longitude, Is.EqualTo(-122));
            Assert.That(ltpEnuSpec.Position.HeightInMeters, Is.EqualTo(5));
        }

        [TestCase(16534234327, "/Ethar.GeoPose/1.0", "LTP-NED", "\"latitude=48&longitude=-122&heightInMeters=5\"")]
        public void CanCorrectlySerializeLtpNedSpecification(long validTime, string authority, string id, string parameters)
        {
            var ltpEnuSpec = new LtpNedSpecification(new TangentPointPosition()
            { Latitude = 48, Longitude = -122, HeightInMeters = 5 });

            var expected =
                "{" +
                    $"\"authority\":\"{authority}\"," +
                    $"\"id\":\"{id}\"," +
                    $"\"parameters\":{parameters}" +
                "}";

            var json = JsonConvert.SerializeObject(ltpEnuSpec);

            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase(16534234327, "/Ethar.GeoPose/1.0", "LTP-NED", "\"latitude=48&longitude=-122&heightInMeters=5\"")]
        public void CanCorrectlyMakeARoundTripSerialization(long validTime, string authority, string id, string parameters)
        {
            var ltpEnuSpec = new LtpNedSpecification(new TangentPointPosition()
            { Latitude = 48, Longitude = -122, HeightInMeters = 5 });

            var json = JsonConvert.SerializeObject(ltpEnuSpec);
            var converted = JsonConvert.DeserializeObject<LtpNedSpecification>(json);
            Assert.That(ltpEnuSpec, Is.EqualTo(converted));
        }
    }
}
