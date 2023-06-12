using Ethar.GeoPose.DataTypes;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    [TestFixture]
    public class FrameTransformIndexPairJsonConverterUnitTests
    {
        [Test]
        public void WriteJson_CorrectlyConvertsFrameTransformIndexPair()
        {
            var pair = new FrameTransformIndexPair()
            {
                OuterFrameIndex = 1,
                InnerFrameIndex = 2
            };

            var json = JsonConvert.SerializeObject(pair);

            var expected = "{" +
                "\"link\":[" +
                    "1," +
                    "2" +
                "]" +
                "}";

            Assert.That(json, Is.EqualTo(expected));
        }

        [Test]
        public void ReadJson_CorrectlyConvertsFrameTransformIndexPair()
        {
            var json = "{" +
                "\"link\":[" +
                    "1," +
                    "2" +
                "]" +
                "}";

            var pair = JsonConvert.DeserializeObject<FrameTransformIndexPair>(json);

            Assert.That(pair.OuterFrameIndex, Is.EqualTo(1));
            Assert.That(pair.InnerFrameIndex, Is.EqualTo(2));
        }

        [Test]
        public void CanCorrectlyMakeARoundTripConversion()
        {
            var pair = new FrameTransformIndexPair()
            {
                OuterFrameIndex = 1,
                InnerFrameIndex = 2
            };

            var json = JsonConvert.SerializeObject(pair);
            var converted = JsonConvert.DeserializeObject<FrameTransformIndexPair>(json);

            Assert.That(converted, Is.EqualTo(pair));
        }

        [Test]
        public void CanConvertFrameTransformIndexPairToString()
        {
            var json = "{" +
                "\"link\":[" +
                    "1," +
                    "2" +
                "]" +
                "}";

            var pair = JsonConvert.DeserializeObject<FrameTransformIndexPair>(json);
            var result = pair.ToString();
            Assert.That(result, !Is.Empty);
            Assert.That(result, Is.EqualTo("OuterFrameIndex:1, InnerFrameIndex:2"));
        }
    }
}
