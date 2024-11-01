using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;

namespace EawModinfo.Model.Json;

/// <inheritdoc/>
internal class JsonSteamData : ISteamData
{
    internal static readonly string[] GameTags = {"FOC", "EAW"};

    /// <inheritdoc/>
    [JsonPropertyName("publishedfileid")]
    [JsonRequired]
    public string Id { get; set; } = string.Empty;

    /// <inheritdoc/>
    [JsonPropertyName("contentfolder")]
    [JsonRequired]
    public string ContentFolder { get; set; } = string.Empty;

    /// <inheritdoc/>
    [JsonPropertyName("previewfile")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? PreviewFile { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("visibility")]
    [JsonRequired]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public SteamWorkshopVisibility Visibility { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("title")]
    [JsonRequired]
    public string Title { get; set; } = string.Empty;

    /// <inheritdoc/>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? Description { get; set; }


    /// <inheritdoc/>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? Metadata { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("tags")]
    [JsonRequired]
    public IEnumerable<string> Tags { get; set; }
    
    [JsonConstructor]
    public JsonSteamData()
    {
        Tags = new HashSet<string>();
    }

    /// <summary>
    /// Creates a new instance from a given <see cref="ISteamData"/> instance.
    /// </summary>
    /// <param name="steamData">The instance that will be copied.</param>
    public JsonSteamData(ISteamData steamData)
    {
        if (steamData == null)
            throw new ArgumentNullException(nameof(steamData));
        Id = steamData.Id;
        ContentFolder = steamData.ContentFolder;
        PreviewFile = steamData.PreviewFile;
        Visibility = steamData.Visibility;
        Title = steamData.Title;
        Description = steamData.Description;
        Metadata = steamData.Metadata;
        Tags = steamData.Tags;
    }

    /// <summary>
    /// Converts this instance to a json string.
    /// </summary>
    /// <param name="validate">If set to <see langword="true"/> this object gets validated first.</param>
    /// <returns>The converted json string data</returns>
    public string ToJson(bool validate = true)
    {
        if (validate)
            this.Validate();

        return JsonSerializer.Serialize(this, ParseUtility.SerializerOptions);
    }
}