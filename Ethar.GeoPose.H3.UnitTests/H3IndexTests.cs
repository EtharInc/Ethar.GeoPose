using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Bit layout, parsing, formatting and validation of <see cref="H3Index"/> and the static <see cref="H3"/> functions.
    /// </summary>
    [TestFixture]
    internal class H3IndexTests
    {
        /// <summary>
        /// Resolution 9 cell under base cell 20 from the H3 documentation.
        /// </summary>
        private const ulong Documented = 0x8928308280fffffUL;

        [TestCase("8928308280fffff", 9, 20)]
        [TestCase("85283473fffffff", 5, 20)]
        [TestCase("8001fffffffffff", 0, 0)]
        [TestCase("80f3fffffffffff", 0, 121)]
        public void DocumentedValuesParseToResolutionAndBaseCell(string hex, int resolution, int baseCell)
        {
            var index = H3Index.Parse(hex);
            Assert.That(index.Resolution, Is.EqualTo(resolution));
            Assert.That(index.BaseCell, Is.EqualTo(baseCell));
            Assert.That(H3.GetResolution(index.Value), Is.EqualTo(resolution));
            Assert.That(H3.GetBaseCellNumber(index.Value), Is.EqualTo(baseCell));
        }

        [Test]
        public void HexRoundTripsThroughValueAndString()
        {
            var index = new H3Index(Documented);
            Assert.That(index.ToString(), Is.EqualTo("8928308280fffff"));
            Assert.That(H3Index.Parse(index.ToString()), Is.EqualTo(index));
            Assert.That(H3Index.Parse("8928308280fffff").Value, Is.EqualTo(Documented));
        }

        [TestCase("8928308280FFFFF")]
        [TestCase("8928308280FfFfF")]
        [TestCase("  8928308280fffff  ")]
        public void ParseAcceptsUppercaseAndSurroundingWhitespaceButEmitsLowercase(string text)
        {
            var index = H3Index.Parse(text);
            Assert.That(index.Value, Is.EqualTo(Documented));
            Assert.That(index.ToString(), Is.EqualTo("8928308280fffff"));
            Assert.That(H3Index.TryParse(text, out var parsed), Is.True);
            Assert.That(parsed, Is.EqualTo(index));
        }

        [Test]
        public void ToStringIsFifteenLowercaseHexCharactersForEveryResolution()
        {
            for (var resolution = 0; resolution <= 15; resolution++)
            {
                var pentagons = new ulong[12];
                Assert.That(H3.GetPentagons(resolution, pentagons), Is.EqualTo(H3ErrorCode.Success));
                foreach (var value in pentagons)
                {
                    var text = new H3Index(value).ToString();
                    Assert.That(text, Has.Length.EqualTo(15));
                    Assert.That(text, Is.EqualTo(text.ToLowerInvariant()));
                    Assert.That(text, Does.Match("^[0-9a-f]{15}$"));
                }
            }
        }

        [Test]
        public void StaticH3ToStringDoesNotPadLikeTheCLibrary()
        {
            Assert.That(H3.H3ToString(0), Is.EqualTo("0"));
            Assert.That(H3.H3ToString(Documented), Is.EqualTo("8928308280fffff"));
        }

        public static IEnumerable<TestCaseData> InvalidValues()
        {
            yield return new TestCaseData(0UL).SetName("H3_NULL");
            yield return new TestCaseData(ulong.MaxValue).SetName("all bits set");
            yield return new TestCaseData(Documented & ~(15UL << 59)).SetName("mode 0");
            yield return new TestCaseData((Documented & ~(15UL << 59)) | (2UL << 59)).SetName("mode 2 directed edge");
            yield return new TestCaseData((Documented & ~(15UL << 59)) | (4UL << 59)).SetName("mode 4 vertex");
            yield return new TestCaseData(Documented | (1UL << 63)).SetName("high bit set");
            yield return new TestCaseData(Documented | (1UL << 56)).SetName("reserved bit 56 set");
            yield return new TestCaseData(Documented | (4UL << 56)).SetName("reserved bit 58 set");
            yield return new TestCaseData((Documented & ~(127UL << 45)) | (122UL << 45)).SetName("base cell 122");
            yield return new TestCaseData((Documented & ~(127UL << 45)) | (127UL << 45)).SetName("base cell 127");
            yield return new TestCaseData(Documented | (7UL << ((15 - 3) * 3))).SetName("digit 3 is 7 inside resolution 9");
            yield return new TestCaseData(Documented | (7UL << ((15 - 9) * 3))).SetName("digit 9 is 7 inside resolution 9");
            yield return new TestCaseData(Documented & ~(7UL << ((15 - 10) * 3))).SetName("digit 10 is 0 after resolution 9");
            yield return new TestCaseData(Documented & ~(1UL << ((15 - 15) * 3))).SetName("digit 15 is 6 after resolution 9");
            yield return new TestCaseData(H3.SetIndexDigit(H3.SetH3Index(1, 4, Direction.Center), 1, Direction.KAxes)).SetName("pentagon base cell 4 with deleted digit 1 at res 1");
            yield return new TestCaseData(H3.SetIndexDigit(H3.SetH3Index(3, 14, Direction.Center), 3, Direction.KAxes)).SetName("pentagon base cell 14 with leading zeros then digit 1 at res 3");
            yield return new TestCaseData(H3.SetIndexDigit(H3.SetH3Index(15, 117, Direction.Center), 15, Direction.KAxes)).SetName("pentagon base cell 117 with digit 1 at res 15");
        }

        [TestCaseSource(nameof(InvalidValues))]
        public void InvalidBitPatternsAreRejected(ulong value)
        {
            Assert.That(H3.IsValidCell(value), Is.False);
            Assert.That(H3Index.TryCreate(value, out var index), Is.False);
            Assert.That(index.Value, Is.EqualTo(0UL));
            var exception = Assert.Throws<H3Exception>(() => new H3Index(value));
            Assert.That(exception.ErrorCode, Is.EqualTo(H3ErrorCode.CellInvalid));
        }

        public static IEnumerable<TestCaseData> ValidNeighboursOfInvalidValues()
        {
            yield return new TestCaseData(H3.SetIndexDigit(H3.SetH3Index(1, 4, Direction.Center), 1, Direction.JAxes)).SetName("pentagon base cell 4 with digit 2 at res 1");
            yield return new TestCaseData(H3.SetIndexDigit(H3.SetIndexDigit(H3.SetH3Index(3, 14, Direction.Center), 2, Direction.JAxes), 3, Direction.KAxes)).SetName("pentagon base cell 14 with digit 1 after a non zero digit");
            yield return new TestCaseData(H3.SetH3Index(15, 117, Direction.Center)).SetName("pentagon base cell 117 all zeros at res 15");
            yield return new TestCaseData(H3.SetIndexDigit(H3.SetH3Index(1, 0, Direction.Center), 1, Direction.KAxes)).SetName("hexagon base cell 0 with digit 1 at res 1");
        }

        [TestCaseSource(nameof(ValidNeighboursOfInvalidValues))]
        public void NearMissesOfTheInvalidPatternsAreAccepted(ulong value)
        {
            Assert.That(H3.IsValidCell(value), Is.True);
            Assert.That(H3Index.TryCreate(value, out var index), Is.True);
            Assert.That(index.Value, Is.EqualTo(value));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("xyz")]
        [TestCase("0x8928308280fffff")]
        [TestCase("8928308280fffff junk")]
        [TestCase("8928308280fffffff")]
        [TestCase("-8928308280fffff")]
        [TestCase("89283 08280fffff")]
        [TestCase("8928308280fffff,")]
        public void MalformedStringsFailToParse(string text)
        {
            Assert.That(H3.StringToH3(text, out var value), Is.EqualTo(H3ErrorCode.Failed));
            Assert.That(value, Is.EqualTo(0UL));
            Assert.That(H3Index.TryParse(text, out var index), Is.False);
            Assert.That(index.Value, Is.EqualTo(0UL));
            var exception = Assert.Throws<H3Exception>(() => H3Index.Parse(text));
            Assert.That(exception.ErrorCode, Is.EqualTo(H3ErrorCode.Failed));
        }

        [TestCase("0")]
        [TestCase("ffffffffffffffff")]
        [TestCase("8928308280ffff7")]
        public void WellFormedHexThatIsNotACellParsesAsRawValueButNotAsIndex(string text)
        {
            Assert.That(H3.StringToH3(text, out var value), Is.EqualTo(H3ErrorCode.Success));
            Assert.That(H3.IsValidCell(value), Is.False);
            Assert.That(H3Index.TryParse(text, out _), Is.False);
            var exception = Assert.Throws<H3Exception>(() => H3Index.Parse(text));
            Assert.That(exception.ErrorCode, Is.EqualTo(H3ErrorCode.CellInvalid));
        }

        [Test]
        public void EqualityAndHashingFollowTheValue()
        {
            var a = new H3Index(Documented);
            var b = H3Index.Parse("8928308280fffff");
            var c = H3Index.Parse("85283473fffffff");
            Assert.That(a.Equals(b), Is.True);
            Assert.That(a.Equals((object)b), Is.True);
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
            Assert.That(a.Equals(c), Is.False);
            Assert.That(a != c, Is.True);
            Assert.That(a.Equals(null), Is.False);
            Assert.That(a.Equals("8928308280fffff"), Is.False);
        }

        [Test]
        public void ComparisonOrdersByValue()
        {
            var low = H3Index.Parse("85283473fffffff");
            var high = H3Index.Parse("8928308280fffff");
            Assert.That(low.Value, Is.LessThan(high.Value));
            Assert.That(low.CompareTo(high), Is.LessThan(0));
            Assert.That(high.CompareTo(low), Is.GreaterThan(0));
            Assert.That(low.CompareTo(low), Is.EqualTo(0));
            Assert.That(((IComparable)low).CompareTo(high), Is.LessThan(0));
            Assert.That(((IComparable)low).CompareTo(null), Is.GreaterThan(0));
            Assert.Throws<ArgumentException>(() => ((IComparable)low).CompareTo("text"));
            Assert.That(low < high, Is.True);
            Assert.That(low <= high, Is.True);
            Assert.That(high > low, Is.True);
            Assert.That(high >= low, Is.True);
            Assert.That(high < low, Is.False);
        }

        [Test]
        public void DefaultIndexIsTheNullIndexAndIsNotValid()
        {
            var index = default(H3Index);
            Assert.That(index.Value, Is.EqualTo(H3.H3Null));
            Assert.That(H3.IsValidCell(index.Value), Is.False);
        }

        [TestCase(0, 4, true)]
        [TestCase(0, 0, false)]
        [TestCase(5, 14, true)]
        [TestCase(15, 117, true)]
        public void PentagonAndClassIiiFlags(int resolution, int baseCell, bool pentagon)
        {
            var index = new H3Index(H3.SetH3Index(resolution, baseCell, Direction.Center));
            Assert.That(index.IsPentagon, Is.EqualTo(pentagon));
            Assert.That(H3.IsPentagon(index.Value), Is.EqualTo(pentagon));
            Assert.That(index.IsClassIII, Is.EqualTo(resolution % 2 == 1));
            Assert.That(H3.IsResClassIII(index.Value), Is.EqualTo(resolution % 2 == 1));
        }

        [Test]
        public void PentagonWithANonZeroDigitIsAHexagon()
        {
            var value = H3.SetIndexDigit(H3.SetH3Index(2, 4, Direction.Center), 2, Direction.JAxes);
            Assert.That(H3.IsValidCell(value), Is.True);
            Assert.That(H3.IsPentagon(value), Is.False);
        }

        [Test]
        public void ExceptionCarriesCodeAndDescription()
        {
            var exception = new H3Exception(H3ErrorCode.ResDomain);
            Assert.That(exception.ErrorCode, Is.EqualTo(H3ErrorCode.ResDomain));
            Assert.That(exception.Message, Is.EqualTo("Resolution argument was outside of acceptable range"));
            Assert.That(H3Exception.Describe(H3ErrorCode.Success), Is.EqualTo("Success"));
            Assert.That(H3Exception.Describe(H3ErrorCode.DeletedDigit), Is.EqualTo("Deleted subsequence indicates invalid index"));
            Assert.That(H3Exception.Describe((H3ErrorCode)99), Is.EqualTo("Invalid error code"));
            Assert.That((int)H3ErrorCode.DeletedDigit, Is.EqualTo(19));
        }
    }
}
