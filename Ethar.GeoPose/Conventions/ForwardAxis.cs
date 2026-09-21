// <copyright file="ForwardAxis.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Conventions
{
    /// <summary>
    /// Which local axis of a posed object is treated as its forward direction when a GeoPose orientation is mapped to an engine frame.
    /// </summary>
    public enum ForwardAxis
    {
        /// <summary>
        /// The GeoPose convention: the inner frame's x axis is the reference direction, so an un-rotated pose faces East and a yaw of 90°
        /// faces North. The mapped engine rotation is the exact image of the GeoPose rotation; the object's engine +X axis faces the
        /// bearing the GeoPose x axis faces.
        /// </summary>
        GeoPoseX = 0,

        /// <summary>
        /// The engine convention: the object's engine +Z axis should face the bearing the GeoPose x axis faces. An extra 90° local yaw is
        /// applied so that models authored with +Z forward (Unity's convention) point the right way. Use this when placing engine content;
        /// use <see cref="GeoPoseX"/> when the engine rotation must round-trip exactly to the GeoPose value.
        /// </summary>
        EngineZ = 1,
    }
}
