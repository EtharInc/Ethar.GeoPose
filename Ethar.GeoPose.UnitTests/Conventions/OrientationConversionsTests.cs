using Ethar.GeoPose.Conventions;
using Ethar.GeoPose.DataTypes;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests.Conventions
{
    [TestFixture]
    internal class OrientationConversionsTests
    {
        private const double Tolerance = 1e-9;

        [Test]
        public void YawOfNinetyTurnsEastToNorth()
        {
            var q = new YawPitchRollAngles(90, 0, 0).ToQuaternion();

            var rotated = q.Rotate(UnitVector3.UnitX);

            AssertVector(rotated, 0, 1, 0);
        }

        [Test]
        public void PositiveYawIsCounterClockwiseFromAbove()
        {
            var q = new YawPitchRollAngles(90, 0, 0).ToQuaternion();

            var rotated = q.Rotate(UnitVector3.UnitY);

            // North turns to West under a right-handed rotation about Up.
            AssertVector(rotated, -1, 0, 0);
        }

        [Test]
        public void PitchIsAboutTheNorthAxis()
        {
            var q = new YawPitchRollAngles(0, 90, 0).ToQuaternion();

            var rotated = q.Rotate(UnitVector3.UnitX);

            // A right-handed rotation about y carries x onto -z.
            AssertVector(rotated, 0, 0, -1);
        }

        [Test]
        public void RollIsAboutTheEastAxis()
        {
            var q = new YawPitchRollAngles(0, 0, 90).ToQuaternion();

            var rotated = q.Rotate(UnitVector3.UnitY);

            // A right-handed rotation about x carries y onto z.
            AssertVector(rotated, 0, 0, 1);
        }

        [Test]
        public void RotationsApplyInYawPitchRollOrder()
        {
            var combined = new YawPitchRollAngles(90, 45, 0).ToQuaternion();
            var yawThenPitch = new YawPitchRollAngles(90, 0, 0).ToQuaternion().Multiply(new YawPitchRollAngles(0, 45, 0).ToQuaternion());

            AssertQuaternion(combined, yawThenPitch);
        }

        [TestCase(0, 0, 0)]
        [TestCase(90, 0, 0)]
        [TestCase(-135, 30, -60)]
        [TestCase(12, 16, 8)]
        [TestCase(179, -89, 1)]
        [TestCase(-45, 45, 45)]
        public void YawPitchRollRoundTripsThroughAQuaternion(double yaw, double pitch, double roll)
        {
            var original = new YawPitchRollAngles(yaw, pitch, roll);

            var back = original.ToQuaternion().ToYawPitchRoll();

            Assert.That(back.Yaw, Is.EqualTo(yaw).Within(1e-7));
            Assert.That(back.Pitch, Is.EqualTo(pitch).Within(1e-7));
            Assert.That(back.Roll, Is.EqualTo(roll).Within(1e-7));
        }

        [Test]
        public void GimbalLockAttributesRotationToYaw()
        {
            var q = new YawPitchRollAngles(30, 90, 0).ToQuaternion();

            var back = q.ToYawPitchRoll();

            Assert.That(back.Pitch, Is.EqualTo(90).Within(1e-7));
            Assert.That(back.Roll, Is.EqualTo(0).Within(1e-7));
            AssertQuaternion(back.ToQuaternion(), q);
        }

        [Test]
        public void ConjugateUndoesTheRotation()
        {
            var q = new YawPitchRollAngles(-70, 20, 110).ToQuaternion();

            AssertQuaternion(q.Multiply(q.Conjugate()), UnitQuaternion.Identity);
        }

        [Test]
        public void TwoQuarterTurnsMakeAHalfTurn()
        {
            var quarter = OrientationConversions.FromAxisAngle(UnitVector3.UnitZ, 90);

            var half = quarter.Multiply(quarter);

            AssertVector(half.Rotate(UnitVector3.UnitX), -1, 0, 0);
        }

        [Test]
        public void NormalizeScalesToUnitLength()
        {
            var scaled = new UnitQuaternion(0, 0, 2, 2);

            Assert.That(scaled.IsUnit(), Is.False);
            Assert.That(scaled.Normalize().IsUnit(), Is.True);
            Assert.That(scaled.Normalize().Norm(), Is.EqualTo(1).Within(Tolerance));
        }

        [Test]
        public void ConversionsProduceUnitQuaternions()
        {
            Assert.That(new YawPitchRollAngles(33, -44, 55).ToQuaternion().IsUnit(1e-12), Is.True);
            Assert.That(OrientationConversions.FromAxisAngle(new UnitVector3(3, 4, 0), 17).IsUnit(1e-12), Is.True);
        }

        private static void AssertVector(UnitVector3 actual, double x, double y, double z)
        {
            Assert.That(actual.X, Is.EqualTo(x).Within(Tolerance));
            Assert.That(actual.Y, Is.EqualTo(y).Within(Tolerance));
            Assert.That(actual.Z, Is.EqualTo(z).Within(Tolerance));
        }

        private static void AssertQuaternion(UnitQuaternion actual, UnitQuaternion expected)
        {
            // q and -q are the same rotation.
            var sign = (actual.W * expected.W) + (actual.X * expected.X) + (actual.Y * expected.Y) + (actual.Z * expected.Z) < 0 ? -1.0 : 1.0;
            Assert.That(actual.X * sign, Is.EqualTo(expected.X).Within(Tolerance));
            Assert.That(actual.Y * sign, Is.EqualTo(expected.Y).Within(Tolerance));
            Assert.That(actual.Z * sign, Is.EqualTo(expected.Z).Within(Tolerance));
            Assert.That(actual.W * sign, Is.EqualTo(expected.W).Within(Tolerance));
        }
    }
}
