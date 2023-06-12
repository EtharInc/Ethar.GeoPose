using Ethar.GeoPose.Authority.TransitionModels;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethar.GeoPose.Authority.UnitTests
{
    [TestFixture]
    internal class TransitionModelJsonConverterTests : UnitTestBase
    {
        [TestCase("/Ethar.GeoPose/1.0", "none")]
        public void CanCorrectlyDeserializeNoneTransitionModel(string authority, string id)
        {
            var json =
                "{" +
                    $"\"authority\":\"{authority}\"," +
                    $"\"id\":\"{id}\"," +
                    $"\"parameters\":\"\"" +
                "}";

            var transitionModel = JsonConvert.DeserializeObject<NoneTransitionModel>(json);
            Assert.IsFalse(transitionModel is null);
        }

        [TestCase("/Ethar.GeoPose/1.0", "none")]
        public void CanCorrectlySerializeNoneTransitionModel(string authority, string id)
        {
            var transitionModel = new NoneTransitionModel();
            var expected =
                "{" +
                    $"\"authority\":\"{authority}\"," +
                    $"\"id\":\"{id}\"," +
                    $"\"parameters\":\"\"" +
                "}";

            var json = JsonConvert.SerializeObject(transitionModel);
            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase("/Ethar.GeoPose/1.0", "none")]
        public void CanCorrectlyMakeARoundTripConversionForNoneTransitionModel(string authority, string id)
        {
            var transitionModel = new NoneTransitionModel();

            var json = JsonConvert.SerializeObject(transitionModel);
            var converted = JsonConvert.DeserializeObject<NoneTransitionModel>(json);

            Assert.That(converted, Is.EqualTo(transitionModel));
        }

        [TestCase("/Ethar.GeoPose/1.0", "interpolated")]
        public void CanCorrectlyDeserializeInterpolatedTransitionModel(string authority, string id)
        {
            var json =
                "{" +
                    $"\"authority\":\"{authority}\"," +
                    $"\"id\":\"{id}\"," +
                    $"\"parameters\":\"\"" +
                "}";

            var transitionModel = JsonConvert.DeserializeObject<InterpolatedTransitionModel>(json);
            Assert.That(transitionModel is null, Is.False);
        }

        [TestCase("/Ethar.GeoPose/1.0", "interpolated")]
        public void CanCorrectlySerializeInterpolatedTransitionModel(string authority, string id)
        {
            var transitionModel = new InterpolatedTransitionModel();
            var expected =
                "{" +
                    $"\"authority\":\"{authority}\"," +
                    $"\"id\":\"{id}\"," +
                    $"\"parameters\":\"\"" +
                "}";

            var json = JsonConvert.SerializeObject(transitionModel);
            Assert.That(json, Is.EqualTo(expected));
        }

        [TestCase("/Ethar.GeoPose/1.0", "interpolated")]
        public void CanCorrectlyMakeARoundTripConversionForInterpolatedTransitionModel(string authority, string id)
        {
            var transitionModel = new InterpolatedTransitionModel();

            var json = JsonConvert.SerializeObject(transitionModel);
            var converted = JsonConvert.DeserializeObject<InterpolatedTransitionModel>(json);

            Assert.That(converted, Is.EqualTo(transitionModel));
        }
    }
}
