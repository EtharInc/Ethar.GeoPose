using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    internal class AdvancedSduTests : UnitTestBase
    {
        [Test]
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"", 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanCorrectlyDeserializeAdvancedSduWithLtpEnuSpecification(long validTime, string authority, string id, string parameters, float x, float y, float z, float w)
        {
            var json =
                "{" +
                    "\"frameSpecification\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": {id}," +
                        $"\"parameters\": {parameters}" +
                    "}," +
                    "\"quaternion\":" +
                    "{" +
                        $"\"x\": {x}," +
                        $"\"y\": {y}," +
                        $"\"z\": {z}," +
                        $"\"w\": {w}" +
                    "}," +
                    $"\"validTime\": {validTime}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<AdvancedSdu>(json);
            Assert.That(sdu.ValidTime, Is.EqualTo(validTime));
            Assert.That(sdu.Quaternion.X, Is.EqualTo(x));
            Assert.That(sdu.Quaternion.Y, Is.EqualTo(y));
            Assert.That(sdu.Quaternion.Z, Is.EqualTo(z));
            Assert.That(sdu.Quaternion.W, Is.EqualTo(w));

            Assert.That(sdu.FrameSpecification is LtpEnuSpecification);

            var ltpEnuSpec = sdu.FrameSpecification as LtpEnuSpecification;
            Assert.That(ltpEnuSpec.Position.Latitude, Is.EqualTo(48));
            Assert.That(ltpEnuSpec.Position.Longitude, Is.EqualTo(-122));
            Assert.That(ltpEnuSpec.Position.HeightInMeters, Is.EqualTo(5));
        }

        [Test]
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"LTP-NED\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"", 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanCorrectlyDeserializeAdvancedSduWithLtpNedSpecification(long validTime, string authority, string id, string parameters, float x, float y, float z, float w)
        {
            var json =
                "{" +
                    "\"frameSpecification\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": {id}," +
                        $"\"parameters\": {parameters}" +
                    "}," +
                    "\"quaternion\":" +
                    "{" +
                        $"\"x\": {x}," +
                        $"\"y\": {y}," +
                        $"\"z\": {z}," +
                        $"\"w\": {w}" +
                    "}," +
                    $"\"validTime\": {validTime}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<AdvancedSdu>(json);
            Assert.That(sdu.ValidTime, Is.EqualTo(validTime));
            Assert.That(sdu.Quaternion.X, Is.EqualTo(x));
            Assert.That(sdu.Quaternion.Y, Is.EqualTo(y));
            Assert.That(sdu.Quaternion.Z, Is.EqualTo(z));
            Assert.That(sdu.Quaternion.W, Is.EqualTo(w));

            Assert.That(sdu.FrameSpecification is LtpNedSpecification);

            var ltpEnuSpec = sdu.FrameSpecification as LtpNedSpecification;
            Assert.That(ltpEnuSpec.Position.Latitude, Is.EqualTo(48));
            Assert.That(ltpEnuSpec.Position.Longitude, Is.EqualTo(-122));
            Assert.That(ltpEnuSpec.Position.HeightInMeters, Is.EqualTo(5));
        }

        [Test]
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"YPR-LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000&orientation.yaw=10&orientation.pitch=11&orientation.roll=12\"", 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanCorrectlyDeserializeAdvancedSduWithYprOrientedLtpEnuSpecification(long validTime, string authority, string id, string parameters, float x, float y, float z, float w)
        {
            var json =
                "{" +
                    "\"frameSpecification\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": {id}," +
                        $"\"parameters\": {parameters}" +
                    "}," +
                    "\"quaternion\":" +
                    "{" +
                        $"\"x\": {x}," +
                        $"\"y\": {y}," +
                        $"\"z\": {z}," +
                        $"\"w\": {w}" +
                    "}," +
                    $"\"validTime\": {validTime}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<AdvancedSdu>(json);
            Assert.That(sdu.ValidTime, Is.EqualTo(validTime));
            Assert.That(sdu.Quaternion.X, Is.EqualTo(x));
            Assert.That(sdu.Quaternion.Y, Is.EqualTo(y));
            Assert.That(sdu.Quaternion.Z, Is.EqualTo(z));
            Assert.That(sdu.Quaternion.W, Is.EqualTo(w));

            Assert.That(sdu.FrameSpecification is YawPitchRollOrientedLtpEnuSpecification);

            var ltpEnuSpec = sdu.FrameSpecification as YawPitchRollOrientedLtpEnuSpecification;
            Assert.That(ltpEnuSpec.Position.Latitude, Is.EqualTo(48));
            Assert.That(ltpEnuSpec.Position.Longitude, Is.EqualTo(-122));
            Assert.That(ltpEnuSpec.Position.HeightInMeters, Is.EqualTo(5));
            Assert.That(ltpEnuSpec.Orientation.Yaw, Is.EqualTo(10));
            Assert.That(ltpEnuSpec.Orientation.Pitch, Is.EqualTo(11));
            Assert.That(ltpEnuSpec.Orientation.Roll, Is.EqualTo(12));
        }

        [Test]
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"Quaternion-LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000&orientation.x=0.5&orientation.y=0.25&orientation.z=0.25&orientation.w=-0.5\"", 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanCorrectlyDeserializeAdvancedSduWithQuaternionOrientedLtpEnuSpecification(long validTime, string authority, string id, string parameters, float x, float y, float z, float w)
        {
            var json =
                "{" +
                    "\"frameSpecification\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": {id}," +
                        $"\"parameters\": {parameters}" +
                    "}," +
                    "\"quaternion\":" +
                    "{" +
                        $"\"x\": {x}," +
                        $"\"y\": {y}," +
                        $"\"z\": {z}," +
                        $"\"w\": {w}" +
                    "}," +
                    $"\"validTime\": {validTime}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<AdvancedSdu>(json);
            Assert.That(sdu.ValidTime, Is.EqualTo(validTime));
            Assert.That(sdu.Quaternion.X, Is.EqualTo(x));
            Assert.That(sdu.Quaternion.Y, Is.EqualTo(y));
            Assert.That(sdu.Quaternion.Z, Is.EqualTo(z));
            Assert.That(sdu.Quaternion.W, Is.EqualTo(w));

            Assert.That(sdu.FrameSpecification is QuaternionOrientedLtpEnuSpecification);

            var ltpEnuSpec = sdu.FrameSpecification as QuaternionOrientedLtpEnuSpecification;
            Assert.That(ltpEnuSpec.Position.Latitude, Is.EqualTo(48));
            Assert.That(ltpEnuSpec.Position.Longitude, Is.EqualTo(-122));
            Assert.That(ltpEnuSpec.Position.HeightInMeters, Is.EqualTo(5));
            Assert.That(ltpEnuSpec.Orientation.X, Is.EqualTo(0.5));
            Assert.That(ltpEnuSpec.Orientation.Y, Is.EqualTo(0.25));
            Assert.That(ltpEnuSpec.Orientation.Z, Is.EqualTo(0.25));
            Assert.That(ltpEnuSpec.Orientation.W, Is.EqualTo(-0.5));
        }

        [Test]
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"Translate-Rotate\"", "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"", 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanCorrectlyDeserializeAdvancedSduWithTranslateRotateSpecification(long validTime, string authority, string id, string parameters, float x, float y, float z, float w)
        {
            var json =
                "{" +
                    "\"frameSpecification\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": {id}," +
                        $"\"parameters\": {parameters}" +
                    "}," +
                    "\"quaternion\":" +
                    "{" +
                        $"\"x\": {x}," +
                        $"\"y\": {y}," +
                        $"\"z\": {z}," +
                        $"\"w\": {w}" +
                    "}," +
                    $"\"validTime\": {validTime}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<AdvancedSdu>(json);
            Assert.That(sdu.ValidTime, Is.EqualTo(validTime));
            Assert.That(sdu.Quaternion.X, Is.EqualTo(x));
            Assert.That(sdu.Quaternion.Y, Is.EqualTo(y));
            Assert.That(sdu.Quaternion.Z, Is.EqualTo(z));
            Assert.That(sdu.Quaternion.W, Is.EqualTo(w));

            Assert.That(sdu.FrameSpecification is TranslateRotateSpecification);

            var translateRotateSpec = sdu.FrameSpecification as TranslateRotateSpecification;
            Assert.That(translateRotateSpec.Translation.X, Is.EqualTo(0.1f));
            Assert.That(translateRotateSpec.Translation.Y, Is.EqualTo(0.2f));
            Assert.That(translateRotateSpec.Translation.Z, Is.EqualTo(0.3f));
            Assert.That(translateRotateSpec.Rotation.X, Is.EqualTo(0.692f));
            Assert.That(translateRotateSpec.Rotation.Y, Is.EqualTo(0.691f));
            Assert.That(translateRotateSpec.Rotation.Z, Is.EqualTo(0.141f));
            Assert.That(translateRotateSpec.Rotation.W, Is.EqualTo(0.14f));
        }

        [Test]
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"Translate-Rotate\"", "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"", 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanConvertAdvancedSDUToString(long validTime, string authority, string id, string parameters, float x, float y, float z, float w)
        {
            var json =
                "{" +
                    "\"frameSpecification\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": {id}," +
                        $"\"parameters\": {parameters}" +
                    "}," +
                    "\"quaternion\":" +
                    "{" +
                        $"\"x\": {x}," +
                        $"\"y\": {y}," +
                        $"\"z\": {z}," +
                        $"\"w\": {w}" +
                    "}," +
                    $"\"validTime\": {validTime}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<AdvancedSdu>(json);
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("ValidTime:16534234327, Quaternion:[X:0.207, Y:0.218, Z:0.655, W:-0.692], FrameSpecification:[Authority:/Ethar.GeoPose/1.0, Id:Translate-Rotate]"));
        }

        [Test]
        [TestCase(16534234327, 0.207f, 0.218f, 0.655f, -0.692f)]
        public void CanConvertAdvancedSDUToStringWithNoFrameSpecification(long validTime, float x, float y, float z, float w)
        {
            var sdu = new AdvancedSdu(validTime,new DataTypes.UnitQuaternion(x, y, z, w),null);
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("ValidTime:16534234327, Quaternion:[X:0.207, Y:0.218, Z:0.655, W:-0.692], FrameSpecification:[]"));
        }

        [Test]
        public void CanConvertAdvancedSDUToStringNew()
        {
            var sdu = new AdvancedSdu();
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("ValidTime:0, Quaternion:[X:0, Y:0, Z:0, W:0], FrameSpecification:[]"));
        }
    }
}
