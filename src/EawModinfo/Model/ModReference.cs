using System;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using Semver;

namespace EawModinfo.Model;

/// <inheritdoc/>
public struct ModReference : IModReference
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
    /// <param name="modReference">The instance that will copied.</param>
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

    bool IEquatable<IModReference>.Equals(IModReference? other)
    {
        return Identifier == other?.Identifier && Type == other.Type;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is IModReference reference)
            return ((IModReference)this).Equals(reference);
        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Identifier, (int) Type);
    }
}