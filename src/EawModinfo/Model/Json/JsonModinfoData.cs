using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Semver;

namespace EawModinfo.Model.Json;

/// <inheritdoc/>
internal class JsonModinfoData : IModinfo
{
    [JsonIgnore] private HashSet<ILanguageInfo>? _languages;
    [JsonIgnore] private SemVersion? _modVersion;
    [JsonIgnore] private bool _versionDetermined;
    [JsonIgnore] private bool _languagesDetermined;

    /// <inheritdoc/>
    [JsonPropertyName("name")]
    [JsonRequired]
    [JsonInclude]
    public string Name { get; internal set; } = string.Empty;

    /// <inheritdoc/>
    [JsonPropertyName("summary")]
    [JsonInclude]
    public string? Summary { get; internal set; }

    [JsonPropertyName("version")]
    [JsonInclude]
    public string? StringVersion { get; set; }

    /// <inheritdoc/>
    [JsonIgnore]
    public SemVersion? Version
    {
        get
        {
            if (_modVersion is null && !_versionDetermined)
            {
                _modVersion = SemVerHelper.CreateSanitizedVersion(StringVersion);
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
    [JsonPropertyName("icon")]
    [JsonInclude]
    public string? Icon { get; internal set; }

    /// <inheritdoc/>
    [JsonPropertyName("dependencies")]
    [JsonConverter(typeof(DependencyListTypeConverter))]
    public IModDependencyList Dependencies { get; set; }

    [JsonPropertyName("languages")]
    public HashSet<JsonLanguageInfo> InternalLanguages { get; set; }

    /// <inheritdoc/>
    [JsonIgnore]
    public IEnumerable<ILanguageInfo> Languages
    {
        get
        {
            if (_languages is null && !_languagesDetermined)
            {
                var languages = new HashSet<ILanguageInfo>(InternalLanguages.Select(x => new LanguageInfo(x)));
                if (!languages.Any())
                    languages.Add(LanguageInfo.Default);
                _languages = languages;
                _languagesDetermined = true;
            }
            return _languages!;
        }
        set
        {
            var languages = new HashSet<JsonLanguageInfo>(value.Select(x => new JsonLanguageInfo(x)));
            if (languages.Count == 1 && languages.First().Equals(LanguageInfo.Default))
                languages.Clear();
            InternalLanguages = languages;
        }
    }

    /// <inheritdoc/>
    [JsonPropertyName("custom")]
    public IDictionary<string, object> Custom { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("steamdata")]
    [JsonConverter(typeof(SteamDataTypeConverter))]
    public ISteamData? SteamData { get; set; }

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
    /// <param name="modinfo">The instance that will be copied.</param>
    public JsonModinfoData(IModinfo modinfo) : this()
    {
        if (modinfo == null) 
            throw new ArgumentNullException(nameof(modinfo));
        Name = modinfo.Name;
        Summary = modinfo.Summary;
        Version = modinfo.Version;
        Icon = modinfo.Icon;
        Dependencies = modinfo.Dependencies;
        Languages = modinfo.Languages;
        Custom = modinfo.Custom;
        SteamData = modinfo.SteamData;
    }


    /// <inheritdoc/>
    public string ToJson()
    {
        this.Validate();
        return JsonSerializer.Serialize(this, ParseUtility.SerializerOptions);
    }
        
    bool IEquatable<IModIdentity>.Equals(IModIdentity? other)
    {
        return ModIdentityEqualityComparer.Default.Equals(this, other);
    }
}