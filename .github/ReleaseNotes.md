# Ethar.GeoPose 2.0.0

Breaking change: every coordinate, angle and quaternion value in the public API is now a double, not a float. Code that assigns these values to a float no longer compiles, and assemblies built against 1.0.0 must be rebuilt. The JSON property names are unchanged.

## New

* Conventions namespace: LeftHandedYUpFrame and LeftHandedYUpConversions place GeoPose East, North, Up poses in a left-handed, Y-up engine frame such as Unity. Also ForwardAxis, EnginePose, EulerConvention, CompassHeading, OrientationConversions and Angles.
* Geodesy namespace: GeodeticConverter (WGS 84 geodetic, ECEF and ENU conversions), EcefPosition, EnuVector, NedVector and Wgs84.
* TangentPointPosition extensions: ToEcef, EnuOffsetFrom, OffsetBy, DistanceTo and BearingTo. DistanceTo and BearingTo use a spherical earth model, and their XML docs state the accuracy.
* OgcGeoPoseAuthority and the OGC frame specifications and transition models in Ethar.GeoPose.Authority.
* InvariantNumber, so JSON numbers read and write the same in every culture.
* Ethar.GeoPose.H3, a new add-on package that converts between GeoPose positions and Uber H3 cell indexes. Ethar.GeoPose and Ethar.GeoPose.Authority do not depend on it.

## Conventions to know

* An unrotated GeoPose faces East, not North. Unity content authored with +Z forward should use ForwardAxis.EngineZ.
* Heights are ellipsoidal heights above WGS 84 in meters, not heights above sea level.

## Unity package com.ethar.geopose

* Ships the netstandard2.1 builds of Ethar.GeoPose, Ethar.GeoPose.Authority and Ethar.GeoPose.H3, all auto-referenced.
* Requires Unity 2022.3 or later.
