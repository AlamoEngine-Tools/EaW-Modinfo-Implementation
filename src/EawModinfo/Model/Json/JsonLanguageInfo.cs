using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EawModinfo.Spec;
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
        Support = languageInfo.Support;
    }
        
    /// <inheritdoc/>
    public bool Equals(ILanguageInfo? other)
    {
        if (other is null) 
            return false;
        if (ReferenceEquals(this, other)) return true;
        return Code == other.Code;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null) 
            return false;
        if (ReferenceEquals(this, obj)) 
            return true;
        if (obj is ILanguageInfo info) 
            return Equals(info);
        return false;

    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Code.ToLower().GetHashCode();
    }

    public string ToJson(bool validate)
    {
        if (validate)
            this.Validate();
        return JsonSerializer.Serialize(this, ParseUtility.SerializerOptions);
    }
}