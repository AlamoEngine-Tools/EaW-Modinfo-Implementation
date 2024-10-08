using System.Globalization;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Utilities;

namespace EawModinfo.Model;

/// <inheritdoc cref="ILanguageInfo"/>
public sealed record LanguageInfo : ILanguageInfo
{
    /// <summary>
    /// Returns an <see cref="ILanguageInfo"/> which represents ENGLISH (en) where <see cref="ILanguageInfo.Support"/> is set to <see cref="LanguageSupportLevel.Default"/>
    /// </summary>
    public static readonly ILanguageInfo Default = new LanguageInfo { Code = "en", Support = LanguageSupportLevel.FullLocalized };

    private CultureInfo? _culture;

    /// <summary>
    /// Gets a culture representation of the <see cref="Code"/> property.
    /// </summary>
    public CultureInfo Culture => _culture ??= new CultureInfo(Code);

    /// <inheritdoc/>
    public string Code { get; init; } = string.Empty;

    /// <inheritdoc/>
    public LanguageSupportLevel Support { get; init; }

    internal LanguageInfo() { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public LanguageInfo(string code, LanguageSupportLevel supportLevel)
    {
        Code = code;
        Support = supportLevel;
    }

    /// <summary>
    /// Creates a new instance from a given <see cref="ILanguageInfo"/> instance.
    /// </summary>
    /// <param name="languageInfo">The instance that will copied.</param>
    public LanguageInfo(ILanguageInfo languageInfo)
    {
        Code = languageInfo.Code;
        Support = languageInfo.Support;
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="LanguageInfo"/>
    /// </summary>
    /// <param name="data">The raw json data.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
    public static LanguageInfo Parse(string data)
    {
        var jsonData = ParseUtility.Parse<JsonLanguageInfo>(data);
        return new LanguageInfo(jsonData);
    }

    /// <inheritdoc/>
    public bool Equals(ILanguageInfo? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other)) return true;
        return Code == other.Code;
    }

    /// <inheritdoc />
    public bool Equals(LanguageInfo? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        if (other is ILanguageInfo info) 
            return Equals(info);
        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Code.ToLower().GetHashCode();
    }

    /// <inheritdoc/>
    public string ToJson(bool validate)
    {
        return new JsonLanguageInfo(this).ToJson(validate);
    }
}