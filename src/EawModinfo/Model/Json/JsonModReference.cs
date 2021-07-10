﻿using System;
using EawModinfo.Spec;
using Newtonsoft.Json;
using Validation;

namespace EawModinfo.Model.Json
{
    internal class JsonModReference : IModReference
    {
        [JsonProperty("identifier", Required = Required.Always)]
        public string Identifier { get; internal init; } = string.Empty;

        [JsonProperty("modtype", Required = Required.Always)]
        public ModType Type { get; internal init; }

        [JsonConstructor]
        internal JsonModReference()
        {
        }

        public JsonModReference(IModReference modReference)
        {
            Requires.NotNull(modReference, nameof(modReference));
            Identifier = modReference.Identifier;
            Type = modReference.Type;
        }

        bool IEquatable<IModReference>.Equals(IModReference? other)
        {
            return Identifier == other?.Identifier && Type == other.Type;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is null) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj is IModReference reference) 
                return ((IModReference)this).Equals(reference);
            return false;

        }

        public override int GetHashCode()
        {
#if NETSTANDARD2_1 || NET
            return HashCode.Combine(Identifier, (int) Type);
#else
            unchecked
            {
                return (Identifier.GetHashCode() * 397) ^ (int) Type;
            }
#endif
        }
    }
}