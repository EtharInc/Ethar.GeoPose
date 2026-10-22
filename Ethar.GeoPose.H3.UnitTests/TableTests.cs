using Ethar.GeoPose.H3.Tables;
using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Sanity checks on the generated lookup tables: shapes, ranges and the cross-table relationships the H3 maths relies on.
    /// </summary>
    [TestFixture]
    internal class TableTests
    {
        private static readonly int[] PentagonBaseCells = { 4, 14, 24, 38, 49, 58, 63, 72, 83, 97, 107, 117 };

        [Test]
        public void ConstantsMatchTheirDefinitions()
        {
            Assert.That(Constants.M_PI, Is.EqualTo(Math.PI));
            Assert.That(Constants.M_PI_2, Is.EqualTo(Math.PI / 2));
            Assert.That(Constants.M_2PI, Is.EqualTo(2 * Math.PI));
            Assert.That(Constants.M_PI_180 * Constants.M_180_PI, Is.EqualTo(1.0).Within(1e-15));
            Assert.That(Math.Sqrt(3) / 2, Is.EqualTo(Constants.M_SQRT3_2).Within(1e-16));
            Assert.That(Constants.M_SIN60, Is.EqualTo(Constants.M_SQRT3_2));
            Assert.That(Constants.M_RSIN60 * Constants.M_SIN60, Is.EqualTo(1.0).Within(1e-15));
            Assert.That(Constants.M_ONETHIRD * 3, Is.EqualTo(1.0).Within(1e-15));
            Assert.That(Constants.M_ONESEVENTH * 7, Is.EqualTo(1.0).Within(1e-15));
            Assert.That(Math.Sin(Constants.M_AP7_ROT_RADS), Is.EqualTo(Constants.M_SIN_AP7_ROT).Within(1e-15));
            Assert.That(Math.Cos(Constants.M_AP7_ROT_RADS), Is.EqualTo(Constants.M_COS_AP7_ROT).Within(1e-15));
            Assert.That(Math.Asin(Math.Sqrt(3.0 / 28.0)), Is.EqualTo(Constants.M_AP7_ROT_RADS).Within(1e-15));
            Assert.That(Constants.RES0_U_GNOMONIC * Constants.INV_RES0_U_GNOMONIC, Is.EqualTo(1.0).Within(1e-15));
            Assert.That(Constants.EPSILON, Is.EqualTo(1e-16));
            Assert.That(Constants.EARTH_RADIUS_KM, Is.EqualTo(6371.007180918475));
            Assert.That(Constants.MAX_H3_RES, Is.EqualTo(15));
            Assert.That(Constants.NUM_ICOSA_FACES, Is.EqualTo(20));
            Assert.That(Constants.NUM_BASE_CELLS, Is.EqualTo(122));
            Assert.That(Constants.NUM_HEX_VERTS, Is.EqualTo(6));
            Assert.That(Constants.NUM_PENT_VERTS, Is.EqualTo(5));
            Assert.That(Constants.NUM_PENTAGONS, Is.EqualTo(12));
            Assert.That(Constants.H3_CELL_MODE, Is.EqualTo(1));
            Assert.That(Constants.H3_DIRECTEDEDGE_MODE, Is.EqualTo(2));
            Assert.That(Constants.H3_EDGE_MODE, Is.EqualTo(3));
            Assert.That(Constants.H3_VERTEX_MODE, Is.EqualTo(4));
            Assert.That(FaceIjkTables.M_SQRT7 * FaceIjkTables.M_RSQRT7, Is.EqualTo(1.0).Within(1e-15));
            Assert.That(Math.Sqrt(7), Is.EqualTo(FaceIjkTables.M_SQRT7).Within(1e-15));
        }

        [Test]
        public void BaseCellTablesHaveTheRightShape()
        {
            Assert.That(BaseCellTables.baseCellData, Has.Length.EqualTo(122));
            Assert.That(BaseCellTables.baseCellNeighbors.GetLength(0), Is.EqualTo(122));
            Assert.That(BaseCellTables.baseCellNeighbors.GetLength(1), Is.EqualTo(7));
            Assert.That(BaseCellTables.baseCellNeighbor60CCWRots.GetLength(0), Is.EqualTo(122));
            Assert.That(BaseCellTables.baseCellNeighbor60CCWRots.GetLength(1), Is.EqualTo(7));
            Assert.That(BaseCellTables.faceIjkBaseCells.GetLength(0), Is.EqualTo(20));
            Assert.That(BaseCellTables.faceIjkBaseCells.GetLength(1), Is.EqualTo(3));
            Assert.That(BaseCellTables.faceIjkBaseCells.GetLength(2), Is.EqualTo(3));
            Assert.That(BaseCellTables.faceIjkBaseCells.GetLength(3), Is.EqualTo(3));
        }

        [Test]
        public void PentagonBaseCellListHasTwelveEntries()
        {
            var pentagons = Enumerable.Range(0, 122).Where(BaseCells.IsBaseCellPentagon).ToArray();
            Assert.That(pentagons, Has.Length.EqualTo(12));
            Assert.That(pentagons, Is.EqualTo(PentagonBaseCells));
            Assert.That(Enumerable.Range(0, 122).Count(baseCell => BaseCellTables.baseCellData[baseCell].IsPentagon), Is.EqualTo(12));
            Assert.That(BaseCells.IsBaseCellPentagon(-1), Is.False);
            Assert.That(BaseCells.IsBaseCellPentagon(122), Is.False);
            Assert.That(BaseCells.IsBaseCellPentagon(127), Is.False);
        }

        [Test]
        public void PentagonsHaveNoNeighbourOnTheKAxisAndHexagonsHaveSevenNeighbours()
        {
            for (var baseCell = 0; baseCell < 122; baseCell++)
            {
                Assert.That(BaseCellTables.baseCellNeighbors[baseCell, 0], Is.EqualTo(baseCell), "centre digit is the cell itself");
                var pentagon = BaseCells.IsBaseCellPentagon(baseCell);
                Assert.That(BaseCellTables.baseCellNeighbors[baseCell, 1] == BaseCells.InvalidBaseCell, Is.EqualTo(pentagon), "base cell " + baseCell);
                Assert.That(BaseCellTables.baseCellNeighbor60CCWRots[baseCell, 1] == BaseCells.InvalidRotations, Is.EqualTo(pentagon), "base cell " + baseCell);
                for (var direction = 2; direction < 7; direction++)
                {
                    Assert.That(BaseCellTables.baseCellNeighbors[baseCell, direction], Is.InRange(0, 121));
                    Assert.That(BaseCellTables.baseCellNeighbor60CCWRots[baseCell, direction], Is.InRange(0, 5));
                }
            }
        }

        [Test]
        public void PolarPentagonsAreFourAndOneHundredSeventeen()
        {
            var polar = Enumerable.Range(0, 122).Where(BaseCells.IsBaseCellPolarPentagon).ToArray();
            Assert.That(polar, Is.EqualTo(new[] { 4, 117 }));
            Assert.That(polar.All(BaseCells.IsBaseCellPentagon), Is.True);
        }

        [Test]
        public void EveryBaseCellIsFoundAtItsHomeCoordinatesWithNoRotation()
        {
            for (var baseCell = 0; baseCell < 122; baseCell++)
            {
                var home = BaseCells.BaseCellToFaceIjk(baseCell);
                Assert.That(home.Face, Is.InRange(0, 19));
                Assert.That(home.Coord.I, Is.InRange(0, 2));
                Assert.That(home.Coord.J, Is.InRange(0, 2));
                Assert.That(home.Coord.K, Is.InRange(0, 2));
                Assert.That(BaseCells.FaceIjkToBaseCell(home), Is.EqualTo(baseCell));
                Assert.That(BaseCells.FaceIjkToBaseCellCcwRot60(home), Is.EqualTo(0));
                Assert.That(BaseCells.BaseCellToCcwRot60(baseCell, home.Face), Is.EqualTo(0));
            }
        }

        [Test]
        public void FaceIjkBaseCellsOnlyNameRealBaseCellsAndRotations()
        {
            var seen = new HashSet<int>();
            foreach (BaseCellRotation entry in BaseCellTables.faceIjkBaseCells)
            {
                Assert.That(entry.BaseCell, Is.InRange(0, 121));
                Assert.That(entry.CcwRot60, Is.InRange(0, 5));
                seen.Add(entry.BaseCell);
            }

            Assert.That(seen, Has.Count.EqualTo(122));
        }

        [Test]
        public void BaseCellDirectionIsTheInverseOfBaseCellNeighbour()
        {
            for (var baseCell = 0; baseCell < 122; baseCell++)
            {
                for (var direction = Direction.KAxes; direction < Direction.Invalid; direction++)
                {
                    var neighbour = BaseCells.GetBaseCellNeighbor(baseCell, direction);
                    if (neighbour == BaseCells.InvalidBaseCell)
                    {
                        continue;
                    }

                    Assert.That(BaseCells.GetBaseCellDirection(baseCell, neighbour), Is.EqualTo(direction), "base cell " + baseCell);
                    Assert.That(BaseCells.GetBaseCellDirection(neighbour, baseCell), Is.Not.EqualTo(Direction.Invalid), "neighbour " + neighbour + " of " + baseCell);
                }

                Assert.That(BaseCells.GetBaseCellDirection(baseCell, baseCell), Is.EqualTo(Direction.Center));
            }

            Assert.That(BaseCells.GetBaseCellDirection(0, 121), Is.EqualTo(Direction.Invalid));
        }

        [Test]
        public void PentagonClockwiseOffsetFacesAreOnlySetForPentagons()
        {
            for (var baseCell = 0; baseCell < 122; baseCell++)
            {
                var data = BaseCellTables.baseCellData[baseCell];
                if (!data.IsPentagon)
                {
                    Assert.That(data.CwOffsetPent[0], Is.EqualTo(0));
                    Assert.That(data.CwOffsetPent[1], Is.EqualTo(0));
                    continue;
                }

                Assert.That(data.CwOffsetPent[0], Is.InRange(-1, 19));
                Assert.That(data.CwOffsetPent[1], Is.InRange(-1, 19));
                if (data.CwOffsetPent[0] >= 0)
                {
                    Assert.That(BaseCells.BaseCellIsCwOffset(baseCell, data.CwOffsetPent[0]), Is.True);
                }
            }

            Assert.That(BaseCells.BaseCellIsCwOffset(4, 0), Is.False);
        }

        [Test]
        public void FaceCentresAreUnitVectors()
        {
            Assert.That(FaceIjkTables.faceCenterPoint, Has.Length.EqualTo(20));
            foreach (var point in FaceIjkTables.faceCenterPoint)
            {
                var length = Math.Sqrt((point.X * point.X) + (point.Y * point.Y) + (point.Z * point.Z));
                Assert.That(length, Is.EqualTo(1.0).Within(1e-12));
            }

            Assert.That(FaceIjkTables.faceCenterPoint[0].X, Is.EqualTo(0.2199307791404606));
            Assert.That(FaceIjkTables.faceCenterPoint[19].Z, Is.EqualTo(-0.8697775121287253));
        }

        [Test]
        public void FaceAxisAzimuthsAreThreePerFaceInRadians()
        {
            Assert.That(FaceIjkTables.faceAxesAzRadsCII.GetLength(0), Is.EqualTo(20));
            Assert.That(FaceIjkTables.faceAxesAzRadsCII.GetLength(1), Is.EqualTo(3));
            foreach (var azimuth in FaceIjkTables.faceAxesAzRadsCII)
            {
                Assert.That(azimuth, Is.InRange(0.0, 2 * Math.PI));
            }

            Assert.That(FaceIjkTables.faceAxesAzRadsCII[0, 0], Is.EqualTo(5.619958268523939882));
            Assert.That(FaceIjkTables.faceAxesAzRadsCII[19, 2], Is.EqualTo(4.455774101589558636));
        }

        [Test]
        public void FaceNeighboursAgreeWithTheAdjacentFaceDirectionTable()
        {
            Assert.That(FaceIjkTables.faceNeighbors.GetLength(0), Is.EqualTo(20));
            Assert.That(FaceIjkTables.faceNeighbors.GetLength(1), Is.EqualTo(4));
            Assert.That(FaceIjkTables.adjacentFaceDir.GetLength(0), Is.EqualTo(20));
            Assert.That(FaceIjkTables.adjacentFaceDir.GetLength(1), Is.EqualTo(20));
            for (var face = 0; face < 20; face++)
            {
                var central = FaceIjkTables.faceNeighbors[face, 0];
                Assert.That(central.Face, Is.EqualTo(face));
                Assert.That(central.CcwRot60, Is.EqualTo(0));
                Assert.That(central.Translate.I + central.Translate.J + central.Translate.K, Is.EqualTo(0));
                Assert.That(FaceIjkTables.adjacentFaceDir[face, face], Is.EqualTo(0));

                var adjacent = 0;
                for (var other = 0; other < 20; other++)
                {
                    var direction = FaceIjkTables.adjacentFaceDir[face, other];
                    Assert.That(direction, Is.InRange(-1, 3));
                    Assert.That(direction == -1, Is.EqualTo(FaceIjkTables.adjacentFaceDir[other, face] == -1), "adjacency is symmetric");
                    if (other != face && direction != -1)
                    {
                        adjacent++;
                        Assert.That(FaceIjkTables.faceNeighbors[face, direction].Face, Is.EqualTo(other));
                        Assert.That(FaceIjkTables.faceNeighbors[face, direction].CcwRot60, Is.InRange(0, 5));
                    }
                }

                Assert.That(adjacent, Is.EqualTo(3), "face " + face);
            }
        }

        [Test]
        public void OverageAndUnitScaleTablesFollowPowersOfSeven()
        {
            Assert.That(FaceIjkTables.maxDimByCIIres, Has.Length.EqualTo(17));
            Assert.That(FaceIjkTables.unitScaleByCIIres, Has.Length.EqualTo(17));
            for (var resolution = 0; resolution <= 16; resolution++)
            {
                if (resolution % 2 == 1)
                {
                    Assert.That(FaceIjkTables.maxDimByCIIres[resolution], Is.EqualTo(-1));
                    Assert.That(FaceIjkTables.unitScaleByCIIres[resolution], Is.EqualTo(-1));
                    continue;
                }

                var scale = (int)Math.Pow(7, resolution / 2);
                Assert.That(FaceIjkTables.unitScaleByCIIres[resolution], Is.EqualTo(scale));
                Assert.That(FaceIjkTables.maxDimByCIIres[resolution], Is.EqualTo(2 * scale));
            }
        }

        [Test]
        public void UnitVectorsEncodeTheDigitBits()
        {
            Assert.That(CoordIjkTables.UNIT_VECS, Has.Length.EqualTo(7));
            for (var digit = 0; digit < 7; digit++)
            {
                var vector = CoordIjkTables.UNIT_VECS[digit];
                Assert.That(vector.I, Is.EqualTo((digit >> 2) & 1));
                Assert.That(vector.J, Is.EqualTo((digit >> 1) & 1));
                Assert.That(vector.K, Is.EqualTo(digit & 1));
            }
        }

        [Test]
        public void DigitRotationTablesAreInverseSixfoldCycles()
        {
            Assert.That(CoordIjkTables.ROTATE60CCW, Has.Length.EqualTo(8));
            Assert.That(CoordIjkTables.ROTATE60CW, Has.Length.EqualTo(8));
            Assert.That(CoordIjkTables.ROTATE60CCW[(int)Direction.Center], Is.EqualTo(Direction.Center));
            Assert.That(CoordIjkTables.ROTATE60CW[(int)Direction.Center], Is.EqualTo(Direction.Center));
            Assert.That(CoordIjkTables.ROTATE60CCW[(int)Direction.Invalid], Is.EqualTo(Direction.Invalid));
            Assert.That(CoordIjkTables.ROTATE60CW[(int)Direction.Invalid], Is.EqualTo(Direction.Invalid));
            Assert.That(CoordIjkTables.ROTATE60CCW[(int)Direction.KAxes], Is.EqualTo(Direction.IkAxes));
            Assert.That(CoordIjkTables.ROTATE60CW[(int)Direction.KAxes], Is.EqualTo(Direction.JkAxes));
            for (var digit = Direction.KAxes; digit < Direction.Invalid; digit++)
            {
                Assert.That(CoordIjkTables.ROTATE60CW[(int)CoordIjkTables.ROTATE60CCW[(int)digit]], Is.EqualTo(digit));
                var rotated = digit;
                for (var turn = 0; turn < 6; turn++)
                {
                    rotated = CoordIjkTables.ROTATE60CCW[(int)rotated];
                    Assert.That(rotated == digit, Is.EqualTo(turn == 5), "digit " + digit + " turn " + turn);
                }
            }
        }
    }
}
