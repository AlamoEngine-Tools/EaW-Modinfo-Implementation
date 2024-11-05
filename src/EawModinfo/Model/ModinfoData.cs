using System;
using System.Collections.Generic;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Semver;

namespace EawModinfo.Model;

/// <inheritdoc cref="IModinfo"/>
public sealed class ModinfoData : IModinfo
{
    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public SemVersion? Version { get; init; }

    /// <inheritdoc/>
    public IModDependencyList Dependencies { get; init; } = DependencyList.EmptyDependencyList;

    /// <inheritdoc/>
    public string? Summary { get; init; }

    /// <inheritdoc/>
    public string? Icon { get; init; }

    /// <inheritdoc/>
    public IDictionary<string, object> Custom { get; init; } = new Dictionary<string, object>();

    /// <inheritdoc/>
    public ISteamData? SteamData { get; init; }

    /// <inheritdoc/>
    public IEnumerable<ILanguageInfo> Languages { get; init; } = new List<ILanguageInfo> { LanguageInfo.Default };
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ModinfoData"/> class with a given name.
    /// </summary>
    /// <param name="name">The name of the mod.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
    public ModinfoData(string name)
    {
        ThrowHelper.ThrowIfNullOrEmpty(name);
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModinfoData"/> class of the specified mod identity.
    /// </summary>
    /// <param name="modIdentity">The mod identity to represent as a modinfo.</param>
    /// <exception cref="ArgumentNullException"><paramref name="modIdentity"/> is <see langword="null"/>.</exception>
    public ModinfoData(IModIdentity modIdentity)
    {
        if (modIdentity == null) 
            throw new ArgumentNullException(nameof(modIdentity));
        Name = modIdentity.Name;
        Dependencies = new DependencyList(modIdentity.Dependencies);
        Version = modIdentity.Version;
    }

    /// <summary>
    /// Creates a new instance from a given <see cref="IModinfo"/> instance.
    /// </summary>
    /// <param name="modinfo">The instance that will be copied.</param>
    public ModinfoData(IModinfo modinfo)
    {
        if (modinfo == null)
            throw new ArgumentNullException(nameof(modinfo));
        Name = modinfo.Name;
        Version = modinfo.Version;
        Dependencies = new DependencyList(modinfo.Dependencies);
        Summary = modinfo.Summary;
        Icon = modinfo.Icon;
        SteamData = modinfo.SteamData != null ? new SteamData(modinfo.SteamData) : null;
        Languages = new List<ILanguageInfo>(modinfo.Languages);
        Custom = new Dictionary<string, object>(modinfo.Custom);
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="ModinfoData"/>
    /// </summary>
    /// <param name="data">The raw json data.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
    public static ModinfoData Parse(string data)
    {
        return new ModinfoData(ParseUtility.Parse<JsonModinfoData>(data));
    }

    /// <inheritdoc/>
    public string ToJson()
    {
        return new JsonModinfoData(this).ToJson();
    }

    /// <inheritdoc/>
    bool IEquatable<IModIdentity>.Equals(IModIdentity? other)
    {
        return ModIdentityEqualityComparer.Default.Equals(this, other);
    }
}