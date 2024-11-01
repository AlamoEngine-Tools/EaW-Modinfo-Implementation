using System;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using EawModinfo.Utilities;
using Semver;

namespace EawModinfo.Model;

/// <inheritdoc cref="IModReference"/>
public readonly struct ModReference : IModReference , IEquatable<ModReference>
{
    /// <inheritdoc/>
    public string Identifier { get; init; }

    /// <inheritdoc/>
    public ModType Type { get; init; }

    /// <inheritdoc/>
    public SemVersionRange? VersionRange { get; }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    public ModReference(string id, ModType modType, SemVersionRange? range = null)
    {
        ThrowHelper.ThrowIfNullOrEmpty(id);
        Identifier = id;
        Type = modType;
        VersionRange = range;
    }

    /// <summary>
    /// Creates a new instance from a given <see cref="IModReference"/> instance.
    /// </summary>
    /// <param name="modReference">The instance that will be copied.</param>
    public ModReference(IModReference modReference)
    {
        Identifier = modReference.Identifier;
        Type = modReference.Type;
        VersionRange = modReference.VersionRange;
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="ModReference"/>
    /// </summary>
    /// <param name="data">The raw json data.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">Throws when parsing failed, e.g. due to missing required properties.</exception>
    public static ModReference Parse(string data)
    {
        return new ModReference(ParseUtility.Parse<JsonModReference>(data));
    }

    /// <inheritdoc />
    public bool Equals(IModReference? other)
    {
        return ModReferenceEqualityComparer.Default.Equals(this, other);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is ModReference other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return ModReferenceEqualityComparer.Default.GetHashCode(this);
    }

    /// <inheritdoc />
    public bool Equals(ModReference other)
    {
        return ModReferenceEqualityComparer.Default.Equals(this, other);
    }
}