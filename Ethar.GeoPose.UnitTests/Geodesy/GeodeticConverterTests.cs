using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.Extensions;
using Ethar.GeoPose.Geodesy;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests.Geodesy
{
    [TestFixture]
    internal class GeodeticConverterTests
    {
        private static readonly TangentPointPosition London = new TangentPointPosition(51.5074, -0.1278, 0);
        private static readonly TangentPointPosition Paris = new TangentPointPosition(48.8566, 2.3522, 0);
        private static readonly TangentPointPosition Huntsville = new TangentPointPosition(34.7304, -86.5861, 190);

        [Test]
        public void EquatorPrimeMeridianIsOnTheXAxis()
        {
            var ecef = GeodeticConverter.GeodeticToEcef(new TangentPointPosition(0, 0, 0));

            Assert.That(ecef.X, Is.EqualTo(Wgs84.SemiMajorAxis).Within(1e-6));
            Assert.That(ecef.Y, Is.EqualTo(0).Within(1e-6));
            Assert.That(ecef.Z, Is.EqualTo(0).Within(1e-6));
        }

        [Test]
        public void NinetyDegreesEastIsOnTheYAxis()
        {
            var ecef = GeodeticConverter.GeodeticToEcef(new TangentPointPosition(0, 90, 0));

            Assert.That(ecef.X, Is.EqualTo(0).Within(1e-6));
            Assert.That(ecef.Y, Is.EqualTo(Wgs84.SemiMajorAxis).Within(1e-6));
            Assert.That(ecef.Z, Is.EqualTo(0).Within(1e-6));
        }

        [Test]
        public void NorthPoleIsOnTheZAxisAtTheSemiMinorAxis()
        {
            var ecef = GeodeticConverter.GeodeticToEcef(new TangentPointPosition(90, 0, 0));

            Assert.That(ecef.X, Is.EqualTo(0).Within(1e-6));
            Assert.That(ecef.Y, Is.EqualTo(0).Within(1e-6));
            Assert.That(ecef.Z, Is.EqualTo(Wgs84.SemiMinorAxis).Within(1e-6));
        }

        [Test]
        public void HeightMovesAlongTheNormal()
        {
            var ecef = GeodeticConverter.GeodeticToEcef(new TangentPointPosition(0, 0, 100));

            Assert.That(ecef.X, Is.EqualTo(Wgs84.SemiMajorAxis + 100).Within(1e-6));
        }

        [TestCase(51.5074, -0.1278, 0)]
        [TestCase(48.8566, 2.3522, 35)]
        [TestCase(34.7304, -86.5861, 190)]
        [TestCase(-33.8688, 151.2093, 58)]
        [TestCase(89.9, 45, 10)]
        [TestCase(-89.9, -135, -10)]
        [TestCase(0.5, 179.9, 0)]
        public void GeodeticEcefRoundTripIsExact(double lat, double lon, double h)
        {
            var original = new TangentPointPosition(lat, lon, h);

            var back = GeodeticConverter.EcefToGeodetic(GeodeticConverter.GeodeticToEcef(original));

            Assert.That(back.Latitude, Is.EqualTo(lat).Within(1e-9));
            Assert.That(back.Longitude, Is.EqualTo(lon).Within(1e-9));
            Assert.That(back.HeightInMeters, Is.EqualTo(h).Within(1e-6));
        }

        [Test]
        public void PoleConvertsBackToGeodetic()
        {
            var back = GeodeticConverter.EcefToGeodetic(new EcefPosition(0, 0, Wgs84.SemiMinorAxis + 50));

            Assert.That(back.Latitude, Is.EqualTo(90).Within(1e-9));
            Assert.That(back.HeightInMeters, Is.EqualTo(50).Within(1e-6));
        }

        [Test]
        public void OneThousandthOfADegreeNorthAtTheEquatorIsAboutOneHundredAndTenMetres()
        {
            var origin = new TangentPointPosition(0, 0, 0);
            var point = new TangentPointPosition(0.001, 0, 0);

            var enu = GeodeticConverter.GeodeticToEnu(point, origin);

            Assert.That(enu.North, Is.EqualTo(110.574).Within(0.05));
            Assert.That(enu.East, Is.EqualTo(0).Within(1e-6));
            Assert.That(enu.Up, Is.EqualTo(0).Within(0.01));
        }

        [Test]
        public void OneThousandthOfADegreeEastAtTheEquatorIsAboutOneHundredAndElevenMetres()
        {
            var origin = new TangentPointPosition(0, 0, 0);
            var point = new TangentPointPosition(0, 0.001, 0);

            var enu = GeodeticConverter.GeodeticToEnu(point, origin);

            Assert.That(enu.East, Is.EqualTo(111.319).Within(0.05));
            Assert.That(enu.North, Is.EqualTo(0).Within(1e-6));
        }

        [Test]
        public void HeightAboveTheOriginIsUp()
        {
            var enu = GeodeticConverter.GeodeticToEnu(new TangentPointPosition(51.5074, -0.1278, 25), London);

            Assert.That(enu.Up, Is.EqualTo(25).Within(1e-6));
            Assert.That(enu.HorizontalLength, Is.EqualTo(0).Within(1e-6));
        }

        [Test]
        public void EastOfTheOriginInTheWesternHemisphereIsPositiveEast()
        {
            var point = new TangentPointPosition(Huntsville.Latitude, Huntsville.Longitude + 0.001, Huntsville.HeightInMeters);

            var enu = GeodeticConverter.GeodeticToEnu(point, Huntsville);

            Assert.That(enu.East, Is.GreaterThan(90));
            Assert.That(enu.North, Is.EqualTo(0).Within(1e-3));
        }

        [TestCase(10, 20, 30)]
        [TestCase(-1500, 2500, -12)]
        [TestCase(0.001, -0.002, 0)]
        public void EnuRoundTripIsExact(double east, double north, double up)
        {
            var displacement = new EnuVector(east, north, up);

            var position = GeodeticConverter.EnuToGeodetic(displacement, Paris);
            var back = GeodeticConverter.GeodeticToEnu(position, Paris);

            Assert.That(back.East, Is.EqualTo(east).Within(1e-6));
            Assert.That(back.North, Is.EqualTo(north).Within(1e-6));
            Assert.That(back.Up, Is.EqualTo(up).Within(1e-6));
        }

        [Test]
        public void LondonToParisIsAboutThreeHundredAndFortyFourKilometres()
        {
            var distance = GeodeticConverter.GreatCircleDistance(London, Paris);

            Assert.That(distance, Is.EqualTo(343_500).Within(2_000));
        }

        [Test]
        public void LondonToParisBearsSouthEast()
        {
            var bearing = GeodeticConverter.InitialBearing(London, Paris);

            Assert.That(bearing, Is.EqualTo(148).Within(1));
        }

        [TestCase(0, 1, 90)]
        [TestCase(1, 0, 0)]
        [TestCase(-1, 0, 180)]
        [TestCase(0, -1, 270)]
        public void CardinalBearingsFromTheOrigin(double lat, double lon, double expected)
        {
            var bearing = GeodeticConverter.InitialBearing(new TangentPointPosition(0, 0, 0), new TangentPointPosition(lat, lon, 0));

            Assert.That(bearing, Is.EqualTo(expected).Within(1e-9));
        }

        [Test]
        public void ExtensionsDelegateToTheConverter()
        {
            var displacement = new EnuVector(5, -7, 2);

            Assert.That(London.ToEcef(), Is.EqualTo(GeodeticConverter.GeodeticToEcef(London)));
            Assert.That(Paris.EnuOffsetFrom(London), Is.EqualTo(GeodeticConverter.GeodeticToEnu(Paris, London)));
            Assert.That(London.OffsetBy(displacement), Is.EqualTo(GeodeticConverter.EnuToGeodetic(displacement, London)));
            Assert.That(London.DistanceTo(Paris), Is.EqualTo(GeodeticConverter.GreatCircleDistance(London, Paris)));
            Assert.That(London.BearingTo(Paris), Is.EqualTo(GeodeticConverter.InitialBearing(London, Paris)));
        }

        [Test]
        public void EnuAndNedConvertBothWays()
        {
            var enu = new EnuVector(1, 2, 3);

            var ned = enu.ToNed();

            Assert.That(ned, Is.EqualTo(new NedVector(2, 1, -3)));
            Assert.That(ned.ToEnu(), Is.EqualTo(enu));
        }
    }
}
