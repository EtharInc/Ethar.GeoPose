using System.Globalization;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.StructuralDataUnits;
using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// The GeoPose facing extension methods: positions and poses to cells and back, containment, boundaries and distances, with in place accuracy checks against the engine and the fixtures.
    /// </summary>
    [TestFixture]
    internal class ExtensionTests
    {
        private static readonly TangentPointPosition Paris = new TangentPointPosition(48.8566, 2.3522, 35.5);
        private static readonly TangentPointPosition SanFrancisco = new TangentPointPosition(37.7752702151959, -122.418307270836, 0);

        [Test]
        public void PositionToCellIgnoresHeightAndMatchesTheEngine()
        {
            var cell = Paris.ToH3Cell(9);
            Assert.That(cell.Resolution, Is.EqualTo(9));
            Assert.That(new TangentPointPosition(Paris.Latitude, Paris.Longitude, -400).ToH3Cell(9), Is.EqualTo(cell));
            Assert.That(new TangentPointPosition(Paris.Latitude, Paris.Longitude, 8848).ToH3Cell(9), Is.EqualTo(cell));
            H3.LatLngToCell(LatLng.FromDegrees(Paris.Latitude, Paris.Longitude), 9, out var engine);
            Assert.That(cell.Value, Is.EqualTo(engine));
            Assert.That(SanFrancisco.ToH3Cell(9).ToString(), Is.EqualTo("8928308280fffff"));
        }

        [Test]
        public void CellToPositionSetsTheHeightAndReturnsTheCentre()
        {
            for (var resolution = 0; resolution <= 15; resolution++)
            {
                var cell = Paris.ToH3Cell(resolution);
                var center = cell.ToTangentPointPosition(35.5);
                Assert.That(center.HeightInMeters, Is.EqualTo(35.5));
                H3.CellToLatLng(cell.Value, out var engine);
                Assert.That(center.Latitude, Is.EqualTo(H3.RadsToDegs(engine.Lat)).Within(1e-12));
                Assert.That(center.Longitude, Is.EqualTo(H3.RadsToDegs(engine.Lng)).Within(1e-12));
                Assert.That(center.Validate().IsValid, Is.True);

                // GeoPose to H3 to GeoPose: the centre indexes back to the cell and lies within the cell of the input
                Assert.That(center.ToH3Cell(resolution), Is.EqualTo(cell), "res " + resolution);
                H3.GetHexagonEdgeLengthAvgM(resolution, out var edge);
                Assert.That(Paris.DistanceTo(center), Is.LessThan(edge * 1.3), "res " + resolution);
            }
        }

        [TestCase("res00ic.txt")]
        [TestCase("res01ic.txt")]
        [TestCase("res02ic.txt")]
        [TestCase("bc14r12centers.txt")]
        public void FixtureCentresRoundTripThroughTheGeoPoseTypes(string name)
        {
            foreach (var line in FixtureFiles.ReadCenters(name))
            {
                var expected = H3Index.Parse(line.Hex);
                var position = new TangentPointPosition(line.Latitude, line.Longitude, 12.5);
                var cell = position.ToH3Cell(expected.Resolution);
                Assert.That(cell, Is.EqualTo(expected), line.Hex);
                var back = cell.ToTangentPointPosition(12.5);
                Assert.That(back.Latitude, Is.EqualTo(line.Latitude).Within(1e-6), line.Hex);
                Assert.That(FixtureFiles.LongitudeDifference(back.Longitude, line.Longitude), Is.LessThan(1e-6), line.Hex);
                Assert.That(position.IsInH3Cell(cell), Is.True, line.Hex);
            }
        }

        [Test]
        public void IsInH3CellUsesDirectProjectionAndIsWithinUsesTheHierarchy()
        {
            var cell = Paris.ToH3Cell(11);
            Assert.That(Paris.IsInH3Cell(cell), Is.True);
            for (var resolution = 0; resolution <= 15; resolution++)
            {
                Assert.That(Paris.IsInH3Cell(Paris.ToH3Cell(resolution)), Is.True, "direct projection at res " + resolution);
            }

            foreach (var ancestor in cell.Ancestors())
            {
                Assert.That(cell.IsWithin(ancestor), Is.True, ancestor.ToString());
            }

            Assert.That(Paris.IsInH3Cell(SanFrancisco.ToH3Cell(0)), Is.False);
            Assert.That(Paris.IsInH3Cell(SanFrancisco.ToH3Cell(9)), Is.False);
            Assert.That(SanFrancisco.IsInH3Cell(H3Index.Parse("8928308280fffff")), Is.True);
        }

        [Test]
        public void CellsAtEveryResolutionAreTheDirectProjections()
        {
            var cells = Paris.ToH3Cells();
            Assert.That(cells, Has.Length.EqualTo(16));
            for (var resolution = 0; resolution <= 15; resolution++)
            {
                Assert.That(cells[resolution].Resolution, Is.EqualTo(resolution));
                Assert.That(cells[resolution], Is.EqualTo(Paris.ToH3Cell(resolution)));
                Assert.That(Paris.IsInH3Cell(cells[resolution]), Is.True);
            }

            Assert.Throws<H3Exception>(() => new TangentPointPosition(95, 0, 0).ToH3Cells());
        }

        [Test]
        public void HierarchyDerivedCellsDisagreeWithDirectProjectionOnlyNearCellEdges()
        {
            // H3 parents only approximately contain their children, so a point in the distortion band along a cell edge can project
            // directly into a neighbouring cell at a coarse resolution while its resolution 15 cell descends from this one.
            // This characterises the rate for the random fixture points and prints it per resolution.
            var points = FixtureFiles.ReadCenters("rand09centers.txt");
            var disagreements = new int[15];
            foreach (var line in points)
            {
                var position = new TangentPointPosition(line.Latitude, line.Longitude > 180 ? line.Longitude - 360 : line.Longitude, 0);
                var direct = position.ToH3Cells();
                var derived = direct[15].Ancestors();
                for (var resolution = 0; resolution < 15; resolution++)
                {
                    if (derived[resolution] != direct[resolution])
                    {
                        disagreements[resolution]++;
                    }
                }
            }

            for (var resolution = 0; resolution < 15; resolution++)
            {
                var percent = 100.0 * disagreements[resolution] / points.Count;
                TestContext.Out.WriteLine("res " + resolution + ": " + disagreements[resolution] + " of " + points.Count + " points differ (" + percent.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + " percent)");
                // measured at 6 to 7 percent for every resolution: the union of a cell's descendants is a jagged shape whose
                // symmetric difference with the hexagon is a fixed fraction of its area, whatever the resolution gap
                Assert.That(percent, Is.LessThan(12), "res " + resolution);
                Assert.That(percent, Is.GreaterThan(2), "res " + resolution);
            }
        }

        [Test]
        public void BoundaryCarriesTheHeightAndMatchesTheEngine()
        {
            var cell = Paris.ToH3Cell(7);
            var boundary = cell.ToCellBoundary(35.5);
            Assert.That(boundary.Count, Is.InRange(6, 10));
            Assert.That(boundary.HeightInMeters, Is.EqualTo(35.5));
            var engine = cell.Boundary();
            Assert.That(boundary.Count, Is.EqualTo(engine.Count));
            for (var i = 0; i < boundary.Count; i++)
            {
                Assert.That(boundary[i].HeightInMeters, Is.EqualTo(35.5));
                Assert.That(boundary[i].Latitude, Is.EqualTo(H3.RadsToDegs(engine[i].Lat)).Within(1e-12));
                Assert.That(boundary[i].Longitude, Is.EqualTo(H3.RadsToDegs(engine[i].Lng)).Within(1e-12));
                Assert.That(boundary[i].Validate().IsValid, Is.True);
            }

            Assert.That(boundary.ToArray(), Has.Length.EqualTo(boundary.Count));
            var pentagon = new H3Index(H3.SetH3Index(4, 14, Direction.Center));
            Assert.That(pentagon.ToCellBoundary(0).Count, Is.EqualTo(5));
        }

        [Test]
        public void PoseBuildersPlaceThePoseAtTheCellCentre()
        {
            var cell = Paris.ToH3Cell(10);
            var angles = new YawPitchRollAngles(-12.5, 0.25, 90);
            var quaternion = new UnitQuaternion(0.20056154657066608, -0.08111602541464237, 0.36606032744426537, -0.9050939692261301);

            var ypr = cell.ToBasicYawPitchRollSdu(35.5, angles);
            Assert.That(ypr.Angles, Is.EqualTo(angles));
            Assert.That(ypr.Position, Is.EqualTo(cell.ToTangentPointPosition(35.5)));
            Assert.That(ypr.ToH3Cell(10), Is.EqualTo(cell));
            Assert.That(ypr.IsInH3Cell(cell), Is.True);
            Assert.That(ypr.IsInH3Cell(cell.Parent(3)), Is.True);
            Assert.That(ypr.IsInH3Cell(SanFrancisco.ToH3Cell(3)), Is.False);

            var quat = cell.ToBasicQuaternionSdu(35.5, quaternion);
            Assert.That(quat.Quaternion, Is.EqualTo(quaternion));
            Assert.That(quat.Position, Is.EqualTo(cell.ToTangentPointPosition(35.5)));
            Assert.That(quat.ToH3Cell(10), Is.EqualTo(cell));
            Assert.That(quat.IsInH3Cell(cell), Is.True);
            Assert.That(quat.IsInH3Cell(SanFrancisco.ToH3Cell(3)), Is.False);
        }

        [Test]
        public void DistanceBetweenCellsUsesTheGeoPoseGeodesy()
        {
            var paris = Paris.ToH3Cell(9);
            var sanFrancisco = SanFrancisco.ToH3Cell(9);
            Assert.That(paris.DistanceTo(paris), Is.EqualTo(0));
            Assert.That(paris.DistanceTo(sanFrancisco), Is.EqualTo(sanFrancisco.DistanceTo(paris)));
            var engine = H3.GreatCircleDistanceM(paris.Center, sanFrancisco.Center);
            Assert.That(paris.DistanceTo(sanFrancisco), Is.EqualTo(engine).Within(engine * 0.001), "same great circle to within the difference in Earth radius");
            Assert.That(paris.DistanceTo(sanFrancisco), Is.EqualTo(8_953_000).Within(50_000));
        }

        [TestCase(91, 0, H3ErrorCode.LatLngDomain)]
        [TestCase(-91, 0, H3ErrorCode.LatLngDomain)]
        [TestCase(0, 181, H3ErrorCode.LatLngDomain)]
        [TestCase(0, -181, H3ErrorCode.LatLngDomain)]
        [TestCase(double.NaN, 0, H3ErrorCode.LatLngDomain)]
        public void OutOfRangePositionsThrowWithTheH3ErrorCode(double latitude, double longitude, H3ErrorCode expected)
        {
            var exception = Assert.Throws<H3Exception>(() => new TangentPointPosition(latitude, longitude, 0).ToH3Cell(5));
            Assert.That(exception.ErrorCode, Is.EqualTo(expected));
        }

        [TestCase(-1)]
        [TestCase(16)]
        public void OutOfRangeResolutionsThrowWithTheH3ErrorCode(int resolution)
        {
            var exception = Assert.Throws<H3Exception>(() => Paris.ToH3Cell(resolution));
            Assert.That(exception.ErrorCode, Is.EqualTo(H3ErrorCode.ResDomain));
        }

        [TestCaseSource(typeof(CultureInvarianceTests), nameof(CultureInvarianceTests.Cultures))]
        public void ConversionsDoNotDependOnTheCulture(string cultureName)
        {
            var original = CultureInfo.CurrentCulture;
            try
            {
                var culture = cultureName.Length == 0 ? CultureInfo.InvariantCulture : new CultureInfo(cultureName);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                var cell = Paris.ToH3Cell(12);
                Assert.That(cell.ToString(), Is.EqualTo(H3.H3ToString(cell.Value)));
                var center = cell.ToTangentPointPosition(35.5);
                Assert.That(center.ToString(), Does.Contain("HeightInMeters:35.5"));
                Assert.That(H3Index.Parse(cell.ToString().ToUpperInvariant()), Is.EqualTo(cell));
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
                CultureInfo.CurrentUICulture = original;
            }
        }
    }
}
