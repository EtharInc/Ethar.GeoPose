using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    internal class BasicQuaternionSduTests : UnitTestBase
    {
        [TestCase(45, 45, 5, 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanCorrectlyDeserializeBasicQuaternionSdu(float lat, float lon, float h, float x, float y, float z, float w)
        {
            var json =
                "{" +
                    "\"position\":" +
                    "{" +
                        $"\"lat\": {lat}," +
                        $"\"lon\": {lon}," +
                        $"\"h\": {h}" +
                    "}," +
                    "\"quaternion\":" +
                    "{" +
                        $"\"x\": {x}," +
                        $"\"y\": {y}," +
                        $"\"z\": {z}," +
                        $"\"w\": {w}" +
                    "}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<BasicQuaternionSdu>(json);
            Assert.That(sdu.Position.Longitude, Is.EqualTo(lon));
            Assert.That(sdu.Position.Latitude, Is.EqualTo(lat));
            Assert.That(sdu.Position.HeightInMeters, Is.EqualTo(h));
            Assert.That(sdu.Quaternion.X, Is.EqualTo(x));
            Assert.That(sdu.Quaternion.Y, Is.EqualTo(y));
            Assert.That(sdu.Quaternion.Z, Is.EqualTo(z));
            Assert.That(sdu.Quaternion.W, Is.EqualTo(w));
        }

        [TestCase(45, 45, 5, 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanCorrectlySerializeBasicQuaternionSdu(float lat, float lon, float h, float x, float y, float z, float w)
        {
            var sdu = new BasicQuaternionSdu(new DataTypes.TangentPointPosition() { HeightInMeters = h, Latitude = lat, Longitude = lon }, new DataTypes.UnitQuaternion() { X = x, Y = y, Z = z, W = w });

            var json = JsonConvert.SerializeObject(sdu);

            var expected =
                "{" +
                    "\"position\":" +
                    "{" +
                        $"\"lat\":{lat}.0," +
                        $"\"lon\":{lon}.0," +
                        $"\"h\":{h}.0" +
                    "}," +
                    "\"quaternion\":" +
                    "{" +
                        $"\"x\":{x}," +
                        $"\"y\":{y}," +
                        $"\"z\":{z}," +
                        $"\"w\":{w}" +
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

        [TestCase(45, 45, 5, 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanConvertBasicQuaternionSduToString(float lat, float lon, float h, float x, float y, float z, float w)
        {
            var sdu = new BasicQuaternionSdu(new DataTypes.TangentPointPosition() { HeightInMeters = h, Latitude = lat, Longitude = lon }, new DataTypes.UnitQuaternion() { X = x, Y = y, Z = z, W = w });
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("Position:[Latitude:45, Longitude:45, HeightInMeters:5], Quaternion:[X:0.207, Y:0.218, Z:0.655, W:-0.692]"));
        }

        [Test]
        public void CanConvertBasicQuaternionSduToStringNew()
        {
            var sdu = new BasicQuaternionSdu();
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("Position:[Latitude:0, Longitude:0, HeightInMeters:0], Quaternion:[X:0, Y:0, Z:0, W:0]"));
        }
    }
}
