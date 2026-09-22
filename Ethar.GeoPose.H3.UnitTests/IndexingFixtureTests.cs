using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Bidirectional value tests against the official Uber centre and boundary fixtures: the centre of every cell must be reproduced, the centre must index back to the same cell, and every boundary vertex must match in order.
    /// </summary>
    [TestFixture]
    internal class IndexingFixtureTests
    {
        /// <summary>
        /// The fixtures print degrees to six decimals (rand and bc files) or ten decimals (ic files).
        /// </summary>
        private const double DegreesTolerance = 1e-6;

        public static IEnumerable<string> CenterFiles => FixtureFiles.CenterFileNames;

        /// <summary>
        /// The ic and bc files hold true cell centres. The rand files hold a random point inside each cell instead, see the fixture readme.
        /// </summary>
        public static IEnumerable<string> TrueCenterFiles => FixtureFiles.CenterFileNames.Where(name => !name.StartsWith("rand"));

        public static IEnumerable<string> RandomPointFiles => FixtureFiles.CenterFileNames.Where(name => name.StartsWith("rand"));

        public static IEnumerable<string> BoundaryFiles => FixtureFiles.BoundaryFileNames;

        [TestCaseSource(nameof(RandomPointFiles))]
        public void CellCentreIsWithinACellOfEveryFixtureRandomPoint(string name)
        {
            var failures = new List<string>();
            var count = 0;
            foreach (var line in FixtureFiles.ReadCenters(name))
            {
                count++;
                var index = H3Index.Parse(line.Hex);
                H3.GetHexagonEdgeLengthAvgM(index.Resolution, out var edge);
                Assert.That(H3.CellToLatLng(index.Value, out var center), Is.EqualTo(H3ErrorCode.Success));
                var distance = H3.GreatCircleDistanceM(center, LatLng.FromDegrees(line.Latitude, line.Longitude));
                if (distance > edge * 1.3)
                {
                    failures.Add(line.Hex + " point is " + distance + " m from the centre, edge length " + edge);
                }
            }

            Assert.That(count, Is.EqualTo(5000));
            Assert.That(failures, Is.Empty, name + ": " + failures.Count + " of " + count + " points are outside their cell. First: " + failures.FirstOrDefault());
        }

        [TestCaseSource(nameof(TrueCenterFiles))]
        public void CellToLatLngReproducesEveryFixtureCentre(string name)
        {
            var failures = new List<string>();
            var count = 0;
            foreach (var line in FixtureFiles.ReadCenters(name))
            {
                count++;
                var value = H3Index.Parse(line.Hex).Value;
                var code = H3.CellToLatLng(value, out var center);
                if (code != H3ErrorCode.Success)
                {
                    failures.Add(line.Hex + " returned " + code);
                    continue;
                }

                var latitude = H3.RadsToDegs(center.Lat);
                var longitude = H3.RadsToDegs(center.Lng);
                if (Math.Abs(latitude - line.Latitude) > DegreesTolerance || FixtureFiles.LongitudeDifference(longitude, line.Longitude) > DegreesTolerance)
                {
                    failures.Add(line.Hex + " expected " + line.Latitude + " " + line.Longitude + " got " + latitude + " " + longitude);
                }
            }

            Assert.That(count, Is.GreaterThan(0));
            Assert.That(failures, Is.Empty, name + ": " + failures.Count + " of " + count + " centres differ. First: " + failures.FirstOrDefault());
        }

        [TestCaseSource(nameof(CenterFiles))]
        public void LatLngToCellOnEveryFixtureCentreReturnsTheCell(string name)
        {
            var failures = new List<string>();
            var count = 0;
            foreach (var line in FixtureFiles.ReadCenters(name))
            {
                count++;
                var expected = H3Index.Parse(line.Hex);
                var code = H3.LatLngToCell(LatLng.FromDegrees(line.Latitude, line.Longitude), expected.Resolution, out var actual);
                if (code != H3ErrorCode.Success)
                {
                    failures.Add(line.Hex + " returned " + code);
                }
                else if (actual != expected.Value)
                {
                    failures.Add(line.Hex + " got " + H3.H3ToString(actual));
                }
            }

            Assert.That(count, Is.GreaterThan(0));
            Assert.That(failures, Is.Empty, name + ": " + failures.Count + " of " + count + " indexes differ. First: " + failures.FirstOrDefault());
        }

        [TestCaseSource(nameof(CenterFiles))]
        public void CentreIndexCentreRoundTripIsStableToANanoradian(string name)
        {
            var failures = new List<string>();
            foreach (var line in FixtureFiles.ReadCenters(name))
            {
                var resolution = H3Index.Parse(line.Hex).Resolution;
                var start = LatLng.FromDegrees(line.Latitude, line.Longitude);
                H3.LatLngToCell(start, resolution, out var cell);
                H3.CellToLatLng(cell, out var center);
                H3.LatLngToCell(center, resolution, out var again);
                H3.CellToLatLng(again, out var centerAgain);
                if (again != cell || Math.Abs(center.Lat - centerAgain.Lat) > 1e-9 || Math.Abs(center.Lng - centerAgain.Lng) > 1e-9)
                {
                    failures.Add(line.Hex);
                }
            }

            Assert.That(failures, Is.Empty, name + ": " + failures.Count + " cells drift. First: " + failures.FirstOrDefault());
        }

        [TestCaseSource(nameof(BoundaryFiles))]
        public void CellToBoundaryReproducesEveryFixtureBoundaryInOrder(string name)
        {
            var failures = new List<string>();
            var blocks = FixtureFiles.ReadBoundaries(name);
            Assert.That(blocks, Is.Not.Empty);
            foreach (var block in blocks)
            {
                var value = H3Index.Parse(block.Hex).Value;
                Assert.That(H3.CellToBoundary(value, out var boundary), Is.EqualTo(H3ErrorCode.Success), block.Hex);
                if (boundary.Count != block.Vertices.Count)
                {
                    failures.Add(block.Hex + " expected " + block.Vertices.Count + " vertices got " + boundary.Count);
                    continue;
                }

                for (var i = 0; i < boundary.Count; i++)
                {
                    var latitude = H3.RadsToDegs(boundary[i].Lat);
                    var longitude = H3.RadsToDegs(boundary[i].Lng);
                    if (Math.Abs(latitude - block.Vertices[i].Latitude) > DegreesTolerance || FixtureFiles.LongitudeDifference(longitude, block.Vertices[i].Longitude) > DegreesTolerance)
                    {
                        failures.Add(block.Hex + " vertex " + i + " expected " + block.Vertices[i].Latitude + " " + block.Vertices[i].Longitude + " got " + latitude + " " + longitude);
                        break;
                    }
                }
            }

            Assert.That(failures, Is.Empty, name + ": " + failures.Count + " of " + blocks.Count + " boundaries differ. First: " + failures.FirstOrDefault());
        }

        [Test]
        public void BoundaryFixturesHaveTheExpectedVertexCounts()
        {
            var blocks = FixtureFiles.ReadBoundaries("res01cells.txt");
            Assert.That(blocks, Has.Count.EqualTo(842));
            var pentagons = blocks.Where(block => H3Index.Parse(block.Hex).IsPentagon).ToList();
            Assert.That(pentagons, Has.Count.EqualTo(12));

            // resolution 1 is class III, so every pentagon edge crosses an icosahedron edge and the boundary has 10 vertices
            Assert.That(pentagons.Select(block => block.Vertices.Count), Is.All.EqualTo(10));
            Assert.That(blocks.Select(block => block.Vertices.Count), Is.All.InRange(6, 10));

            // resolution 0 is class II, so pentagons have exactly 5 vertices
            var res0 = FixtureFiles.ReadBoundaries("res00cells.txt");
            Assert.That(res0.Where(block => H3Index.Parse(block.Hex).IsPentagon).Select(block => block.Vertices.Count), Is.All.EqualTo(5));
        }
    }
}
