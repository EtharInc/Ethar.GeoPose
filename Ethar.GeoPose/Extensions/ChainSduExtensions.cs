// <copyright file="ChainSduExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.Authority;
    using Ethar.GeoPose.StructuralDataUnits;

    /// <summary>
    /// Extensions for the <see cref="ChainSdu"/>.
    /// </summary>
    public static class ChainSduExtensions
    {
        /// <summary>
        /// Validates that the outer frame of the <see cref="ChainSdu"/> is extrinsic.
        /// </summary>
        /// <param name="sdu">The <see cref="ChainSdu"/> to validate.</param>
        /// <returns>True if the sdu is valid, false otherwise.</returns>
        public static bool Validate(this ChainSdu sdu)
        {
            var outerFrame = sdu.OuterFrame;
            var authority = AuthorityProvider.GetAuthority(outerFrame.Authority);
            return authority.IsFrameSpecificationExtrinsic(outerFrame);
        }
    }
}
