using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    internal static readonly IReadOnlyCollection<ILanguageInfo> UnsetLanguages = [LanguageInfo.Default];

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
    public IReadOnlyCollection<ILanguageInfo> Languages { get; init; } = UnsetLanguages;
        
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
    /// Initializes a new instance of the <see cref="ModinfoData"/> of the specified <see cref="IModinfo"/>.
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
        Custom = new Dictionary<string, object>(modinfo.Custom);

        if (ReferenceEquals(modinfo.Languages, UnsetLanguages) || modinfo.Languages.Count == 0)
            return;
        Languages = modinfo.Languages.Select(x => new LanguageInfo(x)).Distinct().ToList();
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="ModinfoData"/>.
    /// </summary>
    /// <param name="data">The raw json data.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
    public static ModinfoData Parse(string data)
    {
        if (data == null) 
            throw new ArgumentNullException(nameof(data));
        return new ModinfoData(ParseUtility.Parse<JsonModinfoData>(data));
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="ModinfoData"/>.
    /// </summary>
    /// <param name="data">The json data stream.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
    public static ModinfoData Parse(Stream data)
    {
        if (data == null) 
            throw new ArgumentNullException(nameof(data));
        return new ModinfoData(ParseUtility.Parse<JsonModinfoData>(data));
    }

    /// <inheritdoc/>
    public string ToJson()
    {
        return new JsonModinfoData(this).ToJson();
    }

    /// <inheritdoc />
    public void ToJson(Stream stream)
    {
        if (stream == null) 
            throw new ArgumentNullException(nameof(stream));
        new JsonModinfoData(this).ToJson(stream);
    }

    /// <inheritdoc/>
    bool IEquatable<IModIdentity>.Equals(IModIdentity? other)
    {
        return ModIdentityEqualityComparer.Default.Equals(this, other);
    }
}