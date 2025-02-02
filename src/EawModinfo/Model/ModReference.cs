using System;
using System.IO;
using AET.Modinfo.Model.Json;
using AET.Modinfo.Spec;
using AET.Modinfo.Spec.Equality;
using AET.Modinfo.Utilities;
using Semver;

namespace AET.Modinfo.Model;

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
    /// Initializes a new instance of the <see cref="ModReference"/> struct.
    /// </summary>
    /// <param name="id">The identifier name of the mod.</param>
    /// <param name="modType">The type of the mod.</param>
    /// <param name="range">The version range or <see langword="null"/> which describes this reference. Default is <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
    public ModReference(string id, ModType modType, SemVersionRange? range = null)
    {
        ThrowHelper.ThrowIfNullOrEmpty(id);
        Identifier = id;
        Type = modType;
        VersionRange = range;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModReference"/> struct of a specified <see cref="IModReference"/>.
    /// </summary>
    /// <param name="modReference">The instance that will be copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="modReference"/> is <see langword="null"/>.</exception>
    public ModReference(IModReference modReference)
    {
        if (modReference == null) 
            throw new ArgumentNullException(nameof(modReference));
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
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    public static ModReference Parse(string data)
    {
        if (data == null) 
            throw new ArgumentNullException(nameof(data));
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

    /// <inheritdoc/>
    public string ToJson()
    {
        return new JsonModReference(this).ToJson();
    }

    /// <inheritdoc />
    public void ToJson(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        new JsonModReference(this).ToJson(stream);
    }
}