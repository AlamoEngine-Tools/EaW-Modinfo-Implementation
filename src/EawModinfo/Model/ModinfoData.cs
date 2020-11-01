using System;
using System.Collections.Generic;
using System.Linq;
using EawModinfo.Model.Steam;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Microsoft;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace EawModinfo.Model
{
    /// <inheritdoc/>
    [JsonObject(MemberSerialization.OptIn)]
    public class ModinfoData : IModinfo
    {
        [JsonIgnore] private readonly HashSet<LanguageInfo> _languages = new HashSet<LanguageInfo>();
        [JsonIgnore] private SemanticVersion? _modVersion;
        [JsonIgnore] private bool _versionDetermined;
        [JsonIgnore] private bool _languagesDetermined;

        [JsonIgnore] public bool HasCustomObjects => Custom.Count > 0;

        [JsonIgnore] public bool HasSteamData => SteamData != null;

        [JsonIgnore] public bool HasDependencies => Dependencies.Count > 0;

        /// <inheritdoc/>
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; internal set; }

        /// <inheritdoc/>
        [JsonProperty("summary")] 
        public string? Summary { get; internal set; }

        [JsonProperty("icon")] 
        public string? Icon { get; internal set; }

        [JsonProperty("version")] 
        private string? StringVersion { get; set; }

        /// <inheritdoc/>
        public SemanticVersion? Version
        {
            get
            {
                if (_modVersion is null && !_versionDetermined)
                {
                    _modVersion = string.IsNullOrEmpty(StringVersion) ? null : SemanticVersion.Parse(StringVersion);
                    _versionDetermined = true;
                }

                return _modVersion;
            }
            internal set
            {
                _modVersion = value;
                StringVersion = value?.ToFullString();
            }
        }

        /// <inheritdoc/>
        [JsonProperty("custom")] 
        public IDictionary<string, object> Custom { get; internal set; }

        /// <inheritdoc/>
        [JsonProperty("steamdata")]
        [JsonConverter(typeof(SteamDataTypeConverter))]
        public ISteamData? SteamData { get; internal set; }


        [JsonProperty("languages")]
        internal IEnumerable<LanguageInfo> InternalLanguages
        {
            get => _languages;
            set
            {
                _languages.Clear();
                foreach (var languageInfo in value) 
                    _languages.Add(languageInfo);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ILanguageInfo> Languages
        {
            get
            {
                if (!_languagesDetermined)
                {
                    if (!InternalLanguages.Any())
                        _languages.Add(new LanguageInfo(LanguageInfo.Default));
                    else
                        foreach (var languageInfo in InternalLanguages)
                            _languages.Add(languageInfo);

                    _languagesDetermined = true;
                }

                return _languages;
            }
            internal set
            {
                _languages.Clear();
                foreach (var languageInfo in value) 
                    _languages.Add(new LanguageInfo(languageInfo));
            }
        }

        /// <inheritdoc/>
        [JsonProperty("dependencies", ItemConverterType = typeof(ModReferenceTypeConverter))]
        public IList<IModReference> Dependencies { get; internal set; }

        [JsonConstructor]
        internal ModinfoData()
        {
            Dependencies = new List<IModReference>();
            Custom = new Dictionary<string, object>();
            InternalLanguages = new HashSet<LanguageInfo>();
        }

        /// <summary>
        /// Creates a new instance from a given <see cref="IModinfo"/> instance.
        /// </summary>
        /// <param name="modinfo">The instance that will copied.</param>
        public ModinfoData(IModinfo modinfo) : this()
        {
            Requires.NotNull(modinfo, nameof(modinfo));
            MergeFrom(modinfo, true);
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
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = ModInfoContractResolver.Instance
            });
        }

        /// <summary>
        /// Parses and deserializes a json data into a <see cref="ModinfoData"/>
        /// </summary>
        /// <param name="data">The raw json data.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
        public static ModinfoData Parse(string data)
        {
            return ParseUtility.Parse<ModinfoData>(data);
        }

        bool IEquatable<IModIdentity>.Equals(IModIdentity? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null)
                return false;
            if (!Name.Equals(other.Name))
                return false;
            if (!Equals(Version, other.Version))
                return false;

            if (Dependencies.Count != other.Dependencies.Count)
                return false;

            if (!Dependencies.SequenceEqual(other.Dependencies))
                return false;

            return true;
        }

        internal void MergeFrom(IModinfo target)
        {
            MergeFrom(target, false);
        }

        private void MergeFrom(IModinfo target, bool fromConstructor)
        {
            Name = target.Name;

            if (fromConstructor || !string.IsNullOrEmpty(target.Summary))
                Summary = target.Summary;

            if (fromConstructor || !string.IsNullOrEmpty(target.Icon))
                Icon = target.Icon;

            if (fromConstructor || target.Version != null)
            {
                if (target.Version != null)
                    Version = new SemanticVersion(target.Version);
            }

            if (fromConstructor || target.Custom.Any())
            {
                foreach (var customObject in target.Custom)
                {
                    if (!fromConstructor && Custom.Contains(customObject))
                        continue;
                    Custom.Add(customObject);
                }
            }

            if (fromConstructor || target.SteamData != null)
            {
                if (target.SteamData != null)
                    SteamData = new SteamData(target.SteamData);
            }

            if (fromConstructor || target.Dependencies.Any())
            {
                if (target.Dependencies.Any())
                    Dependencies = target.Dependencies.Select(x => (IModReference) new ModReference(x)).ToList();
            }

            if (fromConstructor || target.Languages.Any())
            {
                if (target.Languages.Any())
                {
#if NETSTANDARD2_1
                    Languages = target.Languages.Select(x => (ILanguageInfo) new LanguageInfo(x)).ToHashSet(null);
#else
                    Languages = target.Languages.Select(x => (ILanguageInfo) new LanguageInfo(x)).Distinct();
#endif
                }


            }
        }
    }
}