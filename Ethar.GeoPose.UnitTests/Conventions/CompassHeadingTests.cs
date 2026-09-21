using Ethar.GeoPose.Conventions;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests.Conventions
{
    [TestFixture]
    internal class CompassHeadingTests
    {
        [TestCase(0, 90)]
        [TestCase(90, 0)]
        [TestCase(180, -90)]
        [TestCase(270, -180)]
        [TestCase(45, 45)]
        [TestCase(135, -45)]
        public void BearingToYawFollowsTheEastZeroCounterClockwiseConvention(double bearing, double expectedYaw)
        {
            Assert.That(CompassHeading.ToGeoPoseYaw(bearing), Is.EqualTo(expectedYaw).Within(1e-12));
        }

        [TestCase(90, 0)]
        [TestCase(0, 90)]
        [TestCase(-90, 180)]
        [TestCase(180, 270)]
        [TestCase(-180, 270)]
        public void YawToBearingIsTheInverse(double yaw, double expectedBearing)
        {
            Assert.That(CompassHeading.ToCompassBearing(yaw), Is.EqualTo(expectedBearing).Within(1e-12));
        }

        [Test]
        public void RoundTripCoversTheWholeCircle()
        {
            for (var bearing = 0.0; bearing < 360.0; bearing += 7.5)
            {
                var back = CompassHeading.ToCompassBearing(CompassHeading.ToGeoPoseYaw(bearing));
                Assert.That(back, Is.EqualTo(bearing).Within(1e-9), $"bearing {bearing}");
            }
        }

        [TestCase(350, 20, 330)]
        [TestCase(10, 20, 350)]
        [TestCase(90, 0, 90)]
        [TestCase(0, 0, 0)]
        public void EngineHeadingOffsetIsBearingOfTheEngineForwardAxis(double trueHeading, double engineYaw, double expected)
        {
            Assert.That(CompassHeading.EngineHeadingOffset(trueHeading, engineYaw), Is.EqualTo(expected).Within(1e-12));
        }

        [TestCase(0, 0)]
        [TestCase(360, 0)]
        [TestCase(-90, 270)]
        [TestCase(725, 5)]
        [TestCase(-0.0, 0)]
        public void Normalize360(double input, double expected)
        {
            Assert.That(Angles.Normalize360(input), Is.EqualTo(expected).Within(1e-12));
        }

        [TestCase(0, 0)]
        [TestCase(180, -180)]
        [TestCase(-180, -180)]
        [TestCase(270, -90)]
        [TestCase(-270, 90)]
        public void Normalize180(double input, double expected)
        {
            Assert.That(Angles.Normalize180(input), Is.EqualTo(expected).Within(1e-12));
        }
    }
}
