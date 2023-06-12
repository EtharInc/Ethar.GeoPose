// <copyright file="BaseFrameSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.FrameSpecifications
{
    using System;
    using Ethar.GeoPose.Interfaces;
    using Ethar.GeoPose.JsonConversion;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that completely and uniquely defines a reference frame.
    ///
    /// Derived from Figure 8 from section 7.2.1 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    [JsonConverter(typeof(BaseFrameSpecificationJsonConverter))]
    public class BaseFrameSpecification : IFrameSpecification, IEquatable<BaseFrameSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFrameSpecification"/> class.
        /// </summary>
        /// <param name="id">An ID that uniquely defines the frame within the authority.</param>
        /// <param name="authority">A string uniquely specifying a source of reference frame specifications.</param>
        protected BaseFrameSpecification(string id, string authority)
        {
            this.Authority = authority;
            this.Id = id;
        }

        /// <inheritdoc/>
        public string Authority { get; }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public bool Equals(BaseFrameSpecification other)
        {
            return string.Equals(this.Authority, other.Authority)
                && string.Equals(this.Id, other.Id);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj != null && obj is BaseFrameSpecification equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Authority.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Id.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Authority:{this.Authority}, Id:{this.Id}";
        }
    }
}