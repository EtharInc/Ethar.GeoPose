using Ethar.GeoPose.Conventions;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests.Conventions
{
    /// <summary>
    /// Proves the yaw, pitch, roll conversions beyond the single convention the standard names: every Tait-Bryan axis order, intrinsic and
    /// extrinsic, degrees and radians, and the conventions found in the OGC reference code and example data.
    /// </summary>
    [TestFixture]
    internal class EulerConventionTests
    {
        private const double Tolerance = 1e-9;
        private static readonly string FixtureDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", "Ogc");

        private static readonly (RotationAxis, RotationAxis, RotationAxis)[] AllOrders =
        {
            (RotationAxis.X, RotationAxis.Y, RotationAxis.Z),
            (RotationAxis.X, RotationAxis.Z, RotationAxis.Y),
            (RotationAxis.Y, RotationAxis.X, RotationAxis.Z),
            (RotationAxis.Y, RotationAxis.Z, RotationAxis.X),
            (RotationAxis.Z, RotationAxis.X, RotationAxis.Y),
            (RotationAxis.Z, RotationAxis.Y, RotationAxis.X),
        };

        private static readonly (double yaw, double pitch, double roll)[] SampleAngles =
        {
            (0, 0, 0),
            (90, 0, 0),
            (0, 45, 0),
            (0, 0, -30),
            (12, 16, 8),
            (-135, 30, -60),
            (179, -89, 1),
            (-45, 45, 45),
            (126.5898, 13.8023, 0.3978),
            (5.5, -0.44, 0),
        };

        public static IEnumerable<TestCaseData> OrderUnitAndAngles()
        {
            foreach (var (a, b, c) in AllOrders)
            {
                foreach (var intrinsic in new[] { true, false })
                {
                    foreach (var unit in new[] { AngleUnit.Degrees, AngleUnit.Radians })
                    {
                        foreach (var (yaw, pitch, roll) in SampleAngles)
                        {
                            yield return new TestCaseData(new EulerConvention(a, b, c, unit, intrinsic), yaw, pitch, roll)
                                .SetName($"RoundTrip_{(intrinsic ? "Intrinsic" : "Extrinsic")}_{a}{b}{c}_{unit}_{yaw}_{pitch}_{roll}");
                        }
                    }
                }
            }
        }

        [TestCaseSource(nameof(OrderUnitAndAngles))]
        public void AnglesRoundTripThroughAQuaternionUnderEveryConvention(EulerConvention convention, double yawDeg, double pitchDeg, double rollDeg)
        {
            var original = new YawPitchRollAngles(convention.FromDegrees(yawDeg), convention.FromDegrees(pitchDeg), convention.FromDegrees(rollDeg));

            var quaternion = original.ToQuaternion(convention);
            var back = quaternion.ToYawPitchRoll(convention);

            Assert.That(quaternion.IsUnit(1e-12), Is.True);
            Assert.That(back.ToQuaternion(convention).AngleTo(quaternion), Is.LessThan(1e-7), "the recovered angles must describe the same rotation");
            if (Math.Abs(pitchDeg) < 89)
            {
                Assert.That(back.Yaw, Is.EqualTo(original.Yaw).Within(1e-7));
                Assert.That(back.Pitch, Is.EqualTo(original.Pitch).Within(1e-7));
                Assert.That(back.Roll, Is.EqualTo(original.Roll).Within(1e-7));
            }
        }

        [Test]
        public void TheStandardConventionIsIntrinsicZyxDegrees()
        {
            var standard = EulerConvention.GeoPoseStandard;

            Assert.That(standard.YawAxis, Is.EqualTo(RotationAxis.Z));
            Assert.That(standard.PitchAxis, Is.EqualTo(RotationAxis.Y));
            Assert.That(standard.RollAxis, Is.EqualTo(RotationAxis.X));
            Assert.That(standard.Unit, Is.EqualTo(AngleUnit.Degrees));
            Assert.That(standard.IsIntrinsic, Is.True);
            AssertSameRotation(new YawPitchRollAngles(12, 16, 8).ToQuaternion(), new YawPitchRollAngles(12, 16, 8).ToQuaternion(standard));
        }

        [Test]
        public void TheStandardConventionMatchesTheGeoPoseSandboxSequence()
        {
            // OGC GeoPoseSandbox Entity.ts: rotateZ(yaw), rotateY(pitch), rotateX(roll) on the local axes, degrees to radians, no negation.
            var yaw = 33.0;
            var pitch = -21.0;
            var roll = 57.0;
            var sandbox = OrientationConversions.FromAxisAngle(RotationAxis.Z, yaw)
                .Multiply(OrientationConversions.FromAxisAngle(RotationAxis.Y, pitch))
                .Multiply(OrientationConversions.FromAxisAngle(RotationAxis.X, roll));

            AssertSameRotation(new YawPitchRollAngles(yaw, pitch, roll).ToQuaternion(), sandbox);
        }

        [Test]
        public void ExtrinsicIsTheReverseOfIntrinsic()
        {
            // Extrinsic x-y-z with (α, β, γ) equals intrinsic z-y-x with (γ, β, α).
            var extrinsic = new EulerConvention(RotationAxis.X, RotationAxis.Y, RotationAxis.Z, AngleUnit.Degrees, false);
            var intrinsic = new EulerConvention(RotationAxis.Z, RotationAxis.Y, RotationAxis.X, AngleUnit.Degrees, true);

            var a = new YawPitchRollAngles(10, 20, 30).ToQuaternion(extrinsic);
            var b = new YawPitchRollAngles(30, 20, 10).ToQuaternion(intrinsic);

            AssertSameRotation(a, b);
        }

        [Test]
        public void RadiansAndDegreesDescribeTheSameRotation()
        {
            var degrees = new EulerConvention(RotationAxis.Z, RotationAxis.Y, RotationAxis.X, AngleUnit.Degrees);
            var radians = new EulerConvention(RotationAxis.Z, RotationAxis.Y, RotationAxis.X, AngleUnit.Radians);

            var fromDegrees = new YawPitchRollAngles(90, 45, -30).ToQuaternion(degrees);
            var fromRadians = new YawPitchRollAngles(Math.PI / 2, Math.PI / 4, -Math.PI / 6).ToQuaternion(radians);

            AssertSameRotation(fromDegrees, fromRadians);
            var converted = new YawPitchRollAngles(90, 45, -30).ConvertTo(degrees, radians);
            Assert.That(converted.Yaw, Is.EqualTo(Math.PI / 2).Within(Tolerance));
            Assert.That(converted.Pitch, Is.EqualTo(Math.PI / 4).Within(Tolerance));
            Assert.That(converted.Roll, Is.EqualTo(-Math.PI / 6).Within(Tolerance));
        }

        [Test]
        public void ConvertToBetweenAxisOrdersPreservesTheRotation()
        {
            var zyx = EulerConvention.GeoPoseStandard;
            var xyz = new EulerConvention(RotationAxis.X, RotationAxis.Y, RotationAxis.Z, AngleUnit.Degrees);
            var original = new YawPitchRollAngles(-70, 25, 140);

            var asXyz = original.ConvertTo(zyx, xyz);
            var back = asXyz.ConvertTo(xyz, zyx);

            AssertSameRotation(asXyz.ToQuaternion(xyz), original.ToQuaternion(zyx));
            Assert.That(back.Yaw, Is.EqualTo(original.Yaw).Within(1e-7));
            Assert.That(back.Pitch, Is.EqualTo(original.Pitch).Within(1e-7));
            Assert.That(back.Roll, Is.EqualTo(original.Roll).Within(1e-7));
        }

        [Test]
        public void SameAxisTwiceIsRejected()
        {
            Assert.Throws<ArgumentException>(() => new EulerConvention(RotationAxis.Z, RotationAxis.Z, RotationAxis.X, AngleUnit.Degrees));
            Assert.Throws<ArgumentException>(() => new EulerConvention(RotationAxis.X, RotationAxis.Y, RotationAxis.X, AngleUnit.Degrees));
        }

        [TestCase("00")]
        [TestCase("01")]
        [TestCase("02")]
        [TestCase("03")]
        public void OgcInstancePairsAgreeOnlyUnderTheInstanceFileConvention(string index)
        {
            var ypr = JsonConvert.DeserializeObject<BasicYawPitchRollSdu>(Load($"GeoPose.Basic.YPR.Instance.{index}.json"));
            var quaternion = JsonConvert.DeserializeObject<BasicQuaternionSdu>(Load($"GeoPose.Basic.Quaternion.Instance.{index}.json"));
            Assert.That(ypr.Position, Is.EqualTo(quaternion.Position), "the pair describes the same location");

            var underInstanceConvention = ypr.Angles.ToQuaternion(EulerConvention.OgcInstanceFiles);
            var underStandard = ypr.Angles.ToQuaternion(EulerConvention.GeoPoseStandard);

            // The published pair is self-consistent only as radians about z then x then y.
            Assert.That(underInstanceConvention.AngleTo(quaternion.Quaternion), Is.LessThan(1e-4), "instance-file convention");

            // Read as the standard requires (degrees, z then y then x) the same numbers are a different rotation: about 55° away.
            Assert.That(underStandard.AngleTo(quaternion.Quaternion), Is.GreaterThan(30), "standard convention");
        }

        [Test]
        public void OgcInstanceAnglesCanBeReExpressedInTheStandardConvention()
        {
            var ypr = JsonConvert.DeserializeObject<BasicYawPitchRollSdu>(Load("GeoPose.Basic.YPR.Instance.00.json"));
            var quaternion = JsonConvert.DeserializeObject<BasicQuaternionSdu>(Load("GeoPose.Basic.Quaternion.Instance.00.json"));

            var standardAngles = ypr.Angles.ConvertTo(EulerConvention.OgcInstanceFiles, EulerConvention.GeoPoseStandard);

            Assert.That(standardAngles.ToQuaternion().AngleTo(quaternion.Quaternion), Is.LessThan(1e-4));
            Assert.That(Math.Abs(standardAngles.Yaw), Is.LessThanOrEqualTo(180));
            Assert.That(Math.Abs(standardAngles.Pitch), Is.LessThanOrEqualTo(90));
        }

        [Test]
        public void StrictQuaternionInstancesMatchTheirBasicSiblings()
        {
            for (var i = 0; i < 4; i++)
            {
                var basic = JsonConvert.DeserializeObject<BasicQuaternionSdu>(Load($"GeoPose.Basic.Quaternion.Instance.0{i}.json"));
                var strict = JsonConvert.DeserializeObject<BasicQuaternionSdu>(Load($"GeoPose.Basic.Strict_Quaternion.Instance.0{i}.json"));
                Assert.That(strict, Is.EqualTo(basic), $"pair {i}");
            }
        }

        [Test]
        public void GimbalLockIsHandledUnderEveryOrder()
        {
            foreach (var (a, b, c) in AllOrders)
            {
                var convention = new EulerConvention(a, b, c, AngleUnit.Degrees);
                foreach (var pitch in new[] { 90.0, -90.0 })
                {
                    var q = new YawPitchRollAngles(37, pitch, 0).ToQuaternion(convention);
                    var back = q.ToYawPitchRoll(convention);
                    Assert.That(back.Pitch, Is.EqualTo(pitch).Within(1e-6), $"{convention} pitch");
                    Assert.That(back.Roll, Is.EqualTo(0).Within(1e-6), $"{convention} roll");
                    AssertSameRotation(back.ToQuaternion(convention), q);
                }
            }
        }

        private static string Load(string file) => File.ReadAllText(Path.Combine(FixtureDirectory, file));

        private static void AssertSameRotation(UnitQuaternion actual, UnitQuaternion expected)
        {
            Assert.That(actual.AngleTo(expected), Is.LessThan(1e-7));
        }
    }
}
