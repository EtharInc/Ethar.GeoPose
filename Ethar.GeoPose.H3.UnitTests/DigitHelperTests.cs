using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// The internal digit get and set helpers, index construction, rotations and pentagon enumeration ported from h3Index.c.
    /// </summary>
    [TestFixture]
    internal class DigitHelperTests
    {
        [Test]
        public void SetH3IndexFillsDigitsToTheResolutionAndSevensAfterIt()
        {
            var value = H3.SetH3Index(9, 20, Direction.Center);
            Assert.That(H3.GetResolution(value), Is.EqualTo(9));
            Assert.That(H3.GetBaseCellNumber(value), Is.EqualTo(20));
            Assert.That(H3.GetMode(value), Is.EqualTo(1));
            Assert.That(H3.GetHighBit(value), Is.EqualTo(0));
            Assert.That(H3.GetReservedBits(value), Is.EqualTo(0));
            Assert.That(H3.IsValidCell(value), Is.True);
            Assert.That(H3.H3ToString(value), Is.EqualTo("89280000003ffff"));
            for (var res = 1; res <= 15; res++)
            {
                Assert.That(H3.GetIndexDigit(value, res), Is.EqualTo(res <= 9 ? Direction.Center : Direction.Invalid), "digit " + res);
            }
        }

        [Test]
        public void SetAndGetIndexDigitRoundTripAtEveryResolution()
        {
            for (var res = 1; res <= 15; res++)
            {
                for (var digit = Direction.Center; digit <= Direction.Invalid; digit++)
                {
                    var value = H3.SetIndexDigit(H3.SetH3Index(15, 0, Direction.Center), res, digit);
                    Assert.That(H3.GetIndexDigit(value, res), Is.EqualTo(digit));
                    for (var other = 1; other <= 15; other++)
                    {
                        if (other != res)
                        {
                            Assert.That(H3.GetIndexDigit(value, other), Is.EqualTo(Direction.Center), "digit " + other + " untouched");
                        }
                    }
                }
            }
        }

        [Test]
        public void PublicGetIndexDigitValidatesTheResolution()
        {
            var value = 0x8928308280fffffUL;
            Assert.That(H3.GetIndexDigit(value, 0, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.GetIndexDigit(value, 16, out _), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.GetIndexDigit(value, 1, out var first), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(first, Is.EqualTo(0));
            Assert.That(H3.GetIndexDigit(value, 2, out var second), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(second, Is.EqualTo(6));
            Assert.That(H3.GetIndexDigit(value, 9, out var last), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(last, Is.EqualTo(3));
            Assert.That(H3.GetIndexDigit(value, 10, out var beyond), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(beyond, Is.EqualTo(7));
        }

        [Test]
        public void FieldSettersTouchOnlyTheirBits()
        {
            var value = H3.SetH3Index(3, 7, Direction.JAxes);
            Assert.That(H3.GetMode(H3.SetMode(value, 2)), Is.EqualTo(2));
            Assert.That(H3.SetMode(H3.SetMode(value, 2), 1), Is.EqualTo(value));
            Assert.That(H3.GetResolution(H3.SetResolution(value, 12)), Is.EqualTo(12));
            Assert.That(H3.SetResolution(H3.SetResolution(value, 12), 3), Is.EqualTo(value));
            Assert.That(H3.GetBaseCellNumber(H3.SetBaseCell(value, 121)), Is.EqualTo(121));
            Assert.That(H3.SetBaseCell(H3.SetBaseCell(value, 121), 7), Is.EqualTo(value));
            Assert.That(H3.GetReservedBits(H3.SetReservedBits(value, 5)), Is.EqualTo(5));
            Assert.That(H3.SetReservedBits(H3.SetReservedBits(value, 5), 0), Is.EqualTo(value));
            Assert.That(H3.GetHighBit(H3.SetHighBit(value, 1)), Is.EqualTo(1));
            Assert.That(H3.SetHighBit(H3.SetHighBit(value, 1), 0), Is.EqualTo(value));
        }

        [Test]
        public void LeadingNonZeroDigitSkipsCentreDigits()
        {
            var value = H3.SetH3Index(5, 20, Direction.Center);
            Assert.That(H3.LeadingNonZeroDigit(value), Is.EqualTo(Direction.Center));
            Assert.That(H3.LeadingNonZeroDigit(H3.SetIndexDigit(value, 3, Direction.IjAxes)), Is.EqualTo(Direction.IjAxes));
            Assert.That(H3.LeadingNonZeroDigit(H3.SetIndexDigit(H3.SetIndexDigit(value, 3, Direction.IjAxes), 2, Direction.KAxes)), Is.EqualTo(Direction.KAxes));
        }

        [Test]
        public void SixCounterClockwiseRotationsAreTheIdentityAndClockwiseUndoesCounterClockwise()
        {
            // The plain rotation only preserves validity under hexagon base cells. Under a pentagon base cell it can
            // rotate a leading digit onto the deleted k axis, which is why the pentagon rotations exist.
            foreach (var hex in FixtureFiles.ReadIndexes("res02cells.txt"))
            {
                var value = H3Index.Parse(hex).Value;
                var hexagonBaseCell = !BaseCells.IsBaseCellPentagon(H3.GetBaseCellNumber(value));
                var rotated = value;
                for (var turn = 0; turn < 6; turn++)
                {
                    rotated = H3.Rotate60ccw(rotated);
                    if (hexagonBaseCell)
                    {
                        Assert.That(H3.IsValidCell(rotated), Is.True, hex);
                    }
                }

                Assert.That(rotated, Is.EqualTo(value), hex);
                Assert.That(H3.Rotate60cw(H3.Rotate60ccw(value)), Is.EqualTo(value), hex);
                Assert.That(H3.Rotate60ccw(H3.Rotate60cw(value)), Is.EqualTo(value), hex);
            }
        }

        [Test]
        public void PentagonRotationsKeepPentagonsValid()
        {
            for (var resolution = 0; resolution <= 15; resolution++)
            {
                var pentagons = new ulong[12];
                Assert.That(H3.GetPentagons(resolution, pentagons), Is.EqualTo(H3ErrorCode.Success));
                foreach (var pentagon in pentagons)
                {
                    var ccw = H3.RotatePent60ccw(pentagon);
                    var cw = H3.RotatePent60cw(pentagon);
                    Assert.That(H3.IsValidCell(ccw), Is.True);
                    Assert.That(H3.IsValidCell(cw), Is.True);
                    Assert.That(H3.IsPentagon(ccw), Is.True);
                    Assert.That(H3.IsPentagon(cw), Is.True);
                }
            }

            var child = H3.SetIndexDigit(H3.SetH3Index(2, 4, Direction.Center), 2, Direction.JAxes);
            var rotatedChild = H3.RotatePent60ccw(child);
            Assert.That(H3.IsValidCell(rotatedChild), Is.True);
            Assert.That(H3.GetIndexDigit(rotatedChild, 2), Is.Not.EqualTo(Direction.KAxes), "the deleted k axis digit is skipped");
        }

        [Test]
        public void GetPentagonsReturnsTwelveValidPentagonsAtEveryResolution()
        {
            Assert.That(H3.PentagonCount(), Is.EqualTo(12));
            for (var resolution = 0; resolution <= 15; resolution++)
            {
                var pentagons = new ulong[12];
                Assert.That(H3.GetPentagons(resolution, pentagons), Is.EqualTo(H3ErrorCode.Success));
                Assert.That(pentagons.Distinct().Count(), Is.EqualTo(12));
                foreach (var pentagon in pentagons)
                {
                    Assert.That(H3.IsValidCell(pentagon), Is.True);
                    Assert.That(H3.IsPentagon(pentagon), Is.True);
                    Assert.That(H3.GetResolution(pentagon), Is.EqualTo(resolution));
                    Assert.That(BaseCells.IsBaseCellPentagon(H3.GetBaseCellNumber(pentagon)), Is.True);
                }
            }

            Assert.That(H3.GetPentagons(16, new ulong[12]), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.GetPentagons(-1, new ulong[12]), Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(H3.GetPentagons(0, new ulong[11]), Is.EqualTo(H3ErrorCode.MemoryBounds));
            Assert.That(H3.GetPentagons(0, null), Is.EqualTo(H3ErrorCode.MemoryBounds));
        }

        [Test]
        public void Res0CellsMatchTheFixtureInOrder()
        {
            Assert.That(H3.Res0CellCount(), Is.EqualTo(122));
            var cells = new ulong[122];
            Assert.That(H3.GetRes0Cells(cells), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(cells.Select(H3.H3ToString), Is.EqualTo(FixtureFiles.ReadIndexes("res00cells.txt")));
            Assert.That(H3.GetRes0Cells(new ulong[121]), Is.EqualTo(H3ErrorCode.MemoryBounds));
        }

        [Test]
        public void OddResolutionsAreClassIii()
        {
            for (var resolution = 0; resolution <= 15; resolution++)
            {
                Assert.That(H3.IsResolutionClassIII(resolution), Is.EqualTo(resolution % 2 == 1));
            }
        }
    }
}
