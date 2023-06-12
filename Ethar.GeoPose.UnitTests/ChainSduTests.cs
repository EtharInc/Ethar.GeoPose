using System.Collections.Generic;
using System.Linq;
using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.FrameSpecifications;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    [TestFixture]
    public class ChainSduTests : UnitTestBase
    {
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"", "\"Translate-Rotate\"",
            "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlyDeserializeChainSdu(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var json =
                "{" +
                    "\"frameChain\": [" +
                        "{" +
                            $"\"authority\": {authority}," +
                            $"\"id\": {innerFrameId}," +
                            $"\"parameters\": {innerFrameParameters1}" +
                        "}," +
                        "{" +
                            $"\"authority\": {authority}," +
                            $"\"id\": {innerFrameId}," +
                            $"\"parameters\": {innerFrameParameters2}" +
                        "}," +
                        "{" +
                            $"\"authority\": {authority}," +
                            $"\"id\": {innerFrameId}," +
                            $"\"parameters\": {innerFrameParameters3}" +
                        "}" +
                    "]," +
                    "\"outerFrame\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": {outerFrameId}," +
                        $"\"parameters\": {outerFrameParams}" +
                    "}," +
                    $"\"validTime\": {validTime}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<ChainSdu>(json);
            Assert.That(sdu.ValidTime, Is.EqualTo(validTime));

            Assert.That(sdu.OuterFrame is LtpEnuSpecification);
            var outerFrame = (LtpEnuSpecification)sdu.OuterFrame;

            Assert.That(outerFrame.Position.Latitude, Is.EqualTo(48));
            Assert.That(outerFrame.Position.Longitude, Is.EqualTo(-122));
            Assert.That(outerFrame.Position.HeightInMeters, Is.EqualTo(5));

            Assert.That(sdu.FrameChain.All(x => x is TranslateRotateSpecification));

            var translateRotateList = sdu.FrameChain.Select(x => x as TranslateRotateSpecification).ToList();
            Assert.That(translateRotateList.Count, Is.EqualTo(3));

            Assert.That(translateRotateList.ElementAt(0).Translation.X, Is.EqualTo(0.1f));
            Assert.That(translateRotateList.ElementAt(0).Translation.Y, Is.EqualTo(0.2f));
            Assert.That(translateRotateList.ElementAt(0).Translation.Z, Is.EqualTo(0.3f));
            Assert.That(translateRotateList.ElementAt(0).Rotation.X, Is.EqualTo(0.692f));
            Assert.That(translateRotateList.ElementAt(0).Rotation.Y, Is.EqualTo(0.691f));
            Assert.That(translateRotateList.ElementAt(0).Rotation.Z, Is.EqualTo(0.141f));
            Assert.That(translateRotateList.ElementAt(0).Rotation.W, Is.EqualTo(0.14f));

            Assert.That(translateRotateList.ElementAt(1).Translation.X, Is.EqualTo(0.4f));
            Assert.That(translateRotateList.ElementAt(1).Translation.Y, Is.EqualTo(0.5f));
            Assert.That(translateRotateList.ElementAt(1).Translation.Z, Is.EqualTo(0.6f));
            Assert.That(translateRotateList.ElementAt(1).Rotation.X, Is.EqualTo(0.692f));
            Assert.That(translateRotateList.ElementAt(1).Rotation.Y, Is.EqualTo(0.691f));
            Assert.That(translateRotateList.ElementAt(1).Rotation.Z, Is.EqualTo(0.141f));
            Assert.That(translateRotateList.ElementAt(1).Rotation.W, Is.EqualTo(0.14f));

            Assert.That(translateRotateList.ElementAt(2).Translation.X, Is.EqualTo(0.7f));
            Assert.That(translateRotateList.ElementAt(2).Translation.Y, Is.EqualTo(0.8f));
            Assert.That(translateRotateList.ElementAt(2).Translation.Z, Is.EqualTo(0.9f));
            Assert.That(translateRotateList.ElementAt(2).Rotation.X, Is.EqualTo(0.692f));
            Assert.That(translateRotateList.ElementAt(2).Rotation.Y, Is.EqualTo(0.691f));
            Assert.That(translateRotateList.ElementAt(2).Rotation.Z, Is.EqualTo(0.141f));
            Assert.That(translateRotateList.ElementAt(2).Rotation.W, Is.EqualTo(0.14f));
        }

        [TestCase(16534234327, "/Ethar.GeoPose/1.0", "LTP-ENU", "\"latitude=48&longitude=-122&heightInMeters=5\"", "Translate-Rotate",
    "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
    "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
    "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlySerializeChainSdu(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var outerFrame = new LtpEnuSpecification(new DataTypes.TangentPointPosition() { Latitude = 48, Longitude = -122, HeightInMeters = 5 });

            var frameList = new List<BaseFrameSpecification>()
            {
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f})
            };

            var sdu = new ChainSdu(validTime, outerFrame, frameList);

            var expected =
                "{" +
                    $"\"validTime\":{validTime}," +
                    "\"outerFrame\":" +
                    "{" +
                        $"\"authority\":\"{authority}\"," +
                        $"\"id\":\"{outerFrameId}\"," +
                        $"\"parameters\":{outerFrameParams}" +
                    "}," +
                    "\"frameChain\":[" +
                        "{" +
                            $"\"authority\":\"{authority}\"," +
                            $"\"id\":\"{innerFrameId}\"," +
                            $"\"parameters\":{innerFrameParameters1}" +
                        "}," +
                        "{" +
                            $"\"authority\":\"{authority}\"," +
                            $"\"id\":\"{innerFrameId}\"," +
                            $"\"parameters\":{innerFrameParameters2}" +
                        "}," +
                        "{" +
                            $"\"authority\":\"{authority}\"," +
                            $"\"id\":\"{innerFrameId}\"," +
                            $"\"parameters\":{innerFrameParameters3}" +
                        "}" +
                    "]" +
                "}";

            var json = JsonConvert.SerializeObject(sdu);
            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase(16534234327)]
        public void CanCorrectlyMakeRoundTripSerialization(long validTime)
        {
            var outerFrame = new LtpEnuSpecification(new DataTypes.TangentPointPosition() { Latitude = 48, Longitude = -122, HeightInMeters = 5 });

            var frameList = new List<BaseFrameSpecification>()
            {
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f})
            };

            var sdu = new ChainSdu(validTime, outerFrame, frameList);
            var json = JsonConvert.SerializeObject(sdu);
            var converted = JsonConvert.DeserializeObject<ChainSdu>(json);
            Assert.That(sdu, Is.EqualTo(converted));
        }

        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"", "\"Translate-Rotate\"",
            "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanConvertChainSduToString(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var outerFrame = new LtpEnuSpecification(new DataTypes.TangentPointPosition() { Latitude = 48, Longitude = -122, HeightInMeters = 5 });

            var frameList = new List<BaseFrameSpecification>()
            {
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f})
            };

            var sdu = new ChainSdu(validTime, outerFrame, frameList);
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("ValidTime:16534234327, OuterFrame:[Authority:/Ethar.GeoPose/1.0, Id:LTP-ENU], FrameChainCount:3"));
        }

        [TestCase(16534234327)]
        public void CanConvertChainSduToStringWithNull(long validTime)
        {
            var sdu = new ChainSdu(validTime, null, null);
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("ValidTime:16534234327, OuterFrame:[], FrameChainCount:0"));
        }

        [Test]
        public void CanConvertChainSduToStringNew()
        {
            var sdu = new ChainSdu();
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("ValidTime:0, OuterFrame:[], FrameChainCount:0"));
        }
    }
}
