using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// The hierarchy and geometry methods on <see cref="H3Index"/>, which wrap the engine and throw <see cref="H3Exception"/> instead of returning codes.
    /// </summary>
    [TestFixture]
    internal class H3IndexMethodTests
    {
        private static readonly H3Index Documented = H3Index.Parse("8928308280fffff");

        [Test]
        public void ParentAndAncestorsWalkUpTheHierarchy()
        {
            Assert.That(Documented.Parent(9), Is.EqualTo(Documented));
            Assert.That(Documented.Parent(5).ToString(), Is.EqualTo("85283083fffffff"));
            Assert.That(Documented.Parent(0).Resolution, Is.EqualTo(0));
            Assert.That(Documented.Parent(0).BaseCell, Is.EqualTo(20));

            var ancestors = Documented.Ancestors();
            Assert.That(ancestors, Has.Length.EqualTo(10));
            for (var resolution = 0; resolution <= 9; resolution++)
            {
                Assert.That(ancestors[resolution].Resolution, Is.EqualTo(resolution));
                Assert.That(ancestors[resolution], Is.EqualTo(Documented.Parent(resolution)));
                Assert.That(ancestors[resolution].Contains(Documented), Is.True);
                Assert.That(Documented.IsWithin(ancestors[resolution]), Is.True);
            }

            Assert.That(ancestors[9], Is.EqualTo(Documented));
            Assert.Throws<H3Exception>(() => Documented.Parent(10));
            Assert.Throws<H3Exception>(() => Documented.Parent(-1));
            Assert.That(Assert.Throws<H3Exception>(() => Documented.Parent(16)).ErrorCode, Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(Assert.Throws<H3Exception>(() => Documented.Parent(12)).ErrorCode, Is.EqualTo(H3ErrorCode.ResMismatch));
        }

        [Test]
        public void ChildrenAndCenterChildWalkDownTheHierarchy()
        {
            Assert.That(Documented.CenterChild(9), Is.EqualTo(Documented));
            var center = Documented.CenterChild(12);
            Assert.That(center.Resolution, Is.EqualTo(12));
            Assert.That(center.Parent(9), Is.EqualTo(Documented));
            Assert.That(H3.GreatCircleDistanceM(center.Center, Documented.Center), Is.LessThan(1e-6));

            Assert.That(Documented.ChildrenSize(9), Is.EqualTo(1));
            Assert.That(Documented.ChildrenSize(10), Is.EqualTo(7));
            Assert.That(Documented.ChildrenSize(11), Is.EqualTo(49));
            var children = Documented.Children(11);
            Assert.That(children, Has.Length.EqualTo(49));
            Assert.That(children.Distinct().Count(), Is.EqualTo(49));
            Assert.That(children.Select(child => child.Parent(9)), Is.All.EqualTo(Documented));
            Assert.That(children.Select(child => Documented.Contains(child)), Is.All.True);
            Assert.That(children.Contains(center.Parent(11)), Is.True);

            Assert.That(Assert.Throws<H3Exception>(() => Documented.CenterChild(8)).ErrorCode, Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(Assert.Throws<H3Exception>(() => Documented.Children(8)).ErrorCode, Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(Assert.Throws<H3Exception>(() => Documented.ChildrenSize(16)).ErrorCode, Is.EqualTo(H3ErrorCode.ResDomain));
        }

        [Test]
        public void ContainsFollowsTheIndexHierarchy()
        {
            var parent = Documented.Parent(6);
            var sibling = Documented.Parent(7).Children(8).First(child => child != Documented.Parent(8));
            Assert.That(parent.Contains(Documented), Is.True);
            Assert.That(parent.Contains(parent), Is.True);
            Assert.That(Documented.Contains(parent), Is.False);
            Assert.That(sibling.Contains(Documented), Is.False);
            Assert.That(Documented.Parent(7).Contains(sibling), Is.True);
            Assert.That(H3Index.Parse("8001fffffffffff").Contains(Documented), Is.False, "a different base cell");
            Assert.That(Documented.IsWithin(sibling), Is.False);
        }

        [Test]
        public void CentreBoundaryAndFromLatLngWrapTheEngine()
        {
            H3.CellToLatLng(Documented.Value, out var center);
            Assert.That(Documented.Center, Is.EqualTo(center));
            var boundary = Documented.Boundary();
            Assert.That(boundary.Count, Is.EqualTo(6));
            Assert.That(H3Index.FromLatLng(center, 9), Is.EqualTo(Documented));
            Assert.That(H3Index.FromLatLng(center, 15).Parent(9), Is.EqualTo(Documented));
            Assert.That(Assert.Throws<H3Exception>(() => H3Index.FromLatLng(center, 16)).ErrorCode, Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(Assert.Throws<H3Exception>(() => H3Index.FromLatLng(new LatLng(double.NaN, 0), 1)).ErrorCode, Is.EqualTo(H3ErrorCode.LatLngDomain));
        }

        [Test]
        public void PentagonHierarchyHasSixChildrenPerStep()
        {
            var pentagon = new H3Index(H3.SetH3Index(2, 4, Direction.Center));
            Assert.That(pentagon.IsPentagon, Is.True);
            var children = pentagon.Children(3);
            Assert.That(children, Has.Length.EqualTo(6));
            Assert.That(children.Count(child => child.IsPentagon), Is.EqualTo(1));
            Assert.That(children.Select(child => child.Parent(2)), Is.All.EqualTo(pentagon));
            Assert.That(pentagon.CenterChild(3).IsPentagon, Is.True);
            Assert.That(pentagon.ChildrenSize(4), Is.EqualTo(1 + (5 * (49 - 1) / 6)));
        }
    }
}
