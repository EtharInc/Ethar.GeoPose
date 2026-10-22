// <copyright file="H3CellSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.H3.Authority
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Extensions;
    using Ethar.GeoPose.FrameSpecifications;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// A frame specification whose origin is the centre of an H3 cell at a given height, with the local tangent plane east-north-up axes of the Ethar LTP-ENU frame.
    /// </summary>
    /// <remarks>
    /// This lets an Advanced GeoPose name a cell as its frame: <c>{"authority":"/Ethar.GeoPose.H3/1.0","id":"H3-CELL","parameters":"cell=8928308280fffff&amp;heightInMeters=35.5"}</c>.
    /// The cell carries the horizontal position, so the parameters are the cell index and the height only. <see cref="Position"/> resolves the origin when a numeric position is needed.
    /// </remarks>
    public class H3CellSpecification : BaseFrameSpecification, IEquatable<H3CellSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="H3CellSpecification"/> class with no cell. Used by JSON validation, which needs a default instance to read the id and authority.
        /// </summary>
        public H3CellSpecification()
            : base(H3AuthorityConstants.H3CellSpecificationId, H3AuthorityConstants.AuthorityName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="H3CellSpecification"/> class.
        /// </summary>
        /// <param name="cell">The cell whose centre is the frame origin.</param>
        /// <param name="heightInMeters">The height of the origin above the WGS84 ellipsoid.</param>
        public H3CellSpecification(H3Index cell, double heightInMeters)
            : base(H3AuthorityConstants.H3CellSpecificationId, H3AuthorityConstants.AuthorityName)
        {
            this.Cell = cell;
            this.HeightInMeters = heightInMeters;
        }

        /// <summary>
        /// Gets the cell whose centre is the frame origin.
        /// </summary>
        public H3Index Cell { get; }

        /// <summary>
        /// Gets the height of the origin above the WGS84 ellipsoid in metres.
        /// </summary>
        public double HeightInMeters { get; }

        /// <summary>
        /// Gets the frame origin as a GeoPose position: the cell centre at <see cref="HeightInMeters"/>.
        /// </summary>
        public TangentPointPosition Position => this.Cell.ToTangentPointPosition(this.HeightInMeters);

        /// <summary>
        /// Builds the culture invariant parameter string, for example <c>cell=8928308280fffff&amp;heightInMeters=35.5</c>.
        /// </summary>
        /// <returns>The parameters.</returns>
        public string BuildParameters()
        {
            return H3AuthorityConstants.CellParameter + "=" + this.Cell + "&" + H3AuthorityConstants.HeightParameter + "=" + InvariantNumber.Format(this.HeightInMeters);
        }

        /// <inheritdoc/>
        public bool Equals(H3CellSpecification other)
        {
            return other != null && this.Cell == other.Cell && this.HeightInMeters.Equals(other.HeightInMeters) && base.Equals(other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is H3CellSpecification other && this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = base.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Cell.GetHashCode();
                hashcode = (hashcode * 397) ^ this.HeightInMeters.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString() + ", Cell:" + this.Cell + ", HeightInMeters:" + InvariantNumber.Format(this.HeightInMeters);
        }
    }
}
