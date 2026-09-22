using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Cell boundaries from Uber's testH3Api.c, including the class III edge vertex regression (issue 45) and the longitude constraint regression (issue 212).
    /// </summary>
    [TestFixture]
    internal class KnownBoundaryTests
    {
        private const double Tolerance = 1e-9;

        [Test]
        public void ClassIIIEdgeVertexCellsHaveSevenVertices()
        {
            var hexes = new[]
            {
                0x894cc5349b7ffffUL, 0x894cc534d97ffffUL, 0x894cc53682bffffUL,
                0x894cc536b17ffffUL, 0x894cc53688bffffUL, 0x894cead92cbffffUL,
                0x894cc536537ffffUL, 0x894cc5acbabffffUL, 0x894cc536597ffffUL,
            };
            foreach (var hex in hexes)
            {
                Assert.That(H3.CellToBoundary(hex, out var boundary), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(boundary.Count, Is.EqualTo(7), H3.H3ToString(hex));
            }
        }

        [Test]
        public void ClassIIIEdgeVertexBoundaryIsExact()
        {
            var expected = new[]
            {
                new[] { 18.043333154, -66.27836523500002 },
                new[] { 18.042238363, -66.27929062800001 },
                new[] { 18.040818259, -66.27854193899998 },
                new[] { 18.040492975, -66.27686786700002 },
                new[] { 18.041040385, -66.27640518300001 },
                new[] { 18.041757122, -66.27596711500001 },
                new[] { 18.043007860, -66.27669118199998 },
            };
            AssertBoundary("894cc536537ffff", expected);
        }

        [Test]
        public void LongitudeConstraintBoundaryIsExact()
        {
            var expected = new[]
            {
                new[] { -52.0130533678236091, -34.6232931343713091 },
                new[] { -52.0041156384652012, -34.6096733160584549 },
                new[] { -51.9929610229502472, -34.6165157145896387 },
                new[] { -51.9907410568096608, -34.6369680004259877 },
                new[] { -51.9996738734672377, -34.6505896528323660 },
                new[] { -52.0108315681413629, -34.6437571897165668 },
            };
            AssertBoundary("87dc6d364ffffff", expected);
        }

        private static void AssertBoundary(string hex, double[][] expectedDegrees)
        {
            var cell = H3Index.Parse(hex).Value;
            Assert.That(H3.CellToBoundary(cell, out var boundary), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(boundary.Count, Is.EqualTo(expectedDegrees.Length));
            for (var i = 0; i < boundary.Count; i++)
            {
                Assert.That(H3.RadsToDegs(boundary[i].Lat), Is.EqualTo(expectedDegrees[i][0]).Within(Tolerance), hex + " vertex " + i + " latitude");
                Assert.That(H3.RadsToDegs(boundary[i].Lng), Is.EqualTo(expectedDegrees[i][1]).Within(Tolerance), hex + " vertex " + i + " longitude");
            }
        }
    }
}
