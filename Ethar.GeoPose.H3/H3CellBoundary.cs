// <copyright file="H3CellBoundary.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.H3
{
    using System;
    using Ethar.GeoPose.Conventions;
    using Ethar.GeoPose.DataTypes;

    /// <summary>
    /// The outline of an H3 cell as GeoPose positions in degrees, counter-clockwise, all at one caller supplied height.
    /// </summary>
    /// <remarks>
    /// A hexagon has 6 vertices and a pentagon 5, but a cell that straddles an icosahedron edge gains one extra vertex per crossing, so the count can reach 10.
    /// The vertices are exposed through <see cref="Count"/> and the indexer rather than a list to keep allocation low.
    /// </remarks>
    public sealed class H3CellBoundary
    {
        private readonly TangentPointPosition[] vertices;

        /// <summary>
        /// Initializes a new instance of the <see cref="H3CellBoundary"/> class from an engine boundary in radians.
        /// </summary>
        /// <param name="boundary">The boundary in radians.</param>
        /// <param name="heightInMeters">The height above the WGS84 ellipsoid to give every vertex.</param>
        public H3CellBoundary(CellBoundary boundary, double heightInMeters)
        {
            if (boundary == null)
            {
                throw new ArgumentNullException(nameof(boundary));
            }

            this.HeightInMeters = heightInMeters;
            this.vertices = new TangentPointPosition[boundary.Count];
            for (var i = 0; i < this.vertices.Length; i++)
            {
                var vertex = boundary[i];
                this.vertices[i] = new TangentPointPosition(Angles.RadiansToDegrees(vertex.Lat), Angles.RadiansToDegrees(vertex.Lng), heightInMeters);
            }
        }

        /// <summary>
        /// Gets the number of vertices, 5 to 10.
        /// </summary>
        public int Count => this.vertices.Length;

        /// <summary>
        /// Gets the height above the WGS84 ellipsoid shared by every vertex.
        /// </summary>
        public double HeightInMeters { get; }

        /// <summary>
        /// Gets the vertex at the given position.
        /// </summary>
        /// <param name="index">The vertex position, 0 to <see cref="Count"/> - 1.</param>
        /// <returns>The vertex in degrees with the boundary height.</returns>
        public TangentPointPosition this[int index] => this.vertices[index];

        /// <summary>
        /// Copies the vertices to a new array.
        /// </summary>
        /// <returns>The vertices in counter-clockwise order.</returns>
        public TangentPointPosition[] ToArray()
        {
            var copy = new TangentPointPosition[this.vertices.Length];
            Array.Copy(this.vertices, copy, copy.Length);
            return copy;
        }
    }
}
