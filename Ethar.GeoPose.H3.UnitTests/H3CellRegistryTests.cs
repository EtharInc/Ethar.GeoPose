using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.StructuralDataUnits;
using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Searching registered GeoPoses by H3 cell. The registry answers by the index hierarchy, so the expected sets are computed the same way by brute force.
    /// </summary>
    [TestFixture]
    internal class H3CellRegistryTests
    {
        private static List<FixtureFiles.CenterLine> points;

        private static H3CellRegistry<string> registry;

        [OneTimeSetUp]
        public static void BuildRegistryFromTheRandomFixturePoints()
        {
            points = FixtureFiles.ReadCenters("rand09centers.txt");
            registry = new H3CellRegistry<string>();
            foreach (var line in points)
            {
                registry.Add(line.Hex, Position(line));
            }
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(9)]
        [TestCase(12)]
        [TestCase(15)]
        public void FindWithinReturnsExactlyTheHierarchyDescendants(int resolution)
        {
            Assert.That(registry.Count, Is.EqualTo(5000));
            var finest = points.ToDictionary(line => line.Hex, line => Position(line).ToH3Cell(15));
            var queried = 0;
            foreach (var line in points.Where((line, i) => i % 97 == 0))
            {
                var cell = finest[line.Hex].Parent(resolution);
                var expected = points.Where(candidate => finest[candidate.Hex].IsWithin(cell)).Select(candidate => candidate.Hex).OrderBy(hex => hex, StringComparer.Ordinal).ToList();
                var actual = registry.FindWithin(cell).OrderBy(hex => hex, StringComparer.Ordinal).ToList();
                Assert.That(actual, Is.EqualTo(expected), cell.ToString());
                Assert.That(actual, Does.Contain(line.Hex));
                Assert.That(registry.CountWithin(cell), Is.EqualTo(expected.Count));
                queried++;
            }

            Assert.That(queried, Is.GreaterThan(40));
        }

        [Test]
        public void FindWithinByPositionAgreesWithFindWithinByCell()
        {
            var line = points[123];
            var position = Position(line);
            for (var resolution = 0; resolution <= 15; resolution += 3)
            {
                var cell = position.ToH3Cell(15).Parent(resolution);
                Assert.That(registry.FindWithin(position, resolution), Is.EqualTo(registry.FindWithin(cell)));
            }
        }

        [Test]
        public void ResultsAreAlmostAlwaysTheCellsThatDirectProjectionWouldGive()
        {
            // The registry uses the hierarchy, direct projection uses the geometry; H3 parents only approximately contain their children.
            var line = points[321];
            var cell = Position(line).ToH3Cell(6);
            var direct = points.Where(candidate => Position(candidate).ToH3Cell(6) == cell).Select(candidate => candidate.Hex).ToList();
            var viaRegistry = registry.FindWithin(cell);
            Assert.That(viaRegistry, Is.Not.Empty);
            Assert.That(viaRegistry.Intersect(direct).Count(), Is.GreaterThanOrEqualTo(Math.Max(direct.Count, viaRegistry.Count) * 3 / 4));
        }

        [Test]
        public void ItemsAddedByCoarseCellAreFoundByThatCellAndItsAncestors()
        {
            var local = new H3CellRegistry<string>();
            var paris = new TangentPointPosition(48.8566, 2.3522, 35.5);
            var cell5 = paris.ToH3Cell(5);
            local.Add("coarse", cell5);
            local.Add("fine", paris);
            Assert.That(local.Count, Is.EqualTo(2));
            Assert.That(local.Items, Is.EquivalentTo(new[] { "coarse", "fine" }));
            Assert.That(local.FindWithin(cell5), Is.EquivalentTo(new[] { "coarse", "fine" }));
            Assert.That(local.FindWithin(cell5.Parent(1)), Is.EquivalentTo(new[] { "coarse", "fine" }));
            Assert.That(local.FindWithin(cell5.CenterChild(9)), Is.EqualTo(new[] { "coarse" }), "a coarse registration lives at the centre");
            Assert.That(local.FindWithin(paris.ToH3Cell(15).Parent(9)), Is.EqualTo(new[] { "fine" }));
            Assert.That(local.FindWithin(new TangentPointPosition(37.77, -122.41, 0).ToH3Cell(0)), Is.Empty);
        }

        [Test]
        public void RemoveClearAndInterleavedQueriesKeepTheOrderCorrect()
        {
            var local = new H3CellRegistry<BasicYawPitchRollSdu>();
            var angles = new YawPitchRollAngles(0, 0, 0);
            var london = new BasicYawPitchRollSdu(angles, new TangentPointPosition(51.5074, -0.1278, 0));
            var paris = new BasicYawPitchRollSdu(angles, new TangentPointPosition(48.8566, 2.3522, 0));
            var tokyo = new BasicYawPitchRollSdu(angles, new TangentPointPosition(35.6762, 139.6503, 0));
            // queries by position use the same hierarchy rule as the keys, so a registered pose always finds itself
            local.Add(london, london.Position);
            Assert.That(local.FindWithin(london.Position, 3), Is.EqualTo(new[] { london }));
            local.Add(paris, paris.Position);
            local.Add(tokyo, tokyo.Position);
            Assert.That(local.Count, Is.EqualTo(3));
            Assert.That(local.FindWithin(paris.Position, 7), Is.EqualTo(new[] { paris }));
            Assert.That(local.FindWithin(tokyo.Position, 0), Is.EqualTo(new[] { tokyo }));
            Assert.That(local.FindWithin(london.Position, 1).Count + local.FindWithin(paris.Position, 1).Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(local.FindWithin(paris.ToH3Cell(15)), Is.EqualTo(new[] { paris }));
            Assert.That(local.Remove(paris), Is.True);
            Assert.That(local.Remove(paris), Is.False);
            Assert.That(local.FindWithin(paris.Position, 7), Is.Empty);
            Assert.That(local.Count, Is.EqualTo(2));
            local.Clear();
            Assert.That(local.Count, Is.EqualTo(0));
            Assert.That(local.FindWithin(tokyo.Position, 0), Is.Empty);
            Assert.That(local.CountWithin(tokyo.ToH3Cell(0)), Is.EqualTo(0));
        }

        [Test]
        public void PentagonCellsAreSearchable()
        {
            var local = new H3CellRegistry<int>();
            var pentagon = new H3Index(H3.SetH3Index(0, 4, Direction.Center));
            var descendants = pentagon.Children(3);
            for (var i = 0; i < descendants.Length; i++)
            {
                local.Add(i, descendants[i].ToTangentPointPosition(0));
            }

            Assert.That(local.CountWithin(pentagon), Is.EqualTo(descendants.Length));
            Assert.That(local.FindWithin(pentagon.CenterChild(3)), Is.EqualTo(new[] { Array.IndexOf(descendants, pentagon.CenterChild(3)) }));
            Assert.That(local.FindWithin(new H3Index(H3.SetH3Index(0, 5, Direction.Center))), Is.Empty);
        }

        private static TangentPointPosition Position(FixtureFiles.CenterLine line)
        {
            return new TangentPointPosition(line.Latitude, line.Longitude > 180 ? line.Longitude - 360 : line.Longitude, 0);
        }
    }
}
