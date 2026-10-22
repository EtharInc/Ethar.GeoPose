// <copyright file="H3Index.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.H3
{
    using System;

    /// <summary>
    /// A validated H3 cell index: a 64-bit identifier for one hexagonal or pentagonal cell at one of the 16 H3 resolutions.
    /// </summary>
    /// <remarks>
    /// The struct can only hold a value that passes <see cref="H3.IsValidCell"/>, so a caller who is handed an <see cref="H3Index"/> never has to check it again.
    /// The one exception is <c>default(H3Index)</c>, which holds <see cref="H3.H3Null"/>.
    /// The text form is the 15 character lowercase hexadecimal string used by H3 everywhere, and parsing and formatting are culture invariant.
    /// </remarks>
    public readonly struct H3Index : IEquatable<H3Index>, IComparable<H3Index>, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H3Index"/> struct.
        /// </summary>
        /// <param name="value">The 64-bit H3 cell index.</param>
        /// <exception cref="H3Exception">Thrown with <see cref="H3ErrorCode.CellInvalid"/> if the value is not a valid cell.</exception>
        public H3Index(ulong value)
        {
            if (!H3.IsValidCell(value))
            {
                throw new H3Exception(H3ErrorCode.CellInvalid, "The value " + H3.H3ToString(value) + " is not a valid H3 cell index.");
            }

            this.Value = value;
        }

        /// <summary>
        /// Gets the 64-bit H3 cell index.
        /// </summary>
        public ulong Value { get; }

        /// <summary>
        /// Gets the resolution, 0 to 15.
        /// </summary>
        public int Resolution => H3.GetResolution(this.Value);

        /// <summary>
        /// Gets the base cell number, 0 to 121.
        /// </summary>
        public int BaseCell => H3.GetBaseCellNumber(this.Value);

        /// <summary>
        /// Gets a value indicating whether the cell is one of the 12 pentagons at its resolution.
        /// </summary>
        public bool IsPentagon => H3.IsPentagon(this.Value);

        /// <summary>
        /// Gets a value indicating whether the cell is at a Class III resolution. Odd resolutions are Class III and are rotated relative to the icosahedron.
        /// </summary>
        public bool IsClassIII => H3.IsResClassIII(this.Value);

        /// <summary>
        /// Gets the centre of the cell in radians. Use the <c>ToTangentPointPosition</c> extension method for a GeoPose position in degrees.
        /// </summary>
        public LatLng Center
        {
            get
            {
                ThrowIfError(H3.CellToLatLng(this.Value, out var center));
                return center;
            }
        }

        /// <summary>
        /// Returns true if both indexes name the same cell.
        /// </summary>
        /// <param name="left">The first index.</param>
        /// <param name="right">The second index.</param>
        /// <returns>True if equal.</returns>
        public static bool operator ==(H3Index left, H3Index right) => left.Value == right.Value;

        /// <summary>
        /// Returns true if the indexes name different cells.
        /// </summary>
        /// <param name="left">The first index.</param>
        /// <param name="right">The second index.</param>
        /// <returns>True if not equal.</returns>
        public static bool operator !=(H3Index left, H3Index right) => left.Value != right.Value;

        /// <summary>
        /// Orders indexes by their numeric value.
        /// </summary>
        /// <param name="left">The first index.</param>
        /// <param name="right">The second index.</param>
        /// <returns>True if the first value is less than the second.</returns>
        public static bool operator <(H3Index left, H3Index right) => left.Value < right.Value;

        /// <summary>
        /// Orders indexes by their numeric value.
        /// </summary>
        /// <param name="left">The first index.</param>
        /// <param name="right">The second index.</param>
        /// <returns>True if the first value is greater than the second.</returns>
        public static bool operator >(H3Index left, H3Index right) => left.Value > right.Value;

        /// <summary>
        /// Orders indexes by their numeric value.
        /// </summary>
        /// <param name="left">The first index.</param>
        /// <param name="right">The second index.</param>
        /// <returns>True if the first value is less than or equal to the second.</returns>
        public static bool operator <=(H3Index left, H3Index right) => left.Value <= right.Value;

        /// <summary>
        /// Orders indexes by their numeric value.
        /// </summary>
        /// <param name="left">The first index.</param>
        /// <param name="right">The second index.</param>
        /// <returns>True if the first value is greater than or equal to the second.</returns>
        public static bool operator >=(H3Index left, H3Index right) => left.Value >= right.Value;

        /// <summary>
        /// Parses the hexadecimal text form of a cell index. Upper and lower case are accepted and surrounding whitespace is ignored. Parsing is culture invariant.
        /// </summary>
        /// <param name="text">The text, for example <c>8928308280fffff</c>.</param>
        /// <returns>The index.</returns>
        /// <exception cref="H3Exception">Thrown with <see cref="H3ErrorCode.Failed"/> if the text is not hexadecimal, or <see cref="H3ErrorCode.CellInvalid"/> if it does not name a valid cell.</exception>
        public static H3Index Parse(string text)
        {
            var code = H3.StringToH3(text, out var value);
            if (code != H3ErrorCode.Success)
            {
                throw new H3Exception(code, "The text '" + text + "' is not a hexadecimal H3 cell index.");
            }

            return new H3Index(value);
        }

        /// <summary>
        /// Parses the hexadecimal text form of a cell index without throwing.
        /// </summary>
        /// <param name="text">The text, for example <c>8928308280fffff</c>.</param>
        /// <param name="index">The index, or <c>default</c> on failure.</param>
        /// <returns>True if the text names a valid cell.</returns>
        public static bool TryParse(string text, out H3Index index)
        {
            if (H3.StringToH3(text, out var value) != H3ErrorCode.Success)
            {
                index = default;
                return false;
            }

            return TryCreate(value, out index);
        }

        /// <summary>
        /// Wraps a 64-bit value without throwing.
        /// </summary>
        /// <param name="value">The 64-bit H3 cell index.</param>
        /// <param name="index">The index, or <c>default</c> if the value is not a valid cell.</param>
        /// <returns>True if the value is a valid cell.</returns>
        public static bool TryCreate(ulong value, out H3Index index)
        {
            if (!H3.IsValidCell(value))
            {
                index = default;
                return false;
            }

            index = new H3Index(value);
            return true;
        }

        /// <summary>
        /// Creates the index of the cell containing a point at the given resolution.
        /// </summary>
        /// <param name="point">The point in radians.</param>
        /// <param name="resolution">The resolution, 0 to 15.</param>
        /// <returns>The containing cell.</returns>
        /// <exception cref="H3Exception">Thrown with <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15, or <see cref="H3ErrorCode.LatLngDomain"/> for a non finite coordinate.</exception>
        public static H3Index FromLatLng(LatLng point, int resolution)
        {
            ThrowIfError(H3.LatLngToCell(point, resolution, out var value));
            return new H3Index(value);
        }

        /// <summary>
        /// Returns the boundary of the cell in radians, counter-clockwise. Use the <c>ToCellBoundary</c> extension method for GeoPose positions in degrees.
        /// </summary>
        /// <returns>The boundary, with 5 to 10 vertices.</returns>
        public CellBoundary Boundary()
        {
            ThrowIfError(H3.CellToBoundary(this.Value, out var boundary));
            return boundary;
        }

        /// <summary>
        /// Returns the ancestor of this cell at a coarser resolution, or the cell itself at its own resolution.
        /// </summary>
        /// <param name="resolution">The parent resolution, 0 to <see cref="Resolution"/>.</param>
        /// <returns>The parent cell.</returns>
        /// <exception cref="H3Exception">Thrown with <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15, or <see cref="H3ErrorCode.ResMismatch"/> for a resolution finer than this cell.</exception>
        public H3Index Parent(int resolution)
        {
            ThrowIfError(H3.CellToParent(this.Value, resolution, out var parent));
            return new H3Index(parent);
        }

        /// <summary>
        /// Returns every ancestor of this cell, indexed by resolution, from resolution 0 up to and including the cell itself.
        /// </summary>
        /// <remarks>
        /// This is the "superposition" of the cell over all coarser levels. Each entry is a few bit operations, so callers who store the finest cell they need can derive any coarser level on demand rather than storing all of them.
        /// </remarks>
        /// <returns>An array of <see cref="Resolution"/> + 1 cells where element r is the ancestor at resolution r.</returns>
        public H3Index[] Ancestors()
        {
            var ancestors = new H3Index[this.Resolution + 1];
            for (var resolution = 0; resolution < ancestors.Length; resolution++)
            {
                ancestors[resolution] = this.Parent(resolution);
            }

            return ancestors;
        }

        /// <summary>
        /// Returns the child of this cell whose centre coincides with this cell's centre, at a finer resolution.
        /// </summary>
        /// <param name="resolution">The child resolution, <see cref="Resolution"/> to 15.</param>
        /// <returns>The center child, or this cell at its own resolution.</returns>
        /// <exception cref="H3Exception">Thrown with <see cref="H3ErrorCode.ResDomain"/> for a resolution coarser than this cell or above 15.</exception>
        public H3Index CenterChild(int resolution)
        {
            ThrowIfError(H3.CellToCenterChild(this.Value, resolution, out var child));
            return new H3Index(child);
        }

        /// <summary>
        /// Returns the number of children of this cell at a finer resolution: 7 to the power of the resolution difference for a hexagon, fewer for a pentagon.
        /// </summary>
        /// <param name="resolution">The child resolution, <see cref="Resolution"/> to 15.</param>
        /// <returns>The number of children.</returns>
        /// <exception cref="H3Exception">Thrown with <see cref="H3ErrorCode.ResDomain"/> for a resolution coarser than this cell or above 15.</exception>
        public long ChildrenSize(int resolution)
        {
            ThrowIfError(H3.CellToChildrenSize(this.Value, resolution, out var size));
            return size;
        }

        /// <summary>
        /// Returns every child of this cell at a finer resolution.
        /// </summary>
        /// <param name="resolution">The child resolution, <see cref="Resolution"/> to 15.</param>
        /// <returns>The children. The count grows by a factor of seven per resolution step, so keep the step small.</returns>
        /// <exception cref="H3Exception">Thrown with <see cref="H3ErrorCode.ResDomain"/> for a resolution coarser than this cell or above 15.</exception>
        public H3Index[] Children(int resolution)
        {
            var size = this.ChildrenSize(resolution);
            var values = new ulong[size];
            ThrowIfError(H3.CellToChildren(this.Value, resolution, values));
            var children = new H3Index[size];
            for (var i = 0; i < children.Length; i++)
            {
                children[i] = new H3Index(values[i]);
            }

            return children;
        }

        /// <summary>
        /// Returns true if <paramref name="other"/> is this cell or one of its descendants at a finer resolution.
        /// </summary>
        /// <remarks>
        /// This is the H3 idiom <c>cellToParent(other, resolution) == cell</c>. Containment follows the index hierarchy, which is what H3 defines; the hexagons themselves only approximately nest.
        /// </remarks>
        /// <param name="other">The cell to test.</param>
        /// <returns>True if the other cell lies under this cell in the hierarchy.</returns>
        public bool Contains(H3Index other)
        {
            if (other.Resolution < this.Resolution)
            {
                return false;
            }

            return H3.CellToParent(other.Value, this.Resolution, out var parent) == H3ErrorCode.Success && parent == this.Value;
        }

        /// <summary>
        /// Returns true if this cell is <paramref name="ancestor"/> or one of its descendants.
        /// </summary>
        /// <param name="ancestor">The coarser cell.</param>
        /// <returns>True if this cell lies under the ancestor in the hierarchy.</returns>
        public bool IsWithin(H3Index ancestor) => ancestor.Contains(this);

        /// <inheritdoc/>
        public bool Equals(H3Index other) => this.Value == other.Value;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is H3Index other && this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <inheritdoc/>
        public int CompareTo(H3Index other) => this.Value.CompareTo(other.Value);

        /// <summary>
        /// Returns the 15 character lowercase hexadecimal form of the index, culture invariant.
        /// </summary>
        /// <returns>The text, for example <c>8928308280fffff</c>.</returns>
        public override string ToString() => H3.H3ToString(this.Value);

        /// <inheritdoc/>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is H3Index other)
            {
                return this.CompareTo(other);
            }

            throw new ArgumentException("Object must be an H3Index.", nameof(obj));
        }

        private static void ThrowIfError(H3ErrorCode code)
        {
            if (code != H3ErrorCode.Success)
            {
                throw new H3Exception(code);
            }
        }
    }
}
