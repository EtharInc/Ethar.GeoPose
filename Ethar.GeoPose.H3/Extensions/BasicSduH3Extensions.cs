// <copyright file="BasicSduH3Extensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.H3;
    using Ethar.GeoPose.StructuralDataUnits;

    /// <summary>
    /// H3 cell indexing for the Basic GeoPose structural data units, which carry a single position.
    /// </summary>
    public static class BasicSduH3Extensions
    {
        /// <summary>
        /// Returns the H3 cell containing the GeoPose position at the given resolution.
        /// </summary>
        /// <param name="sdu">The GeoPose.</param>
        /// <param name="resolution">The H3 resolution, 0 to 15.</param>
        /// <returns>The containing cell.</returns>
        public static H3Index ToH3Cell(this BasicYawPitchRollSdu sdu, int resolution)
        {
            return sdu.Position.ToH3Cell(resolution);
        }

        /// <summary>
        /// Returns the H3 cell containing the GeoPose position at the given resolution.
        /// </summary>
        /// <param name="sdu">The GeoPose.</param>
        /// <param name="resolution">The H3 resolution, 0 to 15.</param>
        /// <returns>The containing cell.</returns>
        public static H3Index ToH3Cell(this BasicQuaternionSdu sdu, int resolution)
        {
            return sdu.Position.ToH3Cell(resolution);
        }

        /// <summary>
        /// Returns true if the GeoPose position lies in the given H3 cell.
        /// </summary>
        /// <param name="sdu">The GeoPose.</param>
        /// <param name="cell">The cell at any resolution.</param>
        /// <returns>True if the position is in the cell.</returns>
        public static bool IsInH3Cell(this BasicYawPitchRollSdu sdu, H3Index cell)
        {
            return sdu.Position.IsInH3Cell(cell);
        }

        /// <summary>
        /// Returns true if the GeoPose position lies in the given H3 cell.
        /// </summary>
        /// <param name="sdu">The GeoPose.</param>
        /// <param name="cell">The cell at any resolution.</param>
        /// <returns>True if the position is in the cell.</returns>
        public static bool IsInH3Cell(this BasicQuaternionSdu sdu, H3Index cell)
        {
            return sdu.Position.IsInH3Cell(cell);
        }
    }
}
