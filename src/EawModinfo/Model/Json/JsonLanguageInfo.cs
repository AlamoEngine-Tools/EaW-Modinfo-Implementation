using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using EawModinfo.Utilities;

namespace EawModinfo.Model.Json;

/// <inheritdoc/>
internal class JsonLanguageInfo : ILanguageInfo
{
    /// <inheritdoc/>
    [JsonPropertyName("code")]
    public string Code { get; }

    /// <inheritdoc/>
    [JsonPropertyName("support")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LanguageSupportLevel Support { get; }

    [JsonConstructor]
    public JsonLanguageInfo(string code, LanguageSupportLevel support)
    {
        Code = code;
        if (support == 0)
            support = LanguageSupportLevel.Default;
        Support = support;
    }

       
    /// <summary>
    /// Creates a new instance from a given <see cref="ILanguageInfo"/> instance.
    /// </summary>
    /// <param name="languageInfo">The instance that will be copied.</param>
    public JsonLanguageInfo(ILanguageInfo languageInfo)
    {
        if (languageInfo == null)
            throw new ArgumentNullException(nameof(languageInfo));
        Code = languageInfo.Code;
        Support = languageInfo.Support is LanguageSupportLevel.FullLocalized ? default : languageInfo.Support;
    }
        
    /// <inheritdoc/>
    public bool Equals(ILanguageInfo? other)
    {
        return LanguageInfoEqualityComparer.Default.Equals(this, other);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) 
            return true;
        return obj is JsonLanguageInfo info && Equals(info);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return LanguageInfoEqualityComparer.Default.GetHashCode(this);
    }

    public string ToJson()
    {
        this.Validate();
        return JsonSerializer.Serialize(this, ParseUtility.SerializerOptions);
    }
}