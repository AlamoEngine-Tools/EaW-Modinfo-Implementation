using System;
using System.Text.Json.Serialization;
using EawModinfo.Spec;
using Validation;

#if NET || NETSTANDARD2_1
using Range = SemanticVersioning.Range;
#else
using SemanticVersioning;
#endif


namespace EawModinfo.Model.Json
{
    internal class JsonModReference : IModReference
    {
        [JsonPropertyName("identifier")]
        [JsonRequired]
        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("modtype")]
        [JsonRequired]
        public ModType Type { get; set; }

        [JsonPropertyName("version-range")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? VersionRangeString { get; set; }

        [JsonIgnore]
        public Range? VersionRange => !Range.TryParse(VersionRangeString, out var range) ? null : range;

        [JsonConstructor]
        public JsonModReference()
        {
        }

        public JsonModReference(IModReference modReference)
        {
            Requires.NotNull(modReference, nameof(modReference));
            Identifier = modReference.Identifier;
            Type = modReference.Type;
            VersionRangeString = modReference.VersionRange?.ToString();
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
            return HashCode.Combine(Identifier, (int) Type);
        }
    }
}