using Ethar.GeoPose.Authority.FrameSpecifications;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.Authority.UnitTests
{
    [TestFixture]
    internal class TranslateRotateSpecificationTests : UnitTestBase
    {
        [TestCase("\"/Ethar.GeoPose/1.0\"", "\"Translate-Rotate\"", "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlyDeserializeTranslateRotateSpecification(string authority, string id, string parameters)
        {
            var json =
                "{" +
                    $"\"authority\": {authority}," +
                    $"\"id\": {id}," +
                    $"\"parameters\": {parameters}" +
                "}";

            var translateRotateSpec = JsonConvert.DeserializeObject<TranslateRotateSpecification>(json);
            Assert.That(translateRotateSpec.Translation.X, Is.EqualTo(0.1));
            Assert.That(translateRotateSpec.Translation.Y, Is.EqualTo(0.2));
            Assert.That(translateRotateSpec.Translation.Z, Is.EqualTo(0.3));
            Assert.That(translateRotateSpec.Rotation.X, Is.EqualTo(0.692));
            Assert.That(translateRotateSpec.Rotation.Y, Is.EqualTo(0.691));
            Assert.That(translateRotateSpec.Rotation.Z, Is.EqualTo(0.141));
            Assert.That(translateRotateSpec.Rotation.W, Is.EqualTo(0.14));
        }

        [TestCase("/Ethar.GeoPose/1.0", "Translate-Rotate", "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlySerializeTranslateRotateSpecification(string authority, string id, string parameters)
        {
            var frameSpec = new TranslateRotateSpecification(
                new DataTypes.UnitVector3(0.1, 0.2, 0.3),
                new DataTypes.UnitQuaternion() { X = 0.692, Y = 0.691, Z = 0.141, W = 0.14 });

            var expected =
                "{" +
                    $"\"authority\":\"{authority}\"," +
                    $"\"id\":\"{id}\"," +
                    $"\"parameters\":{parameters}" +
                "}";

            var json = JsonConvert.SerializeObject(frameSpec);
            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase("/Ethar.GeoPose/1.0", "Translate-Rotate")]
        public void CanCorrectlyMakeARoundTripSerialization(string authority, string id)
        {
            var frameSpec = new TranslateRotateSpecification(
                new DataTypes.UnitVector3(0.1, 0.2, 0.3),
                new DataTypes.UnitQuaternion() { X = 0.692, Y = 0.691, Z = 0.141, W = 0.14 });

            var json = JsonConvert.SerializeObject(frameSpec);

            var converted = JsonConvert.DeserializeObject<TranslateRotateSpecification>(json);
            Assert.That(frameSpec, Is.EqualTo(converted));
        }
    }
}
