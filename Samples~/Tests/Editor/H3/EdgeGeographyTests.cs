using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// The poles, the antimeridian, longitude zero, icosahedron face centres and out of range input.
    /// </summary>
    [TestFixture]
    internal class EdgeGeographyTests
    {
        [Test]
        public void QuickstartValuesFromTheH3DocumentationRoundTrip()
        {
            // https://h3geo.org/docs/quickstart: latLngToCell(37.7752702151959, -122.418307270836, 9) is 8928308280fffff
            var point = LatLng.FromDegrees(37.7752702151959, -122.418307270836);
            Assert.That(H3.LatLngToCell(point, 9, out var cell), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(H3.H3ToString(cell), Is.EqualTo("8928308280fffff"));
            Assert.That(H3.CellToLatLng(cell, out var center), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(H3.RadsToDegs(center.Lat), Is.EqualTo(37.77670234943567).Within(1e-9));
            Assert.That(H3.RadsToDegs(center.Lng), Is.EqualTo(-122.41845932318311).Within(1e-9));
            Assert.That(H3.CellToBoundary(cell, out var boundary), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(boundary.Count, Is.EqualTo(6));
        }

        [Test]
        public void UberIndexExampleAtTheStatueOfLibertyProducesAValidResolutionTenCellContainingThePoint()
        {
            // examples/index.c in the Uber repository
            var location = LatLng.FromDegrees(40.689167, -74.044444);
            Assert.That(H3.LatLngToCell(location, 10, out var indexed), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(H3.IsValidCell(indexed), Is.True);
            Assert.That(H3.GetResolution(indexed), Is.EqualTo(10));
            Assert.That(H3.CellToBoundary(indexed, out var boundary), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(boundary.Count, Is.InRange(6, 10));
            Assert.That(H3.CellToLatLng(indexed, out var center), Is.EqualTo(H3ErrorCode.Success));
            H3.GetHexagonEdgeLengthAvgM(10, out var edge);
            Assert.That(H3.GreatCircleDistanceM(location, center), Is.LessThan(edge * 1.2), "the centre is within an edge length of the input");
            foreach (var vertex in boundary.ToArray())
            {
                Assert.That(H3.GreatCircleDistanceM(center, vertex), Is.EqualTo(edge).Within(edge * 0.35));
            }
        }

        [TestCase(90.0)]
        [TestCase(-90.0)]
        public void PolesIndexToPentagonFreeValidCellsAtEveryResolution(double latitude)
        {
            for (var res = 0; res <= 15; res++)
            {
                var pole = LatLng.FromDegrees(latitude, 0);
                Assert.That(H3.LatLngToCell(pole, res, out var cell), Is.EqualTo(H3ErrorCode.Success), "res " + res);
                Assert.That(H3.IsValidCell(cell), Is.True);
                Assert.That(H3.LatLngToCell(LatLng.FromDegrees(latitude, 123.4), res, out var sameCell), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(sameCell, Is.EqualTo(cell), "longitude is irrelevant at the pole");
                Assert.That(H3.CellToLatLng(cell, out var center), Is.EqualTo(H3ErrorCode.Success));
                H3.GetHexagonEdgeLengthAvgM(res, out var edge);
                Assert.That(H3.GreatCircleDistanceM(pole, center), Is.LessThan(edge * 1.5));
                Assert.That(H3.CellToBoundary(cell, out var boundary), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(boundary.Count, Is.InRange(5, 10));
            }
        }

        [Test]
        public void AntimeridianLongitudesMapToTheSameCell()
        {
            for (var res = 0; res <= 15; res++)
            {
                var east = LatLng.FromDegrees(12.5, 180);
                var west = LatLng.FromDegrees(12.5, -180);
                var wrapped = LatLng.FromDegrees(12.5, 540);
                Assert.That(H3.LatLngToCell(east, res, out var eastCell), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(H3.LatLngToCell(west, res, out var westCell), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(H3.LatLngToCell(wrapped, res, out var wrappedCell), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(westCell, Is.EqualTo(eastCell), "res " + res);
                Assert.That(wrappedCell, Is.EqualTo(eastCell), "res " + res);
                Assert.That(H3.CellToLatLng(eastCell, out var center), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(Math.Abs(H3.RadsToDegs(center.Lng)), Is.InRange(0.0, 180.0));
            }
        }

        [Test]
        public void LongitudeZeroAndTheEquatorIndexNormally()
        {
            var origin = new LatLng(0, 0);
            for (var res = 0; res <= 15; res++)
            {
                Assert.That(H3.LatLngToCell(origin, res, out var cell), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(H3.IsValidCell(cell), Is.True);
                Assert.That(H3.CellToLatLng(cell, out var center), Is.EqualTo(H3ErrorCode.Success));
                H3.GetHexagonEdgeLengthAvgM(res, out var edge);
                Assert.That(H3.GreatCircleDistanceM(origin, center), Is.LessThan(edge * 1.5));
            }
        }

        [Test]
        public void IcosahedronFaceCentresAreCellCentresAtResolutionZero()
        {
            foreach (var point in Tables.FaceIjkTables.faceCenterPoint)
            {
                var geo = Vec3d.Vec3ToLatLng(point);
                Assert.That(H3.LatLngToCell(geo, 0, out var cell), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(H3.IsValidCell(cell), Is.True);
                Assert.That(H3.IsPentagon(cell), Is.False, "face centres are hexagon centres");
                Assert.That(H3.CellToLatLng(cell, out var center), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(H3.GreatCircleDistanceRads(geo, center), Is.LessThan(1e-12));
                for (var res = 1; res <= 15; res++)
                {
                    Assert.That(H3.LatLngToCell(geo, res, out var fine), Is.EqualTo(H3ErrorCode.Success));
                    Assert.That(H3.CellToParent(fine, 0, out var parent), Is.EqualTo(H3ErrorCode.Success));
                    Assert.That(parent, Is.EqualTo(cell));
                }
            }
        }

        [Test]
        public void EveryPentagonCentreIndexesBackToItself()
        {
            for (var res = 0; res <= 15; res++)
            {
                var pentagons = new ulong[12];
                H3.GetPentagons(res, pentagons);
                foreach (var pentagon in pentagons)
                {
                    Assert.That(H3.CellToLatLng(pentagon, out var center), Is.EqualTo(H3ErrorCode.Success));
                    Assert.That(H3.LatLngToCell(center, res, out var back), Is.EqualTo(H3ErrorCode.Success));
                    Assert.That(back, Is.EqualTo(pentagon), H3.H3ToString(pentagon));
                    Assert.That(H3.CellToBoundary(pentagon, out var boundary), Is.EqualTo(H3ErrorCode.Success));
                    Assert.That(boundary.Count, Is.EqualTo(res % 2 == 0 ? 5 : 10), "class II pentagons have 5 vertices, class III pentagons gain one distortion vertex per edge");
                }
            }
        }

        [Test]
        public void OutOfRangeInputsReturnTheCErrorCodes()
        {
            var anywhere = LatLng.FromDegrees(1, 2);
            Assert.That(H3.LatLngToCell(anywhere, -1, out var cell), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(cell, Is.EqualTo(H3.H3Null));
            Assert.That(H3.LatLngToCell(anywhere, 16, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.LatLngToCell(new LatLng(double.NaN, 0), 1, out _), Is.EqualTo(H3ErrorCode.LatLngDomain));
            Assert.That(H3.LatLngToCell(new LatLng(0, double.PositiveInfinity), 1, out _), Is.EqualTo(H3ErrorCode.LatLngDomain));
            Assert.That(H3.LatLngToCell(new LatLng(double.NegativeInfinity, double.NaN), 1, out _), Is.EqualTo(H3ErrorCode.LatLngDomain));
            var badBaseCell = (0x8928308280fffffUL & ~(127UL << 45)) | (125UL << 45);
            Assert.That(H3.CellToLatLng(badBaseCell, out _), Is.EqualTo(H3ErrorCode.CellInvalid));
            Assert.That(H3.CellToBoundary(badBaseCell, out var boundary), Is.EqualTo(H3ErrorCode.CellInvalid));
            Assert.That(boundary, Is.Null);
        }

        [Test]
        public void LatitudeBeyondNinetyDegreesIsAcceptedByTheEngineLikeTheCLibrary()
        {
            // The C library only rejects non finite input; the sphere maths folds the value. The GeoPose extension layer validates degrees instead.
            Assert.That(H3.LatLngToCell(LatLng.FromDegrees(91, 0), 5, out var cell), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(H3.IsValidCell(cell), Is.True);
        }
    }
}
