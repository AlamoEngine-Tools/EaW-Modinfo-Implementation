using System.Collections.Generic;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using SemanticVersioning;
using Validation;

namespace EawModinfo.Model
{
    /// <inheritdoc cref="IModinfo"/>
    public sealed record ModinfoData : IModinfo
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Version? Version { get; init; }

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
        /// Creates a new instance with a given name
        /// </summary>
        /// <param name="name"></param>
        public ModinfoData(string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));
            Name = name;
        }

        /// <summary>
        /// Creates a new instance from a given <see cref="IModIdentity"/> instance.
        /// </summary>
        /// <param name="modIdentity">The instance that will used as a base.</param>
        public ModinfoData(IModIdentity modIdentity)
        {
            Requires.NotNull(modIdentity, nameof(modIdentity));
            Name = modIdentity.Name;
            Dependencies = modIdentity.Dependencies;
            Version = modIdentity.Version;
        }

        /// <summary>
        /// Creates a new instance from a given <see cref="IModinfo"/> instance.
        /// </summary>
        /// <param name="modinfo">The instance that will copied.</param>
        public ModinfoData(IModinfo modinfo)
        {
            Requires.NotNull(modinfo, nameof(modinfo));
            Name = modinfo.Name;
            Version = modinfo.Version;
            Dependencies = modinfo.Dependencies;
            Summary = modinfo.Summary;
            Icon = modinfo.Icon;
            SteamData = modinfo.SteamData;
            Languages = modinfo.Languages;
            Custom = modinfo.Custom;
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
        public string ToJson(bool validate)
        {
            return new JsonModinfoData(this).ToJson(validate);
        }

        /// <inheritdoc/>
        public bool Equals(IModIdentity? other)
        {
            return ModIdentityEqualityComparer.Default.Equals(this, other);
        }
    }
}