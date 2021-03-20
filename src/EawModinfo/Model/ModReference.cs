using System;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using Newtonsoft.Json;
using Validation;

namespace EawModinfo.Model
{
    /// <inheritdoc/>
    public class ModReference : IModReference
    {
        /// <inheritdoc/>
        [JsonProperty("identifier", Required = Required.Always)]
        public string Identifier { get; internal init; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("modtype", Required = Required.Always)]
        public ModType Type { get; internal init; }

        [JsonConstructor]
        internal ModReference()
        {
        }

        /// <summary>
        /// Creates a new instance from a given <see cref="IModReference"/> instance.
        /// </summary>
        /// <param name="modReference">The instance that will copied.</param>
        public ModReference(IModReference modReference)
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
                return ((IEquatable<IModReference>)this).Equals(reference);
            return false;

        }

        /// <inheritdoc/>
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