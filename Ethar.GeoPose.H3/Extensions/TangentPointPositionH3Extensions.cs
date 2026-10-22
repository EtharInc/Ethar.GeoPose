// <copyright file="TangentPointPositionH3Extensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.Conventions;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.H3;

    /// <summary>
    /// H3 cell indexing for a GeoPose <see cref="TangentPointPosition"/>. Latitude and longitude are converted from degrees to the radians the engine uses; the height is ignored because H3 is two dimensional.
    /// </summary>
    public static class TangentPointPositionH3Extensions
    {
        /// <summary>
        /// Returns the H3 cell containing the position at the given resolution.
        /// </summary>
        /// <param name="position">The position in degrees. The height is ignored.</param>
        /// <param name="resolution">The H3 resolution, 0 (coarsest, about 1,280 km edges) to 15 (finest, about 0.5 m edges).</param>
        /// <returns>The containing cell.</returns>
        /// <exception cref="H3Exception">Thrown with <see cref="H3ErrorCode.LatLngDomain"/> if the latitude or longitude is outside the GeoPose range, or <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15.</exception>
        public static H3Index ToH3Cell(this TangentPointPosition position, int resolution)
        {
            var validation = position.Validate();
            if (!validation.IsValid)
            {
                throw new H3Exception(H3ErrorCode.LatLngDomain, validation.Message);
            }

            return H3Index.FromLatLng(position.ToH3LatLng(), resolution);
        }

        /// <summary>
        /// Returns the H3 cell containing the position at every resolution, indexed by resolution 0 to 15, each one projected directly.
        /// </summary>
        /// <remarks>
        /// This performs 16 projections and gives the exact H3 cell at every level. The cheaper alternative, <c>position.ToH3Cell(15).Ancestors()</c>, derives the coarser cells from the finest one by the index hierarchy.
        /// The two differ for points near cell edges because H3 cells only approximately nest; see the package readme for the measured rate.
        /// </remarks>
        /// <param name="position">The position in degrees. The height is ignored.</param>
        /// <returns>An array of 16 cells where element r is the cell at resolution r.</returns>
        public static H3Index[] ToH3Cells(this TangentPointPosition position)
        {
            var validation = position.Validate();
            if (!validation.IsValid)
            {
                throw new H3Exception(H3ErrorCode.LatLngDomain, validation.Message);
            }

            var point = position.ToH3LatLng();
            var cells = new H3Index[16];
            for (var resolution = 0; resolution < cells.Length; resolution++)
            {
                cells[resolution] = H3Index.FromLatLng(point, resolution);
            }

            return cells;
        }

        /// <summary>
        /// Returns true if the position lies in the given H3 cell, that is if indexing the position at the cell's resolution produces the cell.
        /// </summary>
        /// <param name="position">The position in degrees. The height is ignored.</param>
        /// <param name="cell">The cell at any resolution.</param>
        /// <returns>True if the position is in the cell.</returns>
        public static bool IsInH3Cell(this TangentPointPosition position, H3Index cell)
        {
            return position.ToH3Cell(cell.Resolution) == cell;
        }

        /// <summary>
        /// Converts the position to the radians form the H3 engine uses. The height is dropped.
        /// </summary>
        /// <param name="position">The position in degrees.</param>
        /// <returns>The latitude and longitude in radians.</returns>
        public static LatLng ToH3LatLng(this TangentPointPosition position)
        {
            return new LatLng(Angles.DegreesToRadians(position.Latitude), Angles.DegreesToRadians(position.Longitude));
        }
    }
}
