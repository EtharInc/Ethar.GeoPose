// <copyright file="H3ErrorCode.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/include/h3api.h.in. Copyright 2016-2021 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    /// <summary>
    /// Result code, success or a specific error, from an H3 operation. The numeric values match the C enum <c>H3ErrorCodes</c>.
    /// </summary>
    public enum H3ErrorCode
    {
        /// <summary>
        /// Success, no error. C name <c>E_SUCCESS</c>.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The operation failed but a more specific error is not available. C name <c>E_FAILED</c>.
        /// </summary>
        Failed = 1,

        /// <summary>
        /// Argument was outside the acceptable range, when a more specific error code is not available. C name <c>E_DOMAIN</c>.
        /// </summary>
        Domain = 2,

        /// <summary>
        /// Latitude or longitude arguments were outside the acceptable range. C name <c>E_LATLNG_DOMAIN</c>.
        /// </summary>
        LatLngDomain = 3,

        /// <summary>
        /// Resolution argument was outside the acceptable range. C name <c>E_RES_DOMAIN</c>.
        /// </summary>
        ResDomain = 4,

        /// <summary>
        /// Cell argument was not valid. C name <c>E_CELL_INVALID</c>.
        /// </summary>
        CellInvalid = 5,

        /// <summary>
        /// Directed edge argument was not valid. C name <c>E_DIR_EDGE_INVALID</c>.
        /// </summary>
        DirEdgeInvalid = 6,

        /// <summary>
        /// Undirected edge argument was not valid. C name <c>E_UNDIR_EDGE_INVALID</c>.
        /// </summary>
        UndirEdgeInvalid = 7,

        /// <summary>
        /// Vertex argument was not valid. C name <c>E_VERTEX_INVALID</c>.
        /// </summary>
        VertexInvalid = 8,

        /// <summary>
        /// Pentagon distortion was encountered which the algorithm could not handle. C name <c>E_PENTAGON</c>.
        /// </summary>
        Pentagon = 9,

        /// <summary>
        /// Duplicate input was encountered in the arguments and the algorithm could not handle it. C name <c>E_DUPLICATE_INPUT</c>.
        /// </summary>
        DuplicateInput = 10,

        /// <summary>
        /// Cell arguments were not neighbors. C name <c>E_NOT_NEIGHBORS</c>.
        /// </summary>
        NotNeighbors = 11,

        /// <summary>
        /// Cell arguments had incompatible resolutions. C name <c>E_RES_MISMATCH</c>.
        /// </summary>
        ResMismatch = 12,

        /// <summary>
        /// Necessary memory allocation failed. C name <c>E_MEMORY_ALLOC</c>.
        /// </summary>
        MemoryAlloc = 13,

        /// <summary>
        /// Bounds of provided memory were not large enough. C name <c>E_MEMORY_BOUNDS</c>.
        /// </summary>
        MemoryBounds = 14,

        /// <summary>
        /// Mode or flags argument was not valid. C name <c>E_OPTION_INVALID</c>.
        /// </summary>
        OptionInvalid = 15,

        /// <summary>
        /// Index argument was not valid. C name <c>E_INDEX_INVALID</c>.
        /// </summary>
        IndexInvalid = 16,

        /// <summary>
        /// Base cell number was outside of the acceptable range. C name <c>E_BASE_CELL_DOMAIN</c>.
        /// </summary>
        BaseCellDomain = 17,

        /// <summary>
        /// Child digits invalid. C name <c>E_DIGIT_DOMAIN</c>.
        /// </summary>
        DigitDomain = 18,

        /// <summary>
        /// Deleted subsequence indicates an invalid index. C name <c>E_DELETED_DIGIT</c>.
        /// </summary>
        DeletedDigit = 19,
    }
}
