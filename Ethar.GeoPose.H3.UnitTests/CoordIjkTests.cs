using Ethar.GeoPose.H3.Tables;
using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// The IJK coordinate maths and vector helpers, checked with the identities the projection relies on.
    /// </summary>
    [TestFixture]
    internal class CoordIjkTests
    {
        [Test]
        public void NormalizeRemovesNegativesAndTheCommonMinimum()
        {
            var c = new CoordIjk(-1, 2, 3);
            CoordIjk.IjkNormalize(ref c);
            Assert.That(c, Is.EqualTo(new CoordIjk(0, 3, 4)));
            c = new CoordIjk(2, 2, 2);
            CoordIjk.IjkNormalize(ref c);
            Assert.That(c, Is.EqualTo(new CoordIjk(0, 0, 0)));
            c = new CoordIjk(5, 3, 1);
            CoordIjk.IjkNormalize(ref c);
            Assert.That(c, Is.EqualTo(new CoordIjk(4, 2, 0)));
            c = new CoordIjk(0, -4, -2);
            CoordIjk.IjkNormalize(ref c);
            Assert.That(c, Is.EqualTo(new CoordIjk(4, 0, 2)));
        }

        [Test]
        public void Hex2dRoundTripsThroughIjkForAGridOfCentres()
        {
            for (var i = 0; i < 12; i++)
            {
                for (var j = 0; j < 12; j++)
                {
                    var ijk = new CoordIjk(i, j, 0);
                    CoordIjk.IjkNormalize(ref ijk);
                    var hex = CoordIjk.IjkToHex2d(ijk);
                    var back = CoordIjk.Hex2dToCoordIjk(hex);
                    Assert.That(back, Is.EqualTo(ijk), ijk.ToString());

                    // a small offset from the centre lands in the same hex
                    var nudged = CoordIjk.Hex2dToCoordIjk(new Vec2d(hex.X + 0.3, hex.Y - 0.2));
                    Assert.That(nudged, Is.EqualTo(ijk), ijk + " nudged");
                }
            }

            // (-2, -0.5) is nearest the centre of the hex at ijk+ (0, 2, 2), whose hex2d centre is (-2, 0)
            Assert.That(CoordIjk.Hex2dToCoordIjk(new Vec2d(-2.0, -0.5)).ToString(), Is.EqualTo("(0, 2, 2)"));
            Assert.That(CoordIjk.IjkToHex2d(new CoordIjk(0, 2, 2)).ToString(), Is.EqualTo("(-2, 0)"));
        }

        [Test]
        public void UnitVectorsMapToTheirDigitsAndOthersAreInvalid()
        {
            for (var digit = 0; digit < 7; digit++)
            {
                Assert.That(CoordIjk.UnitIjkToDigit(CoordIjkTables.UNIT_VECS[digit]), Is.EqualTo((Direction)digit));
            }

            Assert.That(CoordIjk.UnitIjkToDigit(new CoordIjk(1, 1, 1)), Is.EqualTo(Direction.Center), "normalizes to zero first");
            Assert.That(CoordIjk.UnitIjkToDigit(new CoordIjk(2, 0, 0)), Is.EqualTo(Direction.Invalid));
        }

        [Test]
        public void ApertureSevenDownThenUpIsTheIdentity()
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var start = new CoordIjk(i, j, 0);
                    CoordIjk.IjkNormalize(ref start);

                    var ccw = start;
                    CoordIjk.DownAp7(ref ccw);
                    CoordIjk.UpAp7(ref ccw);
                    Assert.That(ccw, Is.EqualTo(start), "ccw " + start);

                    var cw = start;
                    CoordIjk.DownAp7r(ref cw);
                    CoordIjk.UpAp7r(ref cw);
                    Assert.That(cw, Is.EqualTo(start), "cw " + start);

                    // each of the seven neighbours of the fine centre still rounds up to the same parent
                    var fine = start;
                    CoordIjk.DownAp7(ref fine);
                    for (var digit = Direction.Center; digit < Direction.Invalid; digit++)
                    {
                        var neighbour = fine;
                        CoordIjk.Neighbor(ref neighbour, digit);
                        CoordIjk.UpAp7(ref neighbour);
                        Assert.That(neighbour, Is.EqualTo(start), start + " digit " + digit);
                    }
                }
            }
        }

        [Test]
        public void SixtyDegreeRotationsCycleSixTimesAndInvertEachOther()
        {
            var start = new CoordIjk(3, 1, 0);
            var rotated = start;
            for (var turn = 0; turn < 6; turn++)
            {
                CoordIjk.IjkRotate60ccw(ref rotated);
                Assert.That(CoordIjk.IjkMatches(rotated, start), Is.EqualTo(turn == 5));
            }

            var back = start;
            CoordIjk.IjkRotate60ccw(ref back);
            CoordIjk.IjkRotate60cw(ref back);
            Assert.That(back, Is.EqualTo(start));

            for (var digit = Direction.KAxes; digit < Direction.Invalid; digit++)
            {
                Assert.That(CoordIjk.Rotate60cw(CoordIjk.Rotate60ccw(digit)), Is.EqualTo(digit));
                var vector = CoordIjkTables.UNIT_VECS[(int)digit];
                CoordIjk.IjkRotate60ccw(ref vector);
                Assert.That(CoordIjk.UnitIjkToDigit(vector), Is.EqualTo(CoordIjk.Rotate60ccw(digit)), "rotating the vector matches rotating the digit");
            }
        }

        [Test]
        public void DistanceAndArithmeticBehave()
        {
            // the i, j and k axes are 120 degrees apart, so unit i and unit j are two steps apart while unit i and unit ij are adjacent
            Assert.That(CoordIjk.IjkDistance(new CoordIjk(0, 0, 0), new CoordIjk(3, 0, 0)), Is.EqualTo(3));
            Assert.That(CoordIjk.IjkDistance(new CoordIjk(1, 0, 0), new CoordIjk(0, 1, 0)), Is.EqualTo(2));
            Assert.That(CoordIjk.IjkDistance(new CoordIjk(1, 0, 0), new CoordIjk(1, 1, 0)), Is.EqualTo(1));
            Assert.That(CoordIjk.IjkDistance(new CoordIjk(2, 0, 0), new CoordIjk(0, 0, 2)), Is.EqualTo(4));
            for (var digit = Direction.KAxes; digit < Direction.Invalid; digit++)
            {
                Assert.That(CoordIjk.IjkDistance(new CoordIjk(0, 0, 0), CoordIjkTables.UNIT_VECS[(int)digit]), Is.EqualTo(1), "every unit vector is one step from the origin");
            }
            Assert.That(CoordIjk.IjkAdd(new CoordIjk(1, 2, 3), new CoordIjk(4, 5, 6)), Is.EqualTo(new CoordIjk(5, 7, 9)));
            Assert.That(CoordIjk.IjkSub(new CoordIjk(1, 2, 3), new CoordIjk(4, 5, 6)), Is.EqualTo(new CoordIjk(-3, -3, -3)));
            var scaled = new CoordIjk(1, 2, 3);
            CoordIjk.IjkScale(ref scaled, 3);
            Assert.That(scaled, Is.EqualTo(new CoordIjk(3, 6, 9)));
            Assert.That(CoordIjk.IjkMatches(scaled, new CoordIjk(3, 6, 9)), Is.True);
        }

        [Test]
        public void Vec2dHelpersMatchTheCDefinitions()
        {
            Assert.That(Vec2d.V2dMag(new Vec2d(3, 4)), Is.EqualTo(5));
            var inter = Vec2d.V2dIntersect(new Vec2d(0, 0), new Vec2d(2, 2), new Vec2d(0, 2), new Vec2d(2, 0));
            Assert.That(inter.X, Is.EqualTo(1).Within(1e-15));
            Assert.That(inter.Y, Is.EqualTo(1).Within(1e-15));
            Assert.That(Vec2d.V2dAlmostEquals(new Vec2d(1, 1), new Vec2d(1 + 1e-8, 1 - 1e-8)), Is.True);
            Assert.That(Vec2d.V2dAlmostEquals(new Vec2d(1, 1), new Vec2d(1 + 1e-6, 1)), Is.False);
        }

        [Test]
        public void Vec3dHelpersMatchTheCDefinitions()
        {
            var geo = LatLng.FromDegrees(37.7752702151959, -122.418307270836);
            var v = Vec3d.LatLngToVec3(geo);
            Assert.That(Vec3d.Vec3Norm(v), Is.EqualTo(1).Within(1e-15));
            var back = Vec3d.Vec3ToLatLng(v);
            Assert.That(back.Lat, Is.EqualTo(geo.Lat).Within(1e-15));
            Assert.That(back.Lng, Is.EqualTo(geo.Lng).Within(1e-15));

            var x = new Vec3d(1, 0, 0);
            var y = new Vec3d(0, 1, 0);
            var z = Vec3d.Vec3Cross(x, y);
            Assert.That(z.Z, Is.EqualTo(1));
            Assert.That(Vec3d.Vec3Dot(x, y), Is.EqualTo(0));
            Assert.That(Vec3d.Vec3DistSq(x, y), Is.EqualTo(2));
            var combo = Vec3d.Vec3LinComb(2, x, 3, y);
            Assert.That(combo.X, Is.EqualTo(2));
            Assert.That(combo.Y, Is.EqualTo(3));

            var zero = new Vec3d(0, 0, 0);
            Vec3d.Vec3Normalize(ref zero);
            Assert.That(zero.X, Is.EqualTo(0));
            var scaled = new Vec3d(0, 0, 5);
            Vec3d.Vec3Normalize(ref scaled);
            Assert.That(scaled.Z, Is.EqualTo(1));

            FaceIjk.Vec3ToClosestFace(FaceIjkTables.faceCenterPoint[7], out var face, out var sqd);
            Assert.That(face, Is.EqualTo(7));
            Assert.That(sqd, Is.EqualTo(0));
            Assert.That(FaceIjk.Vec3AzimuthRads(x, z), Is.EqualTo(0).Within(1e-15), "due north");
            Assert.That(FaceIjk.Vec3AzimuthRads(x, y), Is.EqualTo(Math.PI / 2).Within(1e-15), "due east");
        }

        [Test]
        public void PosAngleAndConstraintsFoldIntoRange()
        {
            Assert.That(LatLng.PosAngleRads(-0.5), Is.EqualTo(Constants.M_2PI - 0.5).Within(1e-15));
            Assert.That(LatLng.PosAngleRads(Constants.M_2PI + 0.5), Is.EqualTo(0.5).Within(1e-15));
            Assert.That(LatLng.PosAngleRads(1.0), Is.EqualTo(1.0));
            Assert.That(LatLng.ConstrainLng(Math.PI + 1), Is.EqualTo(-Math.PI + 1).Within(1e-15));
            Assert.That(LatLng.ConstrainLng(-Math.PI - 1), Is.EqualTo(Math.PI - 1).Within(1e-15));
            Assert.That(LatLng.ConstrainLat(Math.PI / 2 + 0.1), Is.EqualTo(-Math.PI / 2 + 0.1).Within(1e-15));
            Assert.That(LatLng.GeoAlmostEqual(new LatLng(1, 1), new LatLng(1 + 1e-12, 1)), Is.True);
            Assert.That(LatLng.GeoAlmostEqual(new LatLng(1, 1), new LatLng(1 + 1e-6, 1)), Is.False);
            Assert.That(new LatLng(1, 2) == new LatLng(1, 2), Is.True);
            Assert.That(new LatLng(1, 2) != new LatLng(2, 1), Is.True);
            var first = new LatLng(1, 2);
            var second = new LatLng(1, 2);
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
            Assert.That(first.Equals((object)second), Is.True);
            Assert.That(new LatLng(0.5, -1.25).ToString(), Is.EqualTo("Lat:0.5, Lng:-1.25"));
        }
    }
}
