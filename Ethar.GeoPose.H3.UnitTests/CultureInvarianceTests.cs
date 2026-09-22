using System.Globalization;
using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// Proves that <see cref="H3Index.Parse"/> and <see cref="H3Index.ToString"/> do not depend on the thread culture.
    /// The locale list matches the core CultureInvarianceTests: period-decimal, comma-decimal, Arabic-comma and non-Latin-digit locales, and the invariant culture.
    /// </summary>
    [TestFixture]
    internal class CultureInvarianceTests
    {
        public static readonly string[] Cultures =
        {
            "de-DE", "fr-FR", "es-ES", "it-IT", "nl-NL", "pt-BR", "ru-RU", "tr-TR", "sv-SE", "pl-PL", "cs-CZ",
            "ar-SA", "fa-IR", "hi-IN", "ja-JP", "zh-CN", "en-US", "en-GB", "",
        };

        private CultureInfo originalCulture;
        private CultureInfo originalUiCulture;

        [SetUp]
        public void RememberCulture()
        {
            this.originalCulture = CultureInfo.CurrentCulture;
            this.originalUiCulture = CultureInfo.CurrentUICulture;
        }

        [TearDown]
        public void RestoreCulture()
        {
            CultureInfo.CurrentCulture = this.originalCulture;
            CultureInfo.CurrentUICulture = this.originalUiCulture;
        }

        [Test]
        public void NineteenCulturesAreCovered()
        {
            Assert.That(Cultures, Has.Length.EqualTo(19));
        }

        [TestCaseSource(nameof(Cultures))]
        public void ParseAndToStringAreInvariant(string cultureName)
        {
            SetCulture(cultureName);
            foreach (var hex in FixtureFiles.ReadIndexes("res01cells.txt"))
            {
                var index = H3Index.Parse(hex);
                Assert.That(index.ToString(), Is.EqualTo(hex), cultureName);
                Assert.That(H3Index.Parse(hex.ToUpperInvariant()), Is.EqualTo(index), cultureName);
                Assert.That(H3.H3ToString(index.Value), Is.EqualTo(hex), cultureName);
            }
        }

        [TestCaseSource(nameof(Cultures))]
        public void TurkishAndUppercaseDoNotChangeTheOutcome(string cultureName)
        {
            SetCulture(cultureName);
            Assert.That(H3Index.Parse("85283473FFFFFFF").ToString(), Is.EqualTo("85283473fffffff"), cultureName);
            Assert.That(H3Index.TryParse("8928308280FFFFF", out var index), Is.True, cultureName);
            Assert.That(index.Resolution, Is.EqualTo(9), cultureName);
            Assert.That(H3Index.TryParse("not hex", out _), Is.False, cultureName);
        }

        private static void SetCulture(string name)
        {
            var culture = name.Length == 0 ? CultureInfo.InvariantCulture : new CultureInfo(name);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
