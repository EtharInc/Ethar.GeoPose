// <copyright file="CellBoundary.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/include/h3api.h.in. Copyright 2016-2021 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using System;

    /// <summary>
    /// Cell boundary in latitude and longitude radians, vertices in counter-clockwise order. Ported from the C struct <c>CellBoundary</c>.
    /// </summary>
    /// <remarks>
    /// A hexagon has 6 vertices and a pentagon 5, but a cell that straddles an icosahedron edge gains one extra vertex per crossing, so the count can reach <see cref="MaxVerts"/>.
    /// </remarks>
    public sealed class CellBoundary
    {
        /// <summary>
        /// Maximum number of cell boundary vertices. The worst case is a pentagon: 5 original vertices plus 5 edge crossings. C name <c>MAX_CELL_BNDRY_VERTS</c>.
        /// </summary>
        public const int MaxVerts = 10;

        private readonly LatLng[] verts = new LatLng[MaxVerts];

        /// <summary>
        /// Gets the number of vertices. C name <c>numVerts</c>.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the vertex at the given position.
        /// </summary>
        /// <param name="index">The vertex position, 0 to <see cref="Count"/> - 1.</param>
        /// <returns>The vertex in radians.</returns>
        public LatLng this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return this.verts[index];
            }
        }

        /// <summary>
        /// Copies the vertices to a new array.
        /// </summary>
        /// <returns>The vertices in counter-clockwise order.</returns>
        public LatLng[] ToArray()
        {
            var copy = new LatLng[this.Count];
            Array.Copy(this.verts, copy, this.Count);
            return copy;
        }

        /// <summary>
        /// Removes all vertices.
        /// </summary>
        internal void Clear()
        {
            this.Count = 0;
        }

        /// <summary>
        /// Appends a vertex.
        /// </summary>
        /// <param name="vertex">The vertex in radians.</param>
        internal void Add(LatLng vertex)
        {
            this.verts[this.Count] = vertex;
            this.Count++;
        }
    }
}
