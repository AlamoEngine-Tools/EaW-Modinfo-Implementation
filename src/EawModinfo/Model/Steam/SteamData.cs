using System.Collections.Generic;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Microsoft;
using Newtonsoft.Json;

namespace EawModinfo.Model.Steam
{
    /// <inheritdoc/>
    public class SteamData : ISteamData
    {
        internal static readonly string[] GameTags = new[] {"FOC", "EAW"};

        /// <inheritdoc/>
        [JsonProperty("publishedfileid", Required = Required.Always)]
        public string Id { get; internal set; }

        /// <inheritdoc/>
        [JsonProperty("contentfolder", Required = Required.Always)]
        public string ContentFolder { get; internal set; }

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

        internal SteamData()
        {
            Tags = new List<string>();
        }

        internal SteamData(ISteamData steamData)
        {
            Requires.NotNull(steamData, nameof(steamData));
            Id = steamData.Id;
            ContentFolder = steamData.ContentFolder;
            Visibility = steamData.Visibility;
            Metadata = steamData.Metadata;
            Tags = steamData.Tags;
            Description = steamData.Description;
            PreviewFile = steamData.PreviewFile;
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
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}