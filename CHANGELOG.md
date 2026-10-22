# Change Log

The format is based on [Keep a Changelog](http://keepachangelog.com/) and this project adheres to [Semantic Versioning](http://semver.org/).

## [2.0.0] - 2026-10-22

A breaking release. See the [migration guide](https://etharinc.github.io/Ethar.GeoPose.Docs/articles/migration-v2.html) and the [conventions page](https://etharinc.github.io/Ethar.GeoPose.Docs/articles/conventions.html).

### Changed

* Every coordinate, angle and quaternion value in the public API is now a double, not a float. Code that assigns these values to a float no longer compiles. The JSON property names are unchanged.
* The package ships the netstandard2.1 builds of its assemblies instead of net48.
* The minimum Unity version is now 2022.3.
* Frame specification parameter strings are written and parsed with the invariant culture.

### Added

* Conventions namespace: LeftHandedYUpFrame, LeftHandedYUpConversions, ForwardAxis, EnginePose, EulerConvention, CompassHeading, OrientationConversions and Angles. An unrotated GeoPose faces East, not North. Unity content authored with +Z forward should use ForwardAxis.EngineZ.
* Geodesy namespace: GeodeticConverter, EcefPosition, EnuVector, NedVector and Wgs84, plus TangentPointPosition extensions including DistanceTo and BearingTo.
* OgcGeoPoseAuthority for the OGC example instances, and InvariantNumber.
* Ethar.GeoPose.H3, an add-on assembly that converts between GeoPose positions and Uber H3 cell indexes, with its LICENSE and NOTICE.
* The Tests sample adds NumericPrecisionTests, CultureInvarianceTests and the H3 tests with their fixture files.

## [Initial Release] - 2023-06-12

The initial release for the Ethar GeoPose library, including:

* Base GeoPose implementation based on [V1.0 of the OGC GeoPose Specification](https://docs.ogc.org/dis/21-056r10/21-056r10.html)
* Ethar's base Authority implementation, providing the GeoPose Standards main Frame Specifications.
* Two Example class demonstrations.
* Unit Tests for both the GeoPose library and the Ethar Authority.
* Initial documentation and API provided at [https://etharinc.github.io/Ethar.GeoPose.Docs](https://etharinc.github.io/Ethar.GeoPose.Docs/index.html)
* NPM distribution for the C# implementations of GeoPose and Authority.
* Starter Unity UPM implementations published via OpenUPM.
