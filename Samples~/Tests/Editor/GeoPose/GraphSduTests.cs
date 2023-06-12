using System.Collections.Generic;
using System.Linq;
using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.FrameSpecifications;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    [TestFixture]
    internal class GraphSduTests : UnitTestBase
    {
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"", "\"Translate-Rotate\"", "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"", "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"", "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlyDeserializeGraphSdu(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var json =
                "{" +
                    "\"frameList\": [" +
                        "{" +
                            $"\"authority\": {authority}," +
                            $"\"id\": {outerFrameId}," +
                            $"\"parameters\": {outerFrameParams}" +
                        "}," +
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
                    "\"transformList\": [" +
                        "{" +
                            $"\"link\": [" +
                                "0,1" +
                            "]" +
                        "}," +
                        "{" +
                            $"\"link\": [" +
                                "1,2" +
                            "]" +
                        "}," +
                        "{" +
                            $"\"link\": [" +
                                "2,3" +
                            "]" +
                        "}," +
                        "{" +
                            $"\"link\": [" +
                                "0,3" +
                            "]" +
                        "}" +
                    "]," +
                    $"\"validTime\": {validTime}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<GraphSdu>(json);
            Assert.That(sdu.ValidTime, Is.EqualTo(validTime));

            Assert.That(sdu.FrameList.OfType<LtpEnuSpecification>().Count(), Is.EqualTo(1));
            var ltpEnuFrame = sdu.FrameList.OfType<LtpEnuSpecification>().Cast<LtpEnuSpecification>().Single();

            Assert.That(ltpEnuFrame.Position.Latitude, Is.EqualTo(48));
            Assert.That(ltpEnuFrame.Position.Longitude, Is.EqualTo(-122));
            Assert.That(ltpEnuFrame.Position.HeightInMeters, Is.EqualTo(5));

            Assert.That(sdu.FrameList.OfType<TranslateRotateSpecification>().Count(), Is.EqualTo(3));

            var translateRotateList = sdu.FrameList.OfType<TranslateRotateSpecification>().ToList();

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

            Assert.That(sdu.TransformList.Count, Is.EqualTo(4));
            Assert.That(sdu.TransformList.Any(x => x.OuterFrameIndex == 0 && x.InnerFrameIndex == 1), Is.True);
            Assert.That(sdu.TransformList.Any(x => x.OuterFrameIndex == 1 && x.InnerFrameIndex == 2), Is.True);
            Assert.That(sdu.TransformList.Any(x => x.OuterFrameIndex == 2 && x.InnerFrameIndex == 3), Is.True);
            Assert.That(sdu.TransformList.Any(x => x.OuterFrameIndex == 0 && x.InnerFrameIndex == 3), Is.True);
        }

        [TestCase(16534234327, "/Ethar.GeoPose/1.0", "LTP-ENU", "\"latitude=48&longitude=-122&heightInMeters=5\"", "Translate-Rotate", "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"", "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"", "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlySerializeGraphSdu(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var frameList = new List<BaseFrameSpecification>()
            {
                new LtpEnuSpecification(new DataTypes.TangentPointPosition() {Latitude = 48, Longitude = -122, HeightInMeters = 5}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f})
            };

            var transformList = new List<FrameTransformIndexPair>()
            {
                new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                new FrameTransformIndexPair() { OuterFrameIndex = 1, InnerFrameIndex = 2 },
                new FrameTransformIndexPair() { OuterFrameIndex = 2, InnerFrameIndex = 3 },
                new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 3 }
            };

            var sdu = new GraphSdu(validTime, frameList, transformList);

            var expected =
                "{" +
                    $"\"validTime\":{validTime}," +
                    "\"frameList\":[" +
                        "{" +
                            $"\"authority\":\"{authority}\"," +
                            $"\"id\":\"{outerFrameId}\"," +
                            $"\"parameters\":{outerFrameParams}" +
                        "}," +
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
                    "]," +
                    "\"transformList\":[" +
                        "{" +
                            $"\"link\":[" +
                                "0,1" +
                            "]" +
                        "}," +
                        "{" +
                            $"\"link\":[" +
                                "1,2" +
                            "]" +
                        "}," +
                        "{" +
                            $"\"link\":[" +
                                "2,3" +
                            "]" +
                        "}," +
                        "{" +
                            $"\"link\":[" +
                                "0,3" +
                            "]" +
                        "}" +
                    "]" +
                "}";

            var json = JsonConvert.SerializeObject(sdu);
            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase(16534234327, "/Ethar.GeoPose/1.0", "LTP-ENU", "\"latitude=48&longitude=-122&heightInMeters=5\"", "Translate-Rotate",
            "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlyMakeARoundTripSerialization(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var frameList = new List<BaseFrameSpecification>()
            {
                new LtpEnuSpecification(new DataTypes.TangentPointPosition() {Latitude = 48, Longitude = -122, HeightInMeters = 5}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f})
            };

            var transformList = new List<FrameTransformIndexPair>()
            {
                new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                new FrameTransformIndexPair() { OuterFrameIndex = 1, InnerFrameIndex = 2 },
                new FrameTransformIndexPair() { OuterFrameIndex = 2, InnerFrameIndex = 3 },
                new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 3 }
            };

            var sdu = new GraphSdu(validTime, frameList, transformList);

            var json = JsonConvert.SerializeObject(sdu);
            var converted = JsonConvert.DeserializeObject<GraphSdu>(json);

            Assert.That(sdu, Is.EqualTo(converted));
        }

        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"", "\"Translate-Rotate\"", "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"", "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"", "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanConvertGraphSduToString(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var frameList = new List<BaseFrameSpecification>()
            {
                new LtpEnuSpecification(new DataTypes.TangentPointPosition() {Latitude = 48, Longitude = -122, HeightInMeters = 5}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f}),
                new TranslateRotateSpecification(new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f), new DataTypes.UnitQuaternion() {X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f})
            };

            var transformList = new List<FrameTransformIndexPair>()
            {
                new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                new FrameTransformIndexPair() { OuterFrameIndex = 1, InnerFrameIndex = 2 },
                new FrameTransformIndexPair() { OuterFrameIndex = 2, InnerFrameIndex = 3 },
                new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 3 }
            };

            var sdu = new GraphSdu(validTime, frameList, transformList);
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("ValidTime:16534234327, FrameListCount:4, TransformListCount:4"));
        }

        [TestCase(16534234327)]
        public void CanConvertGraphSduToStringWithNull(long validTime)
        {
            var sdu = new GraphSdu(validTime, null, null);
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("ValidTime:16534234327, FrameListCount:0, TransformListCount:0"));
        }

        [Test]
        public void CanConvertGraphSduToStringNew()
        {
            var sdu = new GraphSdu();
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("ValidTime:0, FrameListCount:0, TransformListCount:0"));
        }
    }
}
