using System.Collections.Generic;
using EawModinfo.Model.Json;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Validation;

namespace EawModinfo.Model
{
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
            Requires.NotNullOrEmpty(id, nameof(id));
            Requires.NotNullOrEmpty(id, nameof(contentFolder));
            Requires.NotNullOrEmpty(id, nameof(title));
            Requires.NotNullOrEmpty(id, nameof(tags));
            Id = id;
            ContentFolder = contentFolder;
            Visibility = visibility;
            Title = title;
            Tags = tags;
        }

        /// <summary>
        /// Creates a new instance from a given <see cref="ISteamData"/> instance.
        /// </summary>
        /// <param name="steamData">The instance that will copied.</param>
        public SteamData(ISteamData steamData)
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
        public string ToJson(bool validate)
        {
            return new JsonSteamData(this).ToJson(validate);
        }
    }
}