using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Angle conversion, great circle distance and the per-resolution metrics.
    /// </summary>
    [TestFixture]
    internal class MetricsTests
    {
        [Test]
        public void DegreesAndRadiansConvertBothWays()
        {
            Assert.That(H3.DegsToRads(180), Is.EqualTo(Math.PI).Within(1e-15));
            Assert.That(H3.RadsToDegs(Math.PI), Is.EqualTo(180).Within(1e-12));
            Assert.That(H3.RadsToDegs(H3.DegsToRads(-122.418307270836)), Is.EqualTo(-122.418307270836).Within(1e-12));
            var point = LatLng.FromDegrees(37.7752702151959, -122.418307270836);
            Assert.That(point.Lat, Is.EqualTo(H3.DegsToRads(37.7752702151959)));
            Assert.That(point.Lng, Is.EqualTo(H3.DegsToRads(-122.418307270836)));
        }

        [Test]
        public void GreatCircleDistanceUsesTheAuthalicSphere()
        {
            var origin = new LatLng(0, 0);
            var quarter = new LatLng(0, Math.PI / 2);
            Assert.That(H3.GreatCircleDistanceRads(origin, origin), Is.EqualTo(0));
            Assert.That(H3.GreatCircleDistanceRads(origin, quarter), Is.EqualTo(Math.PI / 2).Within(1e-12));
            Assert.That(H3.GreatCircleDistanceKm(origin, quarter), Is.EqualTo(Math.PI / 2 * 6371.007180918475).Within(1e-9));
            Assert.That(H3.GreatCircleDistanceM(origin, quarter), Is.EqualTo(H3.GreatCircleDistanceKm(origin, quarter) * 1000).Within(1e-6));
            Assert.That(H3.GreatCircleDistanceRads(quarter, origin), Is.EqualTo(H3.GreatCircleDistanceRads(origin, quarter)));
            var pole = new LatLng(Math.PI / 2, 1.0);
            Assert.That(H3.GreatCircleDistanceRads(origin, pole), Is.EqualTo(Math.PI / 2).Within(1e-12));
        }

        [Test]
        public void NumberOfCellsIsTwoPlusOneHundredTwentyTimesPowersOfSeven()
        {
            Assert.That(H3.GetNumCells(0, out var res0), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(res0, Is.EqualTo(122));
            Assert.That(H3.GetNumCells(1, out var res1), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(res1, Is.EqualTo(842));
            Assert.That(H3.GetNumCells(2, out var res2), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(res2, Is.EqualTo(5882));
            Assert.That(H3.GetNumCells(15, out var res15), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(res15, Is.EqualTo(569707381193162L));
            Assert.That(H3.GetNumCells(-1, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.GetNumCells(16, out _), Is.EqualTo(H3ErrorCode.ResDomain));
        }

        [Test]
        public void EdgeLengthAndAreaTablesAreConsistentAcrossUnits()
        {
            for (var res = 0; res <= 15; res++)
            {
                Assert.That(H3.GetHexagonEdgeLengthAvgKm(res, out var km), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(H3.GetHexagonEdgeLengthAvgM(res, out var m), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(m, Is.EqualTo(km * 1000).Within(1e-3), "res " + res);
                Assert.That(H3.GetHexagonAreaAvgKm2(res, out var km2), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(H3.GetHexagonAreaAvgM2(res, out var m2), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(m2 / km2, Is.EqualTo(1e6).Within(1e-3), "res " + res);
                if (res > 0)
                {
                    H3.GetHexagonEdgeLengthAvgM(res - 1, out var coarser);
                    Assert.That(coarser / m, Is.EqualTo(Math.Sqrt(7)).Within(0.05), "edge length shrinks by root 7 per resolution");
                }
            }

            H3.GetHexagonEdgeLengthAvgM(0, out var res0);
            Assert.That(res0, Is.EqualTo(1281256.011));
            H3.GetHexagonEdgeLengthAvgM(15, out var res15);
            Assert.That(res15, Is.EqualTo(0.584168630));
            H3.GetHexagonAreaAvgKm2(0, out var area0);
            Assert.That(area0, Is.EqualTo(4.357449416078383e+06));
            Assert.That(H3.GetHexagonEdgeLengthAvgM(16, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.GetHexagonEdgeLengthAvgKm(-1, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.GetHexagonAreaAvgKm2(16, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.GetHexagonAreaAvgM2(-1, out _), Is.EqualTo(H3ErrorCode.ResDomain));
        }

        [Test]
        public void IntegerPowerMatchesMathPow()
        {
            for (var exp = 0; exp <= 18; exp++)
            {
                Assert.That(MathExtensions.Ipow(7, exp), Is.EqualTo((long)Math.Pow(7, exp)));
            }

            Assert.That(MathExtensions.Ipow(-3, 3), Is.EqualTo(-27));
            Assert.That(MathExtensions.Ipow(2, 62), Is.EqualTo(1L << 62));
        }
    }
}
