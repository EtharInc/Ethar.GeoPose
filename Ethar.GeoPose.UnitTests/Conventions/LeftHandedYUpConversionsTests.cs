using Ethar.GeoPose.Conventions;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.Geodesy;
using Ethar.GeoPose.StructuralDataUnits;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests.Conventions
{
    [TestFixture]
    internal class LeftHandedYUpConversionsTests
    {
        private const double Tolerance = 1e-9;
        private static readonly TangentPointPosition London = new TangentPointPosition(51.5074, -0.1278, 0);

        [Test]
        public void NorthMapsToEngineForwardWhenTheEngineFacesNorth()
        {
            var engine = new EnuVector(0, 10, 0).ToLeftHandedYUp(0);

            AssertVector(engine, 0, 0, 10);
        }

        [Test]
        public void NorthMapsToEngineLeftWhenTheEngineFacesEast()
        {
            var engine = new EnuVector(0, 10, 0).ToLeftHandedYUp(90);

            AssertVector(engine, -10, 0, 0);
        }

        [Test]
        public void EastMapsToEngineForwardWhenTheEngineFacesEast()
        {
            var engine = new EnuVector(10, 0, 0).ToLeftHandedYUp(90);

            AssertVector(engine, 0, 0, 10);
        }

        [Test]
        public void UpMapsToEngineY()
        {
            var engine = new EnuVector(0, 0, 5).ToLeftHandedYUp(123);

            AssertVector(engine, 0, 5, 0);
        }

        [TestCase(0)]
        [TestCase(37.5)]
        [TestCase(90)]
        [TestCase(271)]
        public void VectorMappingRoundTrips(double headingOffset)
        {
            var enu = new EnuVector(3, -4, 5);

            var back = enu.ToLeftHandedYUp(headingOffset).ToEnu(headingOffset);

            Assert.That(back.East, Is.EqualTo(enu.East).Within(Tolerance));
            Assert.That(back.North, Is.EqualTo(enu.North).Within(Tolerance));
            Assert.That(back.Up, Is.EqualTo(enu.Up).Within(Tolerance));
        }

        [Test]
        public void EnuYawOfNinetyBecomesEngineRotationCarryingXToForward()
        {
            var enuRotation = new YawPitchRollAngles(90, 0, 0).ToQuaternion();

            var engineRotation = enuRotation.ToLeftHandedYUp(0);
            var rotated = engineRotation.Rotate(UnitVector3.UnitX);

            // The pose faces North; North is engine +Z when the heading offset is zero.
            AssertVector(rotated, 0, 0, 1);
        }

        [Test]
        public void HeadingOffsetTurnsTheMappedRotation()
        {
            var enuRotation = new YawPitchRollAngles(90, 0, 0).ToQuaternion();

            var engineRotation = enuRotation.ToLeftHandedYUp(90);
            var rotated = engineRotation.Rotate(UnitVector3.UnitX);

            // The pose faces North; when the engine faces East, North is engine -X.
            AssertVector(rotated, -1, 0, 0);
        }

        [Test]
        public void PositiveEnuYawBecomesNegativeEngineYaw()
        {
            var enuRotation = new YawPitchRollAngles(30, 0, 0).ToQuaternion();

            var engineRotation = enuRotation.ToLeftHandedYUp(0);

            // Right-handed +30° about Up must become -30° about engine Y (whose positive sense is clockwise).
            Assert.That(engineRotation.Y, Is.LessThan(0));
            Assert.That(engineRotation.X, Is.EqualTo(0).Within(Tolerance));
            Assert.That(engineRotation.Z, Is.EqualTo(0).Within(Tolerance));
        }

        [TestCase(0)]
        [TestCase(45)]
        [TestCase(200)]
        public void RotationMappingRoundTrips(double headingOffset)
        {
            var enuRotation = new YawPitchRollAngles(-25, 10, 70).ToQuaternion();

            var back = enuRotation.ToLeftHandedYUp(headingOffset).ToEnu(headingOffset);

            AssertQuaternion(back, enuRotation);
        }

        [Test]
        public void FrameMapsAPointTenMetresNorthToEngineForward()
        {
            var frame = new LeftHandedYUpFrame(London, new LeftHandedYUpVector(1, 2, 3), 0);
            var target = London.OffsetBy(new EnuVector(0, 10, 0));

            var engine = frame.ToEngine(target);

            AssertVector(engine, 1, 2, 13, 1e-6);
        }

        [Test]
        public void FrameMapsAPointTenMetresNorthToEngineLeftWhenFacingEast()
        {
            var frame = new LeftHandedYUpFrame(London, new LeftHandedYUpVector(1, 2, 3), 90);
            var target = London.OffsetBy(new EnuVector(0, 10, 0));

            var engine = frame.ToEngine(target);

            AssertVector(engine, -9, 2, 3, 1e-6);
        }

        [Test]
        public void FramePositionRoundTrips()
        {
            var frame = new LeftHandedYUpFrame(London, new LeftHandedYUpVector(-4, 1.5, 8), 212);
            var target = London.OffsetBy(new EnuVector(120, -35, 6));

            var back = frame.ToGeodetic(frame.ToEngine(target));

            Assert.That(back.Latitude, Is.EqualTo(target.Latitude).Within(1e-10));
            Assert.That(back.Longitude, Is.EqualTo(target.Longitude).Within(1e-10));
            Assert.That(back.HeightInMeters, Is.EqualTo(target.HeightInMeters).Within(1e-6));
        }

        [Test]
        public void GeoPoseFacingNorthPointsItsXAxisNorthInTheEngine()
        {
            var frame = new LeftHandedYUpFrame(London, LeftHandedYUpVector.Zero, 0);
            var sdu = new BasicYawPitchRollSdu(new YawPitchRollAngles(CompassHeading.ToGeoPoseYaw(0), 0, 0), London);

            var pose = frame.ToEngine(sdu, ForwardAxis.GeoPoseX);

            AssertVector(pose.Rotation.Rotate(UnitVector3.UnitX), 0, 0, 1);
        }

        [Test]
        public void EngineZForwardFacesTheBearingTheGeoPoseFaces()
        {
            var frame = new LeftHandedYUpFrame(London, LeftHandedYUpVector.Zero, 0);
            var facingNorth = new BasicYawPitchRollSdu(new YawPitchRollAngles(CompassHeading.ToGeoPoseYaw(0), 0, 0), London);
            var facingEast = new BasicYawPitchRollSdu(new YawPitchRollAngles(CompassHeading.ToGeoPoseYaw(90), 0, 0), London);

            var northPose = frame.ToEngine(facingNorth, ForwardAxis.EngineZ);
            var eastPose = frame.ToEngine(facingEast, ForwardAxis.EngineZ);

            AssertVector(northPose.Rotation.Rotate(UnitVector3.UnitZ), 0, 0, 1);
            AssertVector(eastPose.Rotation.Rotate(UnitVector3.UnitZ), 1, 0, 0);
        }

        [Test]
        public void EngineZForwardStillRespectsTheHeadingOffset()
        {
            var frame = new LeftHandedYUpFrame(London, LeftHandedYUpVector.Zero, 90);
            var facingNorth = new BasicYawPitchRollSdu(new YawPitchRollAngles(CompassHeading.ToGeoPoseYaw(0), 0, 0), London);

            var pose = frame.ToEngine(facingNorth, ForwardAxis.EngineZ);

            // Engine faces East, so North is engine -X.
            AssertVector(pose.Rotation.Rotate(UnitVector3.UnitZ), -1, 0, 0);
        }

        [TestCase(ForwardAxis.GeoPoseX)]
        [TestCase(ForwardAxis.EngineZ)]
        public void BasicYawPitchRollRoundTripsThroughTheEngine(ForwardAxis forwardAxis)
        {
            var frame = new LeftHandedYUpFrame(London, new LeftHandedYUpVector(2, 0, -1), 305);
            var sdu = new BasicYawPitchRollSdu(new YawPitchRollAngles(-40, 15, 5), London.OffsetBy(new EnuVector(30, 40, 2)));

            var back = frame.ToBasicYawPitchRoll(frame.ToEngine(sdu, forwardAxis), forwardAxis);

            Assert.That(back.Angles.Yaw, Is.EqualTo(sdu.Angles.Yaw).Within(1e-7));
            Assert.That(back.Angles.Pitch, Is.EqualTo(sdu.Angles.Pitch).Within(1e-7));
            Assert.That(back.Angles.Roll, Is.EqualTo(sdu.Angles.Roll).Within(1e-7));
            Assert.That(back.Position.Latitude, Is.EqualTo(sdu.Position.Latitude).Within(1e-10));
            Assert.That(back.Position.Longitude, Is.EqualTo(sdu.Position.Longitude).Within(1e-10));
            Assert.That(back.Position.HeightInMeters, Is.EqualTo(sdu.Position.HeightInMeters).Within(1e-6));
        }

        [Test]
        public void BasicQuaternionRoundTripsThroughTheEngine()
        {
            var frame = new LeftHandedYUpFrame(London, LeftHandedYUpVector.Zero, 77);
            var sdu = new BasicQuaternionSdu(London.OffsetBy(new EnuVector(-3, 9, 1)), new YawPitchRollAngles(100, -20, 33).ToQuaternion());

            var back = frame.ToBasicQuaternion(frame.ToEngine(sdu));

            AssertQuaternion(back.Quaternion, sdu.Quaternion);
            Assert.That(back.Position.Latitude, Is.EqualTo(sdu.Position.Latitude).Within(1e-10));
        }

        [Test]
        public void HeadingOffsetIsNormalised()
        {
            var frame = new LeftHandedYUpFrame(London, LeftHandedYUpVector.Zero, -90);

            Assert.That(frame.HeadingOffsetDegrees, Is.EqualTo(270).Within(Tolerance));
        }

        private static void AssertVector(LeftHandedYUpVector actual, double x, double y, double z, double tolerance = Tolerance)
        {
            Assert.That(actual.X, Is.EqualTo(x).Within(tolerance));
            Assert.That(actual.Y, Is.EqualTo(y).Within(tolerance));
            Assert.That(actual.Z, Is.EqualTo(z).Within(tolerance));
        }

        private static void AssertVector(UnitVector3 actual, double x, double y, double z)
        {
            Assert.That(actual.X, Is.EqualTo(x).Within(Tolerance));
            Assert.That(actual.Y, Is.EqualTo(y).Within(Tolerance));
            Assert.That(actual.Z, Is.EqualTo(z).Within(Tolerance));
        }

        private static void AssertQuaternion(UnitQuaternion actual, UnitQuaternion expected)
        {
            var sign = (actual.W * expected.W) + (actual.X * expected.X) + (actual.Y * expected.Y) + (actual.Z * expected.Z) < 0 ? -1.0 : 1.0;
            Assert.That(actual.X * sign, Is.EqualTo(expected.X).Within(Tolerance));
            Assert.That(actual.Y * sign, Is.EqualTo(expected.Y).Within(Tolerance));
            Assert.That(actual.Z * sign, Is.EqualTo(expected.Z).Within(Tolerance));
            Assert.That(actual.W * sign, Is.EqualTo(expected.W).Within(Tolerance));
        }
    }
}
