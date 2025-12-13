using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AET.Modinfo.Spec;
using AET.Modinfo.Spec.Equality;
using AET.Modinfo.Spec.Steam;
using AET.Modinfo.Utilities;
using Semver;

namespace AET.Modinfo.Model.Json;

/// <inheritdoc/>
internal class JsonModinfoData : IModinfo
{
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
    [field: JsonIgnore]
    public SemVersion? Version
    {
        get
        {
            if (field is null && !_versionDetermined)
            {
                field = SemVerHelper.CreateSanitizedVersion(StringVersion);
                _versionDetermined = true;
            }

            return field;
        }
        internal set
        {
            field = value;
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
    public HashSet<JsonLanguageInfo>? InternalLanguages { get; set; }

    /// <inheritdoc/>
    [JsonIgnore]
    [field: JsonIgnore]
    public IReadOnlyCollection<ILanguageInfo> Languages
    {
        get
        {
            if (field is null && !_languagesDetermined)
            {
                if (InternalLanguages == null || InternalLanguages.Count == 0)
                    field = ModinfoData.UnsetLanguages;
                else
                    field = new HashSet<ILanguageInfo>(InternalLanguages.Select(x => new LanguageInfo(x)));
               
                _languagesDetermined = true;
            }
            return field!;
        }
#pragma warning disable CS9266 // The '{0}' accessor of property '{1}' should use 'field' because the other accessor is using it.
        set
        {
            var languages = new HashSet<JsonLanguageInfo>(value.Select(x => new JsonLanguageInfo(x)));
            if (languages.Count == 1 && languages.First().Equals(LanguageInfo.Default))
                languages.Clear();
            InternalLanguages = languages;
        }
#pragma warning restore CS9266
    }

    [JsonIgnore]
    public bool LanguagesExplicitlySet => !ReferenceEquals(Languages, ModinfoData.UnsetLanguages);

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


    public string ToJson()
    {
        this.Validate();
        return JsonSerializer.Serialize(this, ParseUtility.SerializerOptions);
    }

    public void ToJson(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        this.Validate();
        JsonSerializer.Serialize(stream, this, ParseUtility.SerializerOptions);
    }

    bool IEquatable<IModIdentity>.Equals(IModIdentity? other)
    {
        return ModIdentityEqualityComparer.Default.Equals(this, other);
    }
}