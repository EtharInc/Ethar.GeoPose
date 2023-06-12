using Ethar.GeoPose.Authority.FrameSpecifications;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.FrameSpecifications;
using Ethar.GeoPose.StructuralDataUnits;
using NUnit.Framework;
using System.Collections.Generic;

namespace Ethar.GeoPose.UnitTests
{
    [TestFixture]
    public class GraphSduExtensionsUnitTests : UnitTestBase
    {
        [TestCaseSource(nameof(IndexValidationTestCaseData))]
        public bool CanCorrectlyValidateGraphSdu(List<BaseFrameSpecification> frameList,
            List<FrameTransformIndexPair> transformList)
        {
            var uut = new GraphSdu(
                1,
                frameList, transformList);
            return uut.Validate();
        }

        private static IEnumerable<TestCaseData> IndexValidationTestCaseData
        {
            get
            {
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                    },
                    new List<FrameTransformIndexPair>()
                    {
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                    }).Returns(true);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                    },
                    new List<FrameTransformIndexPair>()
                    {
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 2 },
                    }).Returns(false);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                    },
                    new List<FrameTransformIndexPair>()
                    {
                        new FrameTransformIndexPair() { OuterFrameIndex = 2, InnerFrameIndex = 0 },
                    }).Returns(false);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                            new DataTypes.UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f),
                            new DataTypes.UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f),
                            new DataTypes.UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f })
                    },
                    new List<FrameTransformIndexPair>()
                    {
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 1, InnerFrameIndex = 2 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 2, InnerFrameIndex = 3 },
                    }).Returns(true);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                            new DataTypes.UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f),
                            new DataTypes.UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f),
                            new DataTypes.UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f })
                    },
                    new List<FrameTransformIndexPair>()
                    {
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 2 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 3 },
                    }).Returns(true);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new DataTypes.TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f })
                    },
                    new List<FrameTransformIndexPair>()
                    {
                        new FrameTransformIndexPair() { OuterFrameIndex = 2, InnerFrameIndex = 3 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 1, InnerFrameIndex = 2 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                    }).Returns(true);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f })
                    },
                    new List<FrameTransformIndexPair>()
                    {
                        new FrameTransformIndexPair() { OuterFrameIndex = 2, InnerFrameIndex = 3 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 3, InnerFrameIndex = 0 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                    }).Returns(false);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new LtpEnuSpecification(new TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                    },
                    new List<FrameTransformIndexPair>()).Returns(true);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f })
                    },
                    new List<FrameTransformIndexPair>()).Returns(false);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new DataTypes.TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f })
                    },
                    new List<FrameTransformIndexPair>()
                    {
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 1, InnerFrameIndex = 2 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 2, InnerFrameIndex = 3 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 3, InnerFrameIndex = 1 },
                    }).Returns(false);
                yield return new TestCaseData(new List<BaseFrameSpecification>()
                    {
                        new LtpEnuSpecification(new DataTypes.TangentPointPosition()
                                { Latitude = 48, Longitude = -122, HeightInMeters = 5 }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.1f, 0.2f, 0.3f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.4f, 0.5f, 0.6f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f }),
                        new TranslateRotateSpecification(
                            new DataTypes.UnitVector3(0.7f, 0.8f, 0.9f),
                            new UnitQuaternion() { X = 0.692f, Y = 0.691f, Z = 0.141f, W = 0.14f })
                    },
                    new List<FrameTransformIndexPair>()
                    {
                        new FrameTransformIndexPair() { OuterFrameIndex = 0, InnerFrameIndex = 1 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 1, InnerFrameIndex = 2 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 2, InnerFrameIndex = 3 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 3, InnerFrameIndex = 4 },
                        new FrameTransformIndexPair() { OuterFrameIndex = 4, InnerFrameIndex = 3 },
                    }).Returns(true);
            }
        }
    }
}