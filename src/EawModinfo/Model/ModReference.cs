using System;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using Microsoft;
using Newtonsoft.Json;

namespace EawModinfo.Model
{
    public class ModReference : IModReference
    {
        [JsonProperty("identifier", Required = Required.Always)]
        public string Identifier { get; internal set; }

        [JsonProperty("modtype", Required = Required.Always)]
        public ModType Type { get; internal set; }

        internal ModReference()
        {
        }

        internal ModReference(IModReference modReference)
        {
            Requires.NotNull(modReference, nameof(modReference));
            Identifier = modReference.Identifier;
            Type = modReference.Type;
        }

        /// <summary>
        /// Parses and deserializes a json data into a <see cref="ModReference"/>
        /// </summary>
        /// <param name="data">The raw json data.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
        public static ModReference Parse(string data)
        {
            return ParseUtility.Parse<ModReference>(data);
        }

        bool IEquatable<IModReference>.Equals(IModReference other)
        {
            return Identifier == other.Identifier && Type == other.Type;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is IModReference reference) 
                return ((IEquatable<IModReference>)this).Equals(reference);
            return false;

        }
        
        public override int GetHashCode()
        {
#if NETSTANDARD2_1
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