using System;
using System.Text.Json.Serialization;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using Semver;


namespace EawModinfo.Model.Json;

internal class JsonModReference : IModReference
{
    [JsonPropertyName("identifier")]
    [JsonRequired]
    public string Identifier { get; set; } = string.Empty;

    [JsonPropertyName("modtype")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonRequired]
    public ModType Type { get; set; }

    [JsonPropertyName("version-range")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? VersionRangeString { get; set; }

    [JsonIgnore]
    public SemVersionRange? VersionRange => !SemVersionRange.TryParse(VersionRangeString, out var range) ? null : range;

    [JsonConstructor]
    public JsonModReference()
    {
    }

    public JsonModReference(IModReference modReference)
    {
        if (modReference == null) 
            throw new ArgumentNullException(nameof(modReference));
        Identifier = modReference.Identifier;
        Type = modReference.Type;
        VersionRangeString = modReference.VersionRange?.ToString();
    }

    public bool Equals(IModReference? other)
    {
        return ModReferenceEqualityComparer.Default.Equals(this, other);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null) 
            return false;
        if (ReferenceEquals(this, obj)) 
            return true;
        if (obj is not JsonModReference jsonRef) 
            return false;
        return ((IModReference)this).Equals(jsonRef);

    }

    public override int GetHashCode()
    {
        return ModReferenceEqualityComparer.Default.GetHashCode(this);
    }
}