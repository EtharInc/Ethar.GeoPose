using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    [TestFixture]
    public class StreamElementSduTests : UnitTestBase
    {
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"Translate-Rotate\"",
    "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlyDeserializeStreamElementSdu(long validTime, string authority, string innerFrameId, string innerFrameParameters)
        {
            var json =
                "{" +
                    "\"streamElement\":" +
                    "{" +
                        "\"frame\":" +
                        "{" +
                            $"\"authority\": {authority}," +
                            $"\"id\": {innerFrameId}," +
                            $"\"parameters\": {innerFrameParameters}" +
                        "}," +
                        $"\"validTime\": {validTime}," +
                    "}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<StreamElementSdu>(json);

            Assert.That(sdu.StreamElement.ValidTime, Is.EqualTo(validTime));

            Assert.That(sdu.StreamElement.Frame is TranslateRotateSpecification);

            var translateRotate = sdu.StreamElement.Frame as TranslateRotateSpecification;

            Assert.That(translateRotate.Translation.X, Is.EqualTo(0.1f));
            Assert.That(translateRotate.Translation.Y, Is.EqualTo(0.2f));
            Assert.That(translateRotate.Translation.Z, Is.EqualTo(0.3f));
            Assert.That(translateRotate.Rotation.X, Is.EqualTo(0.692f));
            Assert.That(translateRotate.Rotation.Y, Is.EqualTo(0.691f));
            Assert.That(translateRotate.Rotation.Z, Is.EqualTo(0.141f));
            Assert.That(translateRotate.Rotation.W, Is.EqualTo(0.14f));
        }

        [TestCase(16534234327, "/Ethar.GeoPose/1.0", "Translate-Rotate",
            "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlySerializeStreamElementSdu(long validTime, string authority, string innerFrameId, string innerFrameParameters)
        {
            var frameAndTime = new FrameAndTimeElement()
            {
                Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                    new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                ValidTime = validTime
            };

            var sdu = new StreamElementSdu(frameAndTime);

            var expected =
                "{" +
                    "\"streamElement\":" +
                    "{" +
                        "\"frame\":" +
                        "{" +
                            $"\"authority\":\"{authority}\"," +
                            $"\"id\":\"{innerFrameId}\"," +
                            $"\"parameters\":{innerFrameParameters}" +
                        "}," +
                        $"\"validTime\":{validTime}" +
                    "}" +
                "}";

            var json = JsonConvert.SerializeObject(sdu);

            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase(16534234327)]
        public void CanCorrectlyMakeARoundTripConversion(long validTime)
        {
            var frameAndTime = new FrameAndTimeElement()
            {
                Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                    new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                ValidTime = validTime
            };

            var sdu = new StreamElementSdu(frameAndTime);

            var json = JsonConvert.SerializeObject(sdu);

            var converted = JsonConvert.DeserializeObject<StreamElementSdu>(json);

            Assert.That(converted, Is.EqualTo(sdu));
        }

        [TestCase(16534234327, "/Ethar.GeoPose/1.0", "Translate-Rotate",
            "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanConvertStreamElementSduToString(long validTime, string authority, string innerFrameId, string innerFrameParameters)
        {
            var frameAndTime = new FrameAndTimeElement()
            {
                Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                    new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                ValidTime = validTime
            };

            var sdu = new StreamElementSdu(frameAndTime);
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("StreamElement:[Frame:[Authority:/Ethar.GeoPose/1.0, Id:Translate-Rotate], ValidTime:16534234327]"));
        }

        [Test]
        public void CanConvertStreamElementSduNew()
        {
            var sdu = new StreamElementSdu();
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("StreamElement:[Frame:[], ValidTime:0]"));
        }
    }
}
