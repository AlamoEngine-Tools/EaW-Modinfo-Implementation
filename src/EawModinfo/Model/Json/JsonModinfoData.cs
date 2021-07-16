using System;
using System.Collections.Generic;
using System.Linq;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Newtonsoft.Json;
using Validation;
using Version = SemanticVersioning.Version;

namespace EawModinfo.Model.Json
{
    /// <inheritdoc/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class JsonModinfoData : IModinfo
    {
        [JsonIgnore] private readonly HashSet<ILanguageInfo> _languages = new();
        [JsonIgnore] private readonly HashSet<JsonLanguageInfo> _jsonLanguages = new();
        [JsonIgnore] private Version? _modVersion;
        [JsonIgnore] private bool _versionDetermined;
        [JsonIgnore] private bool _languagesDetermined;

        /// <summary>
        /// Returns <see langword="true"/> whether <see cref="Custom"/> has any contents; <see langword="false"/> otherwise.
        /// </summary>
        [JsonIgnore] public bool HasCustomObjects => Custom.Count > 0;

        /// <summary>
        /// Returns <see langword="true"/> whether <see cref="SteamData"/> is present; <see langword="false"/> otherwise.
        /// </summary>
        [JsonIgnore] public bool HasSteamData => SteamData != null;

        /// <summary>
        /// Returns <see langword="true"/> whether this instance has any dependencies; <see langword="false"/> otherwise.
        /// </summary>
        [JsonIgnore] public bool HasDependencies => Dependencies.Count > 0;

        /// <inheritdoc/>
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; internal set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("summary")] 
        public string? Summary { get; internal set; }

        /// <inheritdoc/>
        [JsonProperty("icon")] 
        public string? Icon { get; internal set; }

        [JsonProperty("version")] 
        private string? StringVersion { get; set; }

        /// <inheritdoc/>
        public Version? Version
        {
            get
            {
                if (_modVersion is null && !_versionDetermined)
                {
                    _modVersion =  SemVerHelper.CreateSanitizedVersion(StringVersion);
                    _versionDetermined = true;
                }

                return _modVersion;
            }
            internal set
            {
                _modVersion = value;
                StringVersion = value?.ToString();
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
        internal IEnumerable<JsonLanguageInfo> InternalLanguages
        {
            get => _jsonLanguages;
            set
            {
                _jsonLanguages.Clear();
                foreach (var languageInfo in value)
                    _jsonLanguages.Add(languageInfo);
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
        [JsonProperty("dependencies")]
        [JsonConverter(typeof(DependencyListTypeConverter))]
        public IModDependencyList Dependencies { get; internal set; }

        [JsonConstructor]
        internal JsonModinfoData()
        {
            Dependencies = new JsonDependencyList();
            Custom = new Dictionary<string, object>();
            InternalLanguages = new HashSet<JsonLanguageInfo>();
        }

        /// <summary>
        /// Creates a new instance from a given <see cref="IModinfo"/> instance.
        /// </summary>
        /// <param name="modinfo">The instance that will copied.</param>
        public JsonModinfoData(IModinfo modinfo) : this()
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


        /// <inheritdoc/>
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
        
        bool IEquatable<IModIdentity>.Equals(IModIdentity? other)
        {
            return ModIdentityEqualityComparer.Default.Equals(this, other);
        }
    }
}