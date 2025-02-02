using System;
using System.Collections.Generic;
using System.IO;
using AET.Modinfo.Model.Json;
using AET.Modinfo.Spec.Steam;
using AET.Modinfo.Utilities;

namespace AET.Modinfo.Model;

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
    /// Initializes a new instance of the <see cref="SteamData"/> class.
    /// </summary>
    /// <param name="id">The Steam Workshops code.</param>
    /// <param name="contentFolder">The content folder used by the Steam Workshop uploader.</param>
    /// <param name="visibility">The visibility of the workshop item.</param>
    /// <param name="title">The title of the workshop item.</param>
    /// <param name="tags">The tags shall be included to the workshop item.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="id"/> or <paramref name="contentFolder"/> or <paramref name="title"/> or <paramref name="tags"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="id"/> or <paramref name="contentFolder"/> or <paramref name="tags"/> is empty.</exception>
    public SteamData(string id, string contentFolder, SteamWorkshopVisibility visibility, string title, IEnumerable<string> tags)
    {
        if (tags == null) 
            throw new ArgumentNullException(nameof(tags));
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
    /// <exception cref="ArgumentNullException"><paramref name="steamData"/> is <see langword="null"/>. </exception>
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
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    public static SteamData Parse(string data)
    {
        if (data == null) 
            throw new ArgumentNullException(nameof(data));
        var jsonData = ParseUtility.Parse<JsonSteamData>(data);
        return new SteamData(jsonData);
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="JsonSteamData"/>
    /// </summary>
    /// <param name="data">The json data stream.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    public static SteamData Parse(Stream data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        var jsonData = ParseUtility.Parse<JsonSteamData>(data);
        return new SteamData(jsonData);
    }

    /// <inheritdoc/>
    public string ToJson()
    {
        return new JsonSteamData(this).ToJson();
    }

    /// <inheritdoc />
    public void ToJson(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        new JsonSteamData(this).ToJson(stream);
    }
}