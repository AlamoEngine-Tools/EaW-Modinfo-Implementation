using EawModinfo.Spec;
using Newtonsoft.Json;
using Validation;

namespace EawModinfo.Model.Json;

/// <inheritdoc/>
internal class JsonLanguageInfo : ILanguageInfo
{
    /// <inheritdoc/>
    [JsonProperty("code")] public string Code { get; private set; } = string.Empty;

    /// <inheritdoc/>
    [JsonProperty("support", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public LanguageSupportLevel Support { get; private set; }

       
    [JsonConstructor]
    internal JsonLanguageInfo()
    {
    }

    /// <summary>
    /// Creates a new instance from a given <see cref="ILanguageInfo"/> instance.
    /// </summary>
    /// <param name="languageInfo">The instance that will copied.</param>
    public JsonLanguageInfo(ILanguageInfo languageInfo)
    {
        Requires.NotNull(languageInfo, nameof(languageInfo));
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
        if (ReferenceEquals(this, obj)) return true;
        if (obj is ILanguageInfo info) return Equals(info);
        return false;

    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Code.ToLower().GetHashCode();
    }
}