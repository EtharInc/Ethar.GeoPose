// <copyright file="H3IndexGeoPoseExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.Conventions;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Geodesy;
    using Ethar.GeoPose.H3;
    using Ethar.GeoPose.StructuralDataUnits;

    /// <summary>
    /// GeoPose positions and poses built from an H3 cell. The height is always supplied by the caller because a cell is two dimensional and the datum choice must be explicit.
    /// </summary>
    public static class H3IndexGeoPoseExtensions
    {
        /// <summary>
        /// Returns the centre of the cell as a GeoPose position in degrees.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="heightInMeters">The height above the WGS84 ellipsoid to give the position.</param>
        /// <returns>The cell centre.</returns>
        public static TangentPointPosition ToTangentPointPosition(this H3Index cell, double heightInMeters)
        {
            return cell.Center.ToTangentPointPosition(heightInMeters);
        }

        /// <summary>
        /// Returns the outline of the cell as GeoPose positions in degrees.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="heightInMeters">The height above the WGS84 ellipsoid to give every vertex.</param>
        /// <returns>The boundary, 5 to 10 vertices counter-clockwise.</returns>
        public static H3CellBoundary ToCellBoundary(this H3Index cell, double heightInMeters)
        {
            return new H3CellBoundary(cell.Boundary(), heightInMeters);
        }

        /// <summary>
        /// Builds a Basic-YPR GeoPose whose position is the cell centre.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="heightInMeters">The height above the WGS84 ellipsoid of the position.</param>
        /// <param name="angles">The orientation.</param>
        /// <returns>The GeoPose.</returns>
        public static BasicYawPitchRollSdu ToBasicYawPitchRollSdu(this H3Index cell, double heightInMeters, YawPitchRollAngles angles)
        {
            return new BasicYawPitchRollSdu(angles, cell.ToTangentPointPosition(heightInMeters));
        }

        /// <summary>
        /// Builds a Basic-Quaternion GeoPose whose position is the cell centre.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="heightInMeters">The height above the WGS84 ellipsoid of the position.</param>
        /// <param name="quaternion">The orientation.</param>
        /// <returns>The GeoPose.</returns>
        public static BasicQuaternionSdu ToBasicQuaternionSdu(this H3Index cell, double heightInMeters, UnitQuaternion quaternion)
        {
            return new BasicQuaternionSdu(cell.ToTangentPointPosition(heightInMeters), quaternion);
        }

        /// <summary>
        /// Returns the distance in metres between the centres of two cells, using the GeoPose geodesy rather than the H3 sphere.
        /// </summary>
        /// <param name="cell">The first cell.</param>
        /// <param name="other">The second cell.</param>
        /// <returns>The great circle distance in metres.</returns>
        public static double DistanceTo(this H3Index cell, H3Index other)
        {
            return GeodeticConverter.GreatCircleDistance(cell.ToTangentPointPosition(0), other.ToTangentPointPosition(0));
        }

        /// <summary>
        /// Converts engine coordinates in radians to a GeoPose position in degrees.
        /// </summary>
        /// <param name="point">The coordinates in radians.</param>
        /// <param name="heightInMeters">The height above the WGS84 ellipsoid to give the position.</param>
        /// <returns>The position.</returns>
        public static TangentPointPosition ToTangentPointPosition(this LatLng point, double heightInMeters)
        {
            return new TangentPointPosition(Angles.RadiansToDegrees(point.Lat), Angles.RadiansToDegrees(point.Lng), heightInMeters);
        }
    }
}
