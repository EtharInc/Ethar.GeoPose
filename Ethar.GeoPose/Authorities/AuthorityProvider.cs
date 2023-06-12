// <copyright file="AuthorityProvider.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority
{
    using System.Collections.Generic;
    using System.Linq;
    using Ethar.GeoPose.Exceptions;
    using Ethar.GeoPose.Extensions;

    /// <summary>
    /// A static management class that provides the application with information about which authorities are implemented, including registration management.
    /// </summary>
    public static class AuthorityProvider
    {
        private static readonly Dictionary<string, IAuthority> AuthorityDictionary;

        static AuthorityProvider()
        {
            AuthorityDictionary = new Dictionary<string, IAuthority>();
        }

        /// <summary>
        /// Gets a list that contains the supported authorities.
        /// </summary>
        public static IList<IAuthority> Authorities => AuthorityDictionary.Values.ToList();

        /// <summary>
        /// Adds an <see cref="IAuthority"/> to the list of supported authorities.
        /// </summary>
        /// <param name="authority">The authority to add.</param>
        public static void RegisterAuthority(IAuthority authority) => AuthorityDictionary.TryAdd(authority.AuthorityName, authority);

        /// <summary>
        /// Removes an <see cref="IAuthority"/> from the list of supported authorities.
        /// </summary>
        /// <param name="authorityName">The name authority to remove.</param>
        public static void UnregisterAuthority(string authorityName) => AuthorityDictionary.Remove(authorityName);

        /// <summary>
        /// Gets the authority associated with the identifier, if it exists in the supported authorities.
        /// </summary>
        /// <param name="authorityName">The name of the authority to get.</param>
        /// <returns>The authority if it exists.</returns>
        /// <exception cref="AuthorityNotSupportedException">Thrown if the authority does not exist in the supported authorities.</exception>
        public static IAuthority GetAuthority(string authorityName)
        {
            AuthorityDictionary.TryGetValue(authorityName, out var authority);
            return authority ?? throw new AuthorityNotSupportedException($"The authority {authorityName} is not supported. Consider adding it with the UseAuthority method.");
        }
    }
}
