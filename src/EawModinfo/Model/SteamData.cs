using System;
using System.Collections.Generic;
using EawModinfo.Model.Json;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;

namespace EawModinfo.Model;

/// <inheritdoc/>
public sealed class SteamData : ISteamData
{
    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public string ContentFolder { get; }

    /// <inheritdoc/>
    public SteamWorkshopVisibility Visibility { get; }

    /// <inheritdoc/>
    public string? Metadata { get; init; }

    /// <inheritdoc/>
    public IEnumerable<string> Tags { get; }

    /// <inheritdoc/>
    public string? Description { get; init; }

    /// <inheritdoc/>
    public string? PreviewFile { get; init; }

    /// <inheritdoc/>
    public string Title { get; }

    /// <summary>
    /// Creates a new instance with necessary data
    /// </summary>
    public SteamData(string id, string contentFolder, SteamWorkshopVisibility visibility, string title, IEnumerable<string> tags)
    {
        ThrowHelper.ThrowIfNullOrEmpty(id);
        ThrowHelper.ThrowIfNullOrEmpty(contentFolder);
        ThrowHelper.ThrowIfNullOrEmpty(title);
        Id = id;
        ContentFolder = contentFolder;
        Visibility = visibility;
        Title = title;
        Tags = new HashSet<string>(tags);
    }

    /// <summary>
    /// Creates a new instance from a given <see cref="ISteamData"/> instance.
    /// </summary>
    /// <param name="steamData">The instance that will be copied.</param>
    public SteamData(ISteamData steamData)
    {
        if (steamData == null)
            throw new ArgumentNullException(nameof(steamData));
        Id = steamData.Id;
        ContentFolder = steamData.ContentFolder;
        Visibility = steamData.Visibility;
        Metadata = steamData.Metadata;
        Tags = new HashSet<string>(steamData.Tags);
        Description = steamData.Description;
        PreviewFile = steamData.PreviewFile;
        Title = steamData.Title;
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="JsonSteamData"/>
    /// </summary>
    /// <param name="data">The raw json data.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
    public static SteamData Parse(string data)
    {
        var jsonData = ParseUtility.Parse<JsonSteamData>(data);
        return new SteamData(jsonData);
    }

    /// <inheritdoc/>
    public string ToJson()
    {
        return new JsonSteamData(this).ToJson();
    }
}