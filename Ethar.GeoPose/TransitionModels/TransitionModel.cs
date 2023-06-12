// <copyright file="TransitionModel.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.TransitionModels
{
    using System;
    using Ethar.GeoPose.JsonConversion;
    using Newtonsoft.Json;

    /// <summary>
    /// An abstract class that providss a base a GeoPose <see cref="TransitionModel"/> model, a specific model is required for implementation.
    /// </summary>
    [JsonConverter(typeof(TransitionModelJsonConverter))]
    public abstract class TransitionModel : IEquatable<TransitionModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionModel"/> class.
        /// </summary>
        /// <param name="id">The id of the <see cref="TransitionModel"/>.</param>
        /// <param name="authority">The authority for the <see cref="TransitionModel"/>.</param>
        protected TransitionModel(string id, string authority)
        {
            this.Authority = authority;
            this.Id = id;
        }

        /// <summary>
        /// Gets the authority.
        /// </summary>
        [JsonProperty("authority")]
        public string Authority { get; }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="TransitionModel"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(TransitionModel a, TransitionModel b)
        {
            if (a is null)
            {
                return b is null;
            }

            return b is null ? false : a.Equals(b);
        }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="TransitionModel"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(TransitionModel a, TransitionModel b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(TransitionModel other)
        {
            return string.Equals(this.Id, other.Id)
                   && string.Equals(this.Authority, other.Authority);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var item = obj as TransitionModel;
            return item != null && this.Equals(item);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Id.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Authority.GetHashCode();
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
