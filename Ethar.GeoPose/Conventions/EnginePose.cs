// <copyright file="EnginePose.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Conventions
{
    using System;
    using Ethar.GeoPose.DataTypes;

    /// <summary>
    /// A position and rotation in a left-handed, Y-up engine frame, the result of mapping a GeoPose through a <see cref="LeftHandedYUpFrame"/>.
    /// </summary>
    public struct EnginePose : IEquatable<EnginePose>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnginePose"/> struct.
        /// </summary>
        /// <param name="position">The position in engine world space, in meters.</param>
        /// <param name="rotation">The rotation in engine world space.</param>
        public EnginePose(LeftHandedYUpVector position, UnitQuaternion rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }

        /// <summary>
        /// Gets or sets the position in engine world space, in meters.
        /// </summary>
        public LeftHandedYUpVector Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation in engine world space. The components map directly onto an engine quaternion (x, y, z, w).
        /// </summary>
        public UnitQuaternion Rotation { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="EnginePose"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator ==(EnginePose a, EnginePose b) => a.Equals(b);

        /// <summary>
        /// Performs an inequality comparison on two objects of type <see cref="EnginePose"/>.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are not equal.</returns>
        public static bool operator !=(EnginePose a, EnginePose b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(EnginePose other)
        {
            return this.Position.Equals(other.Position) && this.Rotation.Equals(other.Rotation);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is EnginePose equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Position.GetHashCode() * 397) ^ this.Rotation.GetHashCode();
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Position:[{this.Position}], Rotation:[{this.Rotation}]";
        }
    }
}
