﻿using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.Authority.TransitionModels;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Ethar.GeoPose.UnitTests
{
    [TestFixture]
    internal class IrregularSeriesSduUnitTests : UnitTestBase
    {
        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"", "\"Translate-Rotate\"",
            "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlyDeserializeIrregularSeriesSdu(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var json =
                "{" +
                    "\"header\":" +
                    "{" +
                        $"\"transitionModel\":" +
                        "{" +
                            $"\"authority\": {authority}," +
                            $"\"id\": \"none\"," +
                            $"\"parameters\": \"\"" +
                        "}," +
                        $"\"poseCount\": 2," +
                        $"\"integrityCheck\": \"{{\\\"SHA256\\\": \\\"5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47858\\\"}}\"," +
                        $"\"startInstant\": {validTime}," +
                        $"\"stopInstant\": {validTime}" +
                    "}," +
                    "\"innerFrameAndTimeSeries\": [" +
                        "{" +
                            "\"frame\":" +
                            "{" +
                                $"\"authority\": {authority}," +
                                $"\"id\": {innerFrameId}," +
                                $"\"parameters\": {innerFrameParameters1}" +
                            "}," +
                            $"\"validTime\": {validTime}," +
                        "}," +
                        "{" +
                            "\"frame\":" +
                            "{" +
                                $"\"authority\": {authority}," +
                                $"\"id\": {innerFrameId}," +
                                $"\"parameters\": {innerFrameParameters2}" +
                            "}," +
                            $"\"validTime\": {validTime}," +
                        "}," +
                        "{" +
                            "\"frame\":" +
                            "{" +
                                $"\"authority\": {authority}," +
                                $"\"id\": {innerFrameId}," +
                                $"\"parameters\": {innerFrameParameters3}" +
                            "}," +
                            $"\"validTime\": {validTime}," +
                        "}" +
                    "]," +
                    "\"outerFrame\":" +
                    "{" +
                        $"\"authority\": {authority}," +
                        $"\"id\": {outerFrameId}," +
                        $"\"parameters\": {outerFrameParams}" +
                    "}," +
                    $"\"trailer\":" +
                    "{" +
                        $"\"poseCount\": 2," +
                        $"\"integrityCheck\":  \"{{\\\"SHA256\\\": \\\"5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47858\\\"}}\"," +
                    "}" +
                "}";

            var sdu = JsonConvert.DeserializeObject<IrregularSeriesSdu>(json);

            Assert.That(sdu.Header.TransitionModel is NoneTransitionModel);

            Assert.That(sdu.OuterFrame is LtpEnuSpecification);
            var outerFrame = (LtpEnuSpecification)sdu.OuterFrame;

            Assert.That(outerFrame.Position.Latitude, Is.EqualTo(48));
            Assert.That(outerFrame.Position.Longitude, Is.EqualTo(-122));
            Assert.That(outerFrame.Position.HeightInMeters, Is.EqualTo(5));

            Assert.That(sdu.InnerFrameAndTimeSeries.All(x => x.Frame is TranslateRotateSpecification));

            var translateRotateList = sdu.InnerFrameAndTimeSeries.Select(x => x.Frame as TranslateRotateSpecification).ToList();
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
        public void CanCorrectlySerializeIrregularSeriesSdu(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var header = new SeriesHeader()
            {
                TransitionModel = new NoneTransitionModel(),
                IntegrityCheck =
                    "SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47858",
                PoseCount = 2,
                StartInstant = validTime,
                StopInstant = validTime
            };

            var outerFrame = new LtpEnuSpecification(new TangentPointPosition() { Latitude = 48, Longitude = -122, HeightInMeters = 5 });

            var trailer = new SeriesTrailer()
            {
                IntegrityCheck =
                    "SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47859",
                PoseCount = 2
            };

            var innerFrames = new List<FrameAndTimeElement>
            {
                new FrameAndTimeElement()
                {
                    Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f), new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                    ValidTime = validTime,
                },
                new FrameAndTimeElement()
                {
                    Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f), new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                    ValidTime = validTime,
                },
                new FrameAndTimeElement()
                {
                    Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f), new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                    ValidTime = validTime,
                }
            };

            var sdu = new IrregularSeriesSdu(header, outerFrame, trailer, innerFrames);

            var expected =
                "{" +
                    "\"header\":" +
                    "{" +
                        $"\"transitionModel\":" +
                        "{" +
                            $"\"authority\":\"{authority}\"," +
                            $"\"id\":\"none\"," +
                            $"\"parameters\":\"\"" +
                        "}," +
                        $"\"poseCount\":{sdu.Header.PoseCount}," +
                        $"\"startInstant\":{validTime}," +
                        $"\"stopInstant\":{validTime}," +
                        $"\"integrityCheck\":\"SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47858\"" +
                    "}," +
                    "\"outerFrame\":" +
                    "{" +
                        $"\"authority\":\"{authority}\"," +
                        $"\"id\":\"{outerFrameId}\"," +
                        $"\"parameters\":{outerFrameParams}" +
                    "}," +
                    "\"innerFrameAndTimeSeries\":[" +
                        "{" +
                            "\"frame\":" +
                            "{" +
                                $"\"authority\":\"{authority}\"," +
                                $"\"id\":\"{innerFrameId}\"," +
                                $"\"parameters\":{innerFrameParameters1}" +
                            "}," +
                            $"\"validTime\":{validTime}" +
                        "}," +
                        "{" +
                            "\"frame\":" +
                            "{" +
                                $"\"authority\":\"{authority}\"," +
                                $"\"id\":\"{innerFrameId}\"," +
                                $"\"parameters\":{innerFrameParameters2}" +
                            "}," +
                            $"\"validTime\":{validTime}" +
                        "}," +
                        "{" +
                            "\"frame\":" +
                            "{" +
                                $"\"authority\":\"{authority}\"," +
                                $"\"id\":\"{innerFrameId}\"," +
                                $"\"parameters\":{innerFrameParameters3}" +
                            "}," +
                            $"\"validTime\":{validTime}" +
                        "}" +
                    "]," +
                    $"\"trailer\":" +
                    "{" +
                        $"\"poseCount\":{sdu.Trailer.PoseCount}," +
                        $"\"integrityCheck\":\"SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47859\"" +
                    "}" +
                "}";

            var json = JsonConvert.SerializeObject(sdu);
            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase(16534234327, "/Ethar.GeoPose/1.0", "LTP-ENU", "\"latitude=48&longitude=-122&heightInMeters=5\"",
            "Translate-Rotate",
            "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanCorrectlyMakeARoundTripConversion(long validTime, string authority, string outerFrameId,
            string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2,
            string innerFrameParameters3)
        {
            var header = new SeriesHeader()
            {
                TransitionModel = new NoneTransitionModel(),
                IntegrityCheck =
                    "SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47858",
                PoseCount = 2,
                StartInstant = validTime,
                StopInstant = validTime
            };

            var outerFrame = new LtpEnuSpecification(new TangentPointPosition() { Latitude = 48, Longitude = -122, HeightInMeters = 5 });

            var trailer = new SeriesTrailer()
            {
                IntegrityCheck =
                    "SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47859",
                PoseCount = 2
            };

            var innerFrames = new List<FrameAndTimeElement>
            {
                new FrameAndTimeElement()
                {
                    Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f), new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                    ValidTime = validTime,
                },
                new FrameAndTimeElement()
                {
                    Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f), new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                    ValidTime = validTime,
                },
                new FrameAndTimeElement()
                {
                    Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f), new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                    ValidTime = validTime,
                }
            };

            var sdu = new IrregularSeriesSdu(header, outerFrame, trailer, innerFrames);

            var json = JsonConvert.SerializeObject(sdu);
            var converted = JsonConvert.DeserializeObject<IrregularSeriesSdu>(json);

            Assert.That(sdu, Is.EqualTo(converted));
        }

        [TestCase(16534234327, "\"/Ethar.GeoPose/1.0\"", "\"LTP-ENU\"", "\"longitude=-122.0000000&latitude=48.0000000&heightInMeters=5.000\"", "\"Translate-Rotate\"",
            "\"translation.x=0.1&translation.y=0.2&translation.z=0.3&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.4&translation.y=0.5&translation.z=0.6&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"",
            "\"translation.x=0.7&translation.y=0.8&translation.z=0.9&rotation.x=0.692&rotation.y=0.691&rotation.z=0.141&rotation.w=0.14\"")]
        public void CanConvertIrregularSeriesSduToString(long validTime, string authority, string outerFrameId, string outerFrameParams, string innerFrameId, string innerFrameParameters1, string innerFrameParameters2, string innerFrameParameters3)
        {
            var header = new SeriesHeader()
            {
                TransitionModel = new NoneTransitionModel(),
                IntegrityCheck =
                    "SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47858",
                PoseCount = 2,
                StartInstant = validTime,
                StopInstant = validTime
            };

            var outerFrame = new LtpEnuSpecification(new TangentPointPosition() { Latitude = 48, Longitude = -122, HeightInMeters = 5 });

            var trailer = new SeriesTrailer()
            {
                IntegrityCheck =
                    "SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47859",
                PoseCount = 2
            };

            var innerFrames = new List<FrameAndTimeElement>
            {
                new FrameAndTimeElement()
                {
                    Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f), new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                    ValidTime = validTime,
                },
                new FrameAndTimeElement()
                {
                    Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f), new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                    ValidTime = validTime,
                },
                new FrameAndTimeElement()
                {
                    Frame = new TranslateRotateSpecification(new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f), new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                    ValidTime = validTime,
                }
            };

            var sdu = new IrregularSeriesSdu(header, outerFrame, trailer, innerFrames);
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("Header:[TransitionModel:[Authority:/Ethar.GeoPose/1.0, Id:none], PoseCount:2, StartInstant:16534234327, StopInstant:16534234327, IntegrityCheck:SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47858], OuterFrame:[Authority:/Ethar.GeoPose/1.0, Id:LTP-ENU], InnerFrameAndTimeSeriesCount:3, Trailer:[PoseCount:2, IntegrityCheck:SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47859]"));
        }

        [TestCase(16534234327)]
        public void CanConvertIrregularSeriesSduToStringWithNull(long validTime)
        {
            var header = new SeriesHeader()
            {
                TransitionModel = new NoneTransitionModel(),
                IntegrityCheck =
                    "SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47858",
                PoseCount = 2,
                StartInstant = validTime,
                StopInstant = validTime
            };

            var trailer = new SeriesTrailer()
            {
                IntegrityCheck =
                    "SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47859",
                PoseCount = 2
            };

            var sdu = new IrregularSeriesSdu(header, null, trailer, null);
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("Header:[TransitionModel:[Authority:/Ethar.GeoPose/1.0, Id:none], PoseCount:2, StartInstant:16534234327, StopInstant:16534234327, IntegrityCheck:SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47858], OuterFrame:[], InnerFrameAndTimeSeriesCount:0, Trailer:[PoseCount:2, IntegrityCheck:SHA256: 5556fb65f8bf9eddb3ace1329c9a6aeedd4833409965aeee3e6b61ed21f47859]"));
        }

        [Test]
        public void CanConvertIrregularSeriesSduToStringNew()
        {
            var sdu = new IrregularSeriesSdu();
            var result = sdu.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("Header:[TransitionModel:[], PoseCount:0, StartInstant:0, StopInstant:0, IntegrityCheck:], OuterFrame:[], InnerFrameAndTimeSeriesCount:0, Trailer:[PoseCount:0, IntegrityCheck:]"));
        }
    }
}