using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Validation;
using Version = SemanticVersioning.Version;

namespace EawModinfo.Model.Json;

/// <inheritdoc/>
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
    [JsonPropertyName("name")]
    [JsonRequired]
    [JsonInclude]
    public string Name { get; internal set; } = string.Empty;

    /// <inheritdoc/>
    [JsonPropertyName("summary")] 
    public string? Summary { get; internal set; }

    /// <inheritdoc/>
    [JsonPropertyName("icon")] 
    public string? Icon { get; internal set; }

    [JsonPropertyName("version")]
    [JsonInclude]
    public string? StringVersion { get; set; }

    /// <inheritdoc/>
    [JsonIgnore]
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
    [JsonPropertyName("custom")] 
    public IDictionary<string, object> Custom { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("steamdata")]
    [JsonConverter(typeof(SteamDataTypeConverter))]
    public ISteamData? SteamData { get; set; }


    [JsonPropertyName("languages")]
    public IEnumerable<JsonLanguageInfo> InternalLanguages
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
    [JsonIgnore]
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
    [JsonPropertyName("dependencies")]
    [JsonConverter(typeof(DependencyListTypeConverter))]
    public IModDependencyList Dependencies { get; set; }

    [JsonConstructor]
    public JsonModinfoData()
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
        return JsonSerializer.Serialize(this, ParseUtility.SerializerOptions);
    }
        
    bool IEquatable<IModIdentity>.Equals(IModIdentity? other)
    {
        return ModIdentityEqualityComparer.Default.Equals(this, other);
    }
}