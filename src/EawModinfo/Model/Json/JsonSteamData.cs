using System.Collections.Generic;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Newtonsoft.Json;
using Validation;

namespace EawModinfo.Model.Json;

/// <inheritdoc/>
internal class JsonSteamData : ISteamData
{
    internal static readonly string[] GameTags = {"FOC", "EAW"};

    /// <inheritdoc/>
    [JsonProperty("publishedfileid", Required = Required.Always)]
    public string Id { get; internal set; } = string.Empty;

    /// <inheritdoc/>
    [JsonProperty("contentfolder", Required = Required.Always)]
    public string ContentFolder { get; internal set; } = string.Empty;

    /// <inheritdoc/>
    [JsonProperty("visibility", Required = Required.Always)]
    public SteamWorkshopVisibility Visibility { get; internal set; }

    /// <inheritdoc/>
    [JsonProperty("metadata")]
    public string? Metadata { get; internal set; }

    /// <inheritdoc/>
    [JsonProperty("tags", Required = Required.Always)]
    public IEnumerable<string> Tags { get; internal set; }

    /// <inheritdoc/>
    [JsonProperty("description")]
    public string? Description { get; internal set; }

    /// <inheritdoc/>
    [JsonProperty("previewfile")] 
    public string? PreviewFile { get; internal set; }

    /// <inheritdoc/>
    [JsonProperty("title", Required = Required.Always)]
    public string Title { get; internal set; } = string.Empty;

    [JsonConstructor]
    internal JsonSteamData()
    {
        Tags = new List<string>();
    }

    /// <summary>
    /// Creates a new instance from a given <see cref="ISteamData"/> instance.
    /// </summary>
    /// <param name="steamData">The instance that will copied.</param>
    public JsonSteamData(ISteamData steamData)
    {
        Requires.NotNull(steamData, nameof(steamData));
        Id = steamData.Id;
        ContentFolder = steamData.ContentFolder;
        Visibility = steamData.Visibility;
        Metadata = steamData.Metadata;
        Tags = steamData.Tags;
        Description = steamData.Description;
        PreviewFile = steamData.PreviewFile;
        Title = steamData.Title;
    }

    /// <summary>
    /// Converts this instance to a json string.
    /// </summary>
    /// <param name="validate">If set to <see langword="true"/> this object get's validated first.</param>
    /// <returns>The converted json string data</returns>
    public string ToJson(bool validate = true)
    {
        if (validate)
            this.Validate();
        return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            ContractResolver = SteamDataResolver.Instance
        });
    }
}