using System.Globalization;
using Ethar.GeoPose.Authority;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Exceptions;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.FrameSpecifications;
using Ethar.GeoPose.H3.Authority;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Ethar.GeoPose.H3.UnitTests
{
    /// <summary>
    /// The H3 cell frame specification and its authority: JSON in both directions, Advanced GeoPose round trips, validation and culture invariance.
    /// </summary>
    [TestFixture]
    internal class AuthorityTests
    {
        private const string Json = "{\"authority\":\"/Ethar.GeoPose.H3/1.0\",\"id\":\"H3-CELL\",\"parameters\":\"cell=8928308280fffff&heightInMeters=35.5\"}";

        private static readonly H3Index Cell = H3Index.Parse("8928308280fffff");

        [OneTimeSetUp]
        public void RegisterAuthority()
        {
            EtharGeoPoseH3Authority.Register();
        }

        [OneTimeTearDown]
        public void UnregisterAuthority()
        {
            EtharGeoPoseH3Authority.Unregister();
        }

        [Test]
        public void AuthorityIsRegisteredUnderItsName()
        {
            var authority = AuthorityProvider.GetAuthority(H3AuthorityConstants.AuthorityName);
            Assert.That(authority, Is.InstanceOf<EtharGeoPoseH3Authority>());
            Assert.That(authority.AuthorityName, Is.EqualTo("/Ethar.GeoPose.H3/1.0"));
        }

        [Test]
        public void SpecificationSerializesToTheDocumentedJson()
        {
            var spec = new H3CellSpecification(Cell, 35.5);
            Assert.That(spec.Authority, Is.EqualTo("/Ethar.GeoPose.H3/1.0"));
            Assert.That(spec.Id, Is.EqualTo("H3-CELL"));
            Assert.That(spec.BuildParameters(), Is.EqualTo("cell=8928308280fffff&heightInMeters=35.5"));
            Assert.That(JsonConvert.SerializeObject(spec), Is.EqualTo(Json));
        }

        [Test]
        public void SpecificationDeserializesAndResolvesItsPosition()
        {
            var spec = JsonConvert.DeserializeObject<H3CellSpecification>(Json);
            Assert.That(spec.Cell, Is.EqualTo(Cell));
            Assert.That(spec.HeightInMeters, Is.EqualTo(35.5));
            Assert.That(spec.Position, Is.EqualTo(Cell.ToTangentPointPosition(35.5)));
            Assert.That(spec.Position.HeightInMeters, Is.EqualTo(35.5));
            Assert.That(spec, Is.EqualTo(new H3CellSpecification(Cell, 35.5)));
            Assert.That(spec.GetHashCode(), Is.EqualTo(new H3CellSpecification(Cell, 35.5).GetHashCode()));
            Assert.That(spec.ToString(), Does.Contain("Cell:8928308280fffff"));

            var uppercase = JsonConvert.DeserializeObject<H3CellSpecification>(Json.Replace("8928308280fffff", "8928308280FFFFF"));
            Assert.That(uppercase.Cell, Is.EqualTo(Cell));
            var reordered = JsonConvert.DeserializeObject<H3CellSpecification>("{\"id\":\"H3-CELL\",\"parameters\":\"heightInMeters=-12&cell=8928308280fffff\",\"authority\":\"/Ethar.GeoPose.H3/1.0\"}");
            Assert.That(reordered.HeightInMeters, Is.EqualTo(-12));
        }

        [Test]
        public void AdvancedGeoPoseRoundTripsWithAnH3CellFrame()
        {
            var quaternion = new UnitQuaternion(0.20056154657066608, -0.08111602541464237, 0.36606032744426537, -0.9050939692261301);
            var sdu = new AdvancedSdu(16534234327, quaternion, new H3CellSpecification(Cell, 35.5));
            var json = JsonConvert.SerializeObject(sdu);
            Assert.That(json, Does.Contain("\"authority\":\"/Ethar.GeoPose.H3/1.0\""));
            Assert.That(json, Does.Contain("cell=8928308280fffff&heightInMeters=35.5"));
            var back = JsonConvert.DeserializeObject<AdvancedSdu>(json);
            Assert.That(back.ValidTime, Is.EqualTo(16534234327));
            Assert.That(back.Quaternion, Is.EqualTo(quaternion));
            Assert.That(back.FrameSpecification, Is.InstanceOf<H3CellSpecification>());
            Assert.That(back.FrameSpecification, Is.EqualTo(sdu.FrameSpecification));
            Assert.That(((H3CellSpecification)back.FrameSpecification).Position.Latitude, Is.EqualTo(37.77670234943567).Within(1e-9));
            Assert.That(AuthorityProvider.GetAuthority(H3AuthorityConstants.AuthorityName).IsFrameSpecificationExtrinsic(back.FrameSpecification), Is.True);
        }

        [Test]
        public void InvalidParametersAreRejected()
        {
            Assert.Throws<FrameSpecificationInvalidException>(() => JsonConvert.DeserializeObject<H3CellSpecification>(Json.Replace("8928308280fffff", "not a cell")));
            Assert.Throws<FrameSpecificationInvalidException>(() => JsonConvert.DeserializeObject<H3CellSpecification>(Json.Replace("8928308280fffff", "8928308280ffff7")));
            Assert.Throws<FrameSpecificationInvalidException>(() => JsonConvert.DeserializeObject<H3CellSpecification>(Json.Replace("&heightInMeters=35.5", string.Empty)));
            Assert.Throws<FrameSpecificationInvalidException>(() => JsonConvert.DeserializeObject<H3CellSpecification>(Json.Replace("35.5", "35,5")));
            Assert.Throws<FrameSpecificationInvalidException>(() => JsonConvert.DeserializeObject<H3CellSpecification>(Json.Replace("H3-CELL", "LTP-ENU")));
            Assert.Throws<AuthorityNotSupportedException>(() => new EtharGeoPoseH3Authority().ConvertJsonToFrameSpec(JObject.Parse(Json.Replace("/Ethar.GeoPose.H3/1.0", "/Ethar.GeoPose/1.0"))));
        }

        [Test]
        public void TransitionModelsAndForeignFramesAreNotHandled()
        {
            var authority = new EtharGeoPoseH3Authority();
            Assert.Throws<TransitionModelInvalidException>(() => authority.ConvertJsonToTransitionModel(new JObject()));
            Assert.Throws<TransitionModelInvalidException>(() => authority.ConvertTransitionModelToJson(null));
            Assert.Throws<NotImplementedException>(() => authority.ConvertFrameSpecToJson(new OtherSpecification()));
            Assert.That(authority.IsFrameSpecificationExtrinsic(new OtherSpecification()), Is.False);
        }

        [TestCaseSource(typeof(CultureInvarianceTests), nameof(CultureInvarianceTests.Cultures))]
        public void JsonIsCultureInvariant(string cultureName)
        {
            var original = CultureInfo.CurrentCulture;
            try
            {
                var culture = cultureName.Length == 0 ? CultureInfo.InvariantCulture : new CultureInfo(cultureName);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                var spec = new H3CellSpecification(Cell, -1234.5678);
                var json = JsonConvert.SerializeObject(spec);
                Assert.That(json, Does.Contain("heightInMeters=-1234.5678"), cultureName);
                var back = JsonConvert.DeserializeObject<H3CellSpecification>(json);
                Assert.That(back, Is.EqualTo(spec), cultureName);
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
                CultureInfo.CurrentUICulture = original;
            }
        }

        private sealed class OtherSpecification : BaseFrameSpecification
        {
            public OtherSpecification()
                : base("OTHER", H3AuthorityConstants.AuthorityName)
            {
            }
        }
    }
}
