using System.Collections.Generic;
using EawModinfo.Spec.Steam;
using Microsoft;
using Newtonsoft.Json;

namespace EawModinfo.Model.Steam
{
    public class SteamData : ISteamData
    {
        [JsonProperty("publishedfileid", Required = Required.Always)]
        public string Id { get; internal set; }

        [JsonProperty("contentfolder", Required = Required.Always)]
        public string ContentFolder { get; internal set; }

        [JsonProperty("visibility", Required = Required.Always)]
        public SteamWorkshopVisibility Visibility { get; internal set; }
        
        [JsonProperty("metadata")]
        public string? Metadata { get; internal set; }

        [JsonProperty("tags", Required = Required.Always)]
        public IEnumerable<string> Tags { get; internal set; }

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
        }
    }
}