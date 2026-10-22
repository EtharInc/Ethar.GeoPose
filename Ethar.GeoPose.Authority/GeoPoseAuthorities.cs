// <copyright file="GeoPoseAuthorities.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority
{
    using Ethar.GeoPose.Authority.Ogc;

    /// <summary>
    /// Registration helpers for the authorities shipped with this package.
    /// </summary>
    public static class GeoPoseAuthorities
    {
        /// <summary>
        /// Registers the Ethar authority (<c>/Ethar.GeoPose/1.0</c>) and the OGC example authority (<c>/geopose/1.0</c>) with the
        /// <see cref="AuthorityProvider"/>. Registering an authority that is already registered is a no-op.
        /// </summary>
        public static void RegisterDefaults()
        {
            AuthorityProvider.RegisterAuthority(new EtharGeoPoseAuthority());
            AuthorityProvider.RegisterAuthority(new OgcGeoPoseAuthority());
        }

        /// <summary>
        /// Removes both shipped authorities from the <see cref="AuthorityProvider"/>.
        /// </summary>
        public static void UnregisterDefaults()
        {
            AuthorityProvider.UnregisterAuthority(Constants.AuthorityName);
            AuthorityProvider.UnregisterAuthority(OgcConstants.AuthorityName);
        }
    }
}
