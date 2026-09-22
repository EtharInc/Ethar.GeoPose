using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Parent, children and center child against the Uber test values and the whole-resolution fixtures.
    /// </summary>
    [TestFixture]
    internal class HierarchyTests
    {
        /// <summary>
        /// Children of 0x88283080ddfffff at resolution 9, from Uber's testCellToChildren.c.
        /// </summary>
        private static readonly ulong[] ChildrenRes9 =
        {
            0x89283080dc3ffff, 0x89283080dc7ffff, 0x89283080dcbffff, 0x89283080dcfffff,
            0x89283080dd3ffff, 0x89283080dd7ffff, 0x89283080ddbffff,
        };

        /// <summary>
        /// Children of 0x88283080ddfffff at resolution 10, from Uber's testCellToChildren.c (order as the C library emits it).
        /// </summary>
        private static readonly ulong[] ChildrenRes10 =
        {
            0x8a283080dd27fff, 0x8a283080dd37fff, 0x8a283080dc47fff,
            0x8a283080dcdffff, 0x8a283080dc5ffff, 0x8a283080dc27fff,
            0x8a283080ddb7fff, 0x8a283080dc07fff, 0x8a283080dd8ffff,
            0x8a283080dd5ffff, 0x8a283080dc4ffff, 0x8a283080dd47fff,
            0x8a283080dce7fff, 0x8a283080dd1ffff, 0x8a283080dceffff,
            0x8a283080dc6ffff, 0x8a283080dc87fff, 0x8a283080dcaffff,
            0x8a283080dd2ffff, 0x8a283080dcd7fff, 0x8a283080dd9ffff,
            0x8a283080dd6ffff, 0x8a283080dcc7fff, 0x8a283080dca7fff,
            0x8a283080dccffff, 0x8a283080dd77fff, 0x8a283080dc97fff,
            0x8a283080dd4ffff, 0x8a283080dd97fff, 0x8a283080dc37fff,
            0x8a283080dc8ffff, 0x8a283080dcb7fff, 0x8a283080dcf7fff,
            0x8a283080dd87fff, 0x8a283080dda7fff, 0x8a283080dc9ffff,
            0x8a283080dc77fff, 0x8a283080dc67fff, 0x8a283080dc57fff,
            0x8a283080ddaffff, 0x8a283080dd17fff, 0x8a283080dc17fff,
            0x8a283080dd57fff, 0x8a283080dc0ffff, 0x8a283080dd07fff,
            0x8a283080dc1ffff, 0x8a283080dd0ffff, 0x8a283080dc2ffff,
            0x8a283080dd67fff,
        };

        /// <summary>
        /// Children of the resolution 1 pentagon 0x81083ffffffffff at resolution 3, from Uber's testCellToChildren.c.
        /// </summary>
        private static readonly ulong[] PentagonChildrenRes3 =
        {
            0x830800fffffffff, 0x830802fffffffff, 0x830803fffffffff,
            0x830804fffffffff, 0x830805fffffffff, 0x830806fffffffff,
            0x830810fffffffff, 0x830811fffffffff, 0x830812fffffffff,
            0x830813fffffffff, 0x830814fffffffff, 0x830815fffffffff,
            0x830816fffffffff, 0x830818fffffffff, 0x830819fffffffff,
            0x83081afffffffff, 0x83081bfffffffff, 0x83081cfffffffff,
            0x83081dfffffffff, 0x83081efffffffff, 0x830820fffffffff,
            0x830821fffffffff, 0x830822fffffffff, 0x830823fffffffff,
            0x830824fffffffff, 0x830825fffffffff, 0x830826fffffffff,
            0x830828fffffffff, 0x830829fffffffff, 0x83082afffffffff,
            0x83082bfffffffff, 0x83082cfffffffff, 0x83082dfffffffff,
            0x83082efffffffff, 0x830830fffffffff, 0x830831fffffffff,
            0x830832fffffffff, 0x830833fffffffff, 0x830834fffffffff,
            0x830835fffffffff, 0x830836fffffffff,
        };

        [Test]
        public void ChildrenMatchTheUberTestValues()
        {
            AssertChildren(0x88283080ddfffffUL, 9, ChildrenRes9);
            AssertChildren(0x88283080ddfffffUL, 10, ChildrenRes10);
            AssertChildren(0x81083ffffffffffUL, 3, PentagonChildrenRes3);
        }

        [Test]
        public void ChildrenSizeFollowsThePowersOfSevenAndThePentagonFormula()
        {
            var hexagon = 0x88283080ddfffffUL;
            var pentagon = 0x81083ffffffffffUL;
            Assert.That(H3.IsPentagon(pentagon), Is.True);
            for (var n = 0; n <= 5; n++)
            {
                Assert.That(H3.CellToChildrenSize(hexagon, 8 + n, out var hexagonSize), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(hexagonSize, Is.EqualTo((long)Math.Pow(7, n)));
                Assert.That(H3.CellToChildrenSize(pentagon, 1 + n, out var pentagonSize), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(pentagonSize, Is.EqualTo(1 + (5 * ((long)Math.Pow(7, n) - 1) / 6)));
            }

            Assert.That(H3.CellToChildrenSize(hexagon, 7, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.CellToChildrenSize(hexagon, 16, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.CellToChildren(hexagon, 9, new ulong[6]), Is.EqualTo(H3ErrorCode.MemoryBounds));
            Assert.That(H3.CellToChildren(hexagon, 9, null), Is.EqualTo(H3ErrorCode.MemoryBounds));
            Assert.That(H3.CellToChildren(hexagon, 7, new ulong[7]), Is.EqualTo(H3ErrorCode.ResDomain));
        }

        [Test]
        public void ChildrenOfEveryResolutionZeroCellAreExactlyTheResolutionOneFixture()
        {
            var expected = new HashSet<string>(FixtureFiles.ReadIndexes("res01cells.txt"));
            var actual = new HashSet<string>();
            foreach (var hex in FixtureFiles.ReadIndexes("res00cells.txt"))
            {
                var parent = H3Index.Parse(hex).Value;
                Assert.That(H3.CellToChildrenSize(parent, 1, out var size), Is.EqualTo(H3ErrorCode.Success));
                var children = new ulong[size];
                Assert.That(H3.CellToChildren(parent, 1, children), Is.EqualTo(H3ErrorCode.Success));
                foreach (var child in children)
                {
                    Assert.That(H3.IsValidCell(child), Is.True, H3.H3ToString(child));
                    Assert.That(H3.CellToParent(child, 0, out var back), Is.EqualTo(H3ErrorCode.Success));
                    Assert.That(back, Is.EqualTo(parent));
                    actual.Add(H3.H3ToString(child));
                }
            }

            Assert.That(actual.SetEquals(expected), Is.True);
            Assert.That(actual, Has.Count.EqualTo(842));
        }

        [Test]
        public void ParentsOfResolutionTwoCellsAreInTheCoarserFixtures()
        {
            var res1 = new HashSet<string>(FixtureFiles.ReadIndexes("res01cells.txt"));
            var res0 = new HashSet<string>(FixtureFiles.ReadIndexes("res00cells.txt"));
            foreach (var hex in FixtureFiles.ReadIndexes("res02cells.txt"))
            {
                var cell = H3Index.Parse(hex).Value;
                Assert.That(H3.CellToParent(cell, 1, out var parent1), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(res1.Contains(H3.H3ToString(parent1)), Is.True, hex);
                Assert.That(H3.CellToParent(cell, 0, out var parent0), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(res0.Contains(H3.H3ToString(parent0)), Is.True, hex);
                Assert.That(H3.CellToParent(parent1, 0, out var grandParent), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(grandParent, Is.EqualTo(parent0));
            }
        }

        [Test]
        public void ChildCentresLieWithinAnEdgeLengthOfTheirAncestors()
        {
            // H3 parents only approximately contain their children, so a geometric bound is the right check.
            // A direct child centre sits about 0.65 parent edge lengths from the parent centre; deeper descendants drift a little further.
            foreach (var line in FixtureFiles.ReadCenters("rand09centers.txt").Take(500))
            {
                var child = H3Index.Parse(line.Hex).Value;
                H3.CellToLatLng(child, out var childCenter);
                for (var res = 0; res <= 9; res++)
                {
                    Assert.That(H3.CellToParent(child, res, out var parent), Is.EqualTo(H3ErrorCode.Success));
                    Assert.That(H3.GetResolution(parent), Is.EqualTo(res));
                    H3.CellToLatLng(parent, out var parentCenter);
                    H3.GetHexagonEdgeLengthAvgM(res, out var edge);
                    var factor = res == 8 ? 1.0 : 1.5;
                    Assert.That(H3.GreatCircleDistanceM(childCenter, parentCenter), Is.LessThan(edge * factor), line.Hex + " res " + res);
                }
            }
        }

        [Test]
        public void ParentErrorsMatchTheCLibrary()
        {
            var child = 0x8928308280fffffUL;
            Assert.That(H3.CellToParent(child, 9, out var self), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(self, Is.EqualTo(child));
            Assert.That(H3.CellToParent(child, 10, out _), Is.EqualTo(H3ErrorCode.ResMismatch));
            Assert.That(H3.CellToParent(child, 15, out _), Is.EqualTo(H3ErrorCode.ResMismatch));
            Assert.That(H3.CellToParent(child, -1, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.CellToParent(child, 16, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.CellToParent(child, 5, out var parent), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(H3.H3ToString(parent), Is.EqualTo("85283083fffffff"));
        }

        [Test]
        public void CenterChildRoundTripsThroughParentAndContainsTheCentre()
        {
            foreach (var hex in FixtureFiles.ReadIndexes("res01cells.txt"))
            {
                var cell = H3Index.Parse(hex).Value;
                H3.CellToLatLng(cell, out var center);
                Assert.That(H3.CellToCenterChild(cell, 1, out var self), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(self, Is.EqualTo(cell));
                Assert.That(H3.CellToCenterChild(cell, 0, out _), Is.EqualTo(H3ErrorCode.ResDomain));
                Assert.That(H3.CellToCenterChild(cell, 16, out _), Is.EqualTo(H3ErrorCode.ResDomain));
                for (var childRes = 2; childRes <= 15; childRes += 4)
                {
                    Assert.That(H3.CellToCenterChild(cell, childRes, out var child), Is.EqualTo(H3ErrorCode.Success));
                    Assert.That(H3.IsValidCell(child), Is.True);
                    Assert.That(H3.GetResolution(child), Is.EqualTo(childRes));
                    Assert.That(H3.IsPentagon(child), Is.EqualTo(H3.IsPentagon(cell)));
                    Assert.That(H3.CellToParent(child, 1, out var parent), Is.EqualTo(H3ErrorCode.Success));
                    Assert.That(parent, Is.EqualTo(cell));
                    H3.CellToLatLng(child, out var childCenter);
                    Assert.That(H3.GreatCircleDistanceM(center, childCenter), Is.LessThan(1e-6), hex);
                }
            }
        }

        [Test]
        public void ZeroIndexDigitsClearsOnlyTheRequestedRange()
        {
            var all = ulong.MaxValue;
            var cleared = H3.ZeroIndexDigits(all, 3, 5);
            for (var res = 1; res <= 15; res++)
            {
                Assert.That(H3.GetIndexDigit(cleared, res), Is.EqualTo(res >= 3 && res <= 5 ? Direction.Center : Direction.Invalid), "digit " + res);
            }

            Assert.That(cleared >> 45, Is.EqualTo(all >> 45), "bits above the digits are untouched");
            Assert.That(H3.ZeroIndexDigits(all, 6, 5), Is.EqualTo(all));
            Assert.That(H3.GetIndexDigit(H3.ZeroIndexDigits(all, 15, 15), 15), Is.EqualTo(Direction.Center));
            Assert.That(H3.GetIndexDigit(H3.ZeroIndexDigits(all, 1, 15), 1), Is.EqualTo(Direction.Center));
        }

        private static void AssertChildren(ulong parent, int childRes, ulong[] expected)
        {
            Assert.That(H3.CellToChildrenSize(parent, childRes, out var size), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(size, Is.EqualTo(expected.Length));
            var children = new ulong[size];
            Assert.That(H3.CellToChildren(parent, childRes, children), Is.EqualTo(H3ErrorCode.Success));
            // Uber's checkChildren compares as sets, so the C test lists are not in iteration order
            Assert.That(children.OrderBy(child => child).Select(H3.H3ToString), Is.EqualTo(expected.OrderBy(child => child).Select(H3.H3ToString)));
            Assert.That(children.Distinct().Count(), Is.EqualTo(children.Length));
            Assert.That(children.All(H3.IsValidCell), Is.True);
        }
    }
}
