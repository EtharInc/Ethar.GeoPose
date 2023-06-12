using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    [TestFixture]
    internal class BasicYawPitchRollSduTests : UnitTestBase
    {
#if !UNITY_EDITOR && APITEST
        [TestCase(45, 45, 10)]
        [TestCase(0, 90, 100)]
        [TestCase(45, 45, 10.12f)]
        [TestCase(-15, -45, -10)]
        public async Task CanCorrectlyParseBasicYawPitchRollSdu(float lat, float lon, float height)
        {
            var sdu = await this.Client.ReadAsJsonAsync<BasicYawPitchRollSdu>($"{ApiEndpoints.BasicYprGet}?longitude={lon}&latitude={lat}&heightInMeters={height}");
            Assert.That(sdu.Position.Longitude, Is.EqualTo(lon));
            Assert.That(sdu.Position.Latitude, Is.EqualTo(lat));
            Assert.That(sdu.Position.HeightInMeters, Is.EqualTo(height));
        }
#endif

        [TestCase(45f, 45f, 5f, 12, 16, 8)]
        public void CanCorrectlyDeserializeBasicYawPitchRollSdu(float lat, float lon, float h, float yaw, float pitch, float roll)
        {
            var json =
                "{" +
                    "\"position\":" +
                    "{" +
                        $"\"lat\": {lat}," +
                        $"\"lon\": {lon}," +
                        $"\"h\": {h}" +
                    "}," +
                    "\"angles\":" +
                    "{" +
                        $"\"yaw\": {yaw}," +
                        $"\"pitch\": {pitch}," +
                        $"\"roll\": {roll}" +
                    "}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<BasicYawPitchRollSdu>(json);

            Assert.That(sdu.Position.Longitude, Is.EqualTo(lon));
            Assert.That(sdu.Position.Latitude, Is.EqualTo(lat));
            Assert.That(sdu.Position.HeightInMeters, Is.EqualTo(h));
            Assert.That(sdu.Angles.Yaw, Is.EqualTo(yaw));
            Assert.That(sdu.Angles.Pitch, Is.EqualTo(pitch));
            Assert.That(sdu.Angles.Roll, Is.EqualTo(roll));
        }

        [TestCase(45.0f, 45.0f, 5, 12, 16, 8)]
        public void CanCorrectlySerializeBasicYawPitchRollSdu(float lat, float lon, float h, float yaw, float pitch, float roll)
        {
            var sdu = new BasicYawPitchRollSdu(new DataTypes.YawPitchRollAngles() { Yaw = yaw, Pitch = pitch, Roll = roll }, new DataTypes.TangentPointPosition() { HeightInMeters = h, Latitude = lat, Longitude = lon });

            var json = JsonConvert.SerializeObject(sdu);

            var expected =
                "{" +
                    "\"position\":" +
                    "{" +
                        $"\"lat\":{lat}.0," +
                        $"\"lon\":{lon}.0," +
                        $"\"h\":{h}.0" +
                    "}," +
                    "\"angles\":" +
                    "{" +
                        $"\"yaw\":{yaw}.0," +
                        $"\"pitch\":{pitch}.0," +
                        $"\"roll\":{roll}.0" +
                    "}" +
                "}";

            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase(45, 45, 5, 12, 16, 8)]
        public void CanCorrectlyMakeARoundTripConversion(float lat, float lon, float h, float yaw, float pitch, float roll)
        {
            var sdu = new BasicYawPitchRollSdu(new DataTypes.YawPitchRollAngles() { Yaw = yaw, Pitch = pitch, Roll = roll }, new DataTypes.TangentPointPosition() { HeightInMeters = h, Latitude = lat, Longitude = lon });

            var json = JsonConvert.SerializeObject(sdu);

            var converted = JsonConvert.DeserializeObject<BasicYawPitchRollSdu>(json);

            Assert.That(sdu, Is.EqualTo(converted));
        }

        [TestCase(45f, 45f, 5f, 12, 16, 8)]
        public void CanConvertBasicYawPitchRollSduToString(float lat, float lon, float h, float yaw, float pitch, float roll)
        {
            var sdu = new BasicYawPitchRollSdu(new DataTypes.YawPitchRollAngles() { Yaw = yaw, Pitch = pitch, Roll = roll }, new DataTypes.TangentPointPosition() { HeightInMeters = h, Latitude = lat, Longitude = lon });
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("Position:[Latitude:45, Longitude:45, HeightInMeters:5], Angles:[Yaw:12, Pitch:16, Roll:8]"));
        }

        [Test]
        public void CanConvertBasicYawPitchRollSduToStringNew()
        {
            var sdu = new BasicYawPitchRollSdu();
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("Position:[Latitude:0, Longitude:0, HeightInMeters:0], Angles:[Yaw:0, Pitch:0, Roll:0]"));
        }
    }
}
