using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Every index in the official Uber fixtures must be accepted by the ported validation and survive a hex round trip.
    /// </summary>
    [TestFixture]
    internal class FixtureValidityTests
    {
        public static IEnumerable<string> Names => FixtureFiles.Names;

        [Test]
        public void ThirtyFiveFixtureFilesAreCopied()
        {
            Assert.That(FixtureFiles.Names.Count(), Is.EqualTo(35));
        }

        [TestCaseSource(nameof(Names))]
        public void EveryIndexIsAValidCellAndRoundTrips(string name)
        {
            var indexes = FixtureFiles.ReadIndexes(name);
            Assert.That(indexes, Is.Not.Empty, name);
            foreach (var hex in indexes)
            {
                Assert.That(H3.StringToH3(hex, out var value), Is.EqualTo(H3ErrorCode.Success), hex);
                Assert.That(H3.IsValidCell(value), Is.True, hex);
                Assert.That(H3.H3ToString(value), Is.EqualTo(hex));
                Assert.That(H3Index.TryParse(hex, out var index), Is.True, hex);
                Assert.That(index.Value, Is.EqualTo(value));
                Assert.That(index.ToString(), Is.EqualTo(hex));
            }
        }

        [Test]
        public void Res00CellsHas122ResolutionZeroCellsInBaseCellOrder()
        {
            var indexes = FixtureFiles.ReadIndexes("res00cells.txt");
            Assert.That(indexes, Has.Count.EqualTo(122));
            for (var ordinal = 0; ordinal < indexes.Count; ordinal++)
            {
                var index = H3Index.Parse(indexes[ordinal]);
                Assert.That(index.Resolution, Is.EqualTo(0), indexes[ordinal]);
                Assert.That(index.BaseCell, Is.EqualTo(ordinal), indexes[ordinal]);
            }
        }

        [TestCase("res00cells.txt", 122, 0)]
        [TestCase("res01cells.txt", 842, 1)]
        [TestCase("res02cells.txt", 5882, 2)]
        [TestCase("res00ic.txt", 122, 0)]
        [TestCase("res01ic.txt", 842, 1)]
        [TestCase("res02ic.txt", 5882, 2)]
        [TestCase("res03ic.txt", 41162, 3)]
        [TestCase("res04ic.txt", 288122, 4)]
        [TestCase("rand05centers.txt", 5000, 5)]
        [TestCase("rand10centers.txt", 5000, 10)]
        [TestCase("rand15centers.txt", 5000, 15)]
        public void FixtureHasExpectedCountAndResolution(string name, int count, int resolution)
        {
            var indexes = FixtureFiles.ReadIndexes(name);
            Assert.That(indexes, Has.Count.EqualTo(count));
            Assert.That(indexes.Select(hex => H3Index.Parse(hex).Resolution), Is.All.EqualTo(resolution));
        }

        [Test]
        public void WholeResolutionFixturesContainTwelvePentagons()
        {
            foreach (var name in new[] { "res00cells.txt", "res01cells.txt", "res02cells.txt" })
            {
                var pentagons = FixtureFiles.ReadIndexes(name).Select(H3Index.Parse).Count(index => index.IsPentagon);
                Assert.That(pentagons, Is.EqualTo(12), name);
            }
        }

        [TestCase("bc05r08centers.txt", 5, 8, false)]
        [TestCase("bc05r15centers.txt", 5, 15, false)]
        [TestCase("bc14r08centers.txt", 14, 8, true)]
        [TestCase("bc14r15centers.txt", 14, 15, true)]
        public void BaseCellFixturesSitUnderTheirBaseCell(string name, int baseCell, int resolution, bool containsPentagon)
        {
            var indexes = FixtureFiles.ReadIndexes(name).Select(H3Index.Parse).ToList();
            Assert.That(indexes, Is.Not.Empty);
            Assert.That(indexes.Select(index => index.BaseCell), Is.All.EqualTo(baseCell));
            Assert.That(indexes.Select(index => index.Resolution), Is.All.EqualTo(resolution));
            Assert.That(indexes.Count(index => index.IsPentagon), Is.EqualTo(containsPentagon ? 1 : 0));
        }
    }
}
