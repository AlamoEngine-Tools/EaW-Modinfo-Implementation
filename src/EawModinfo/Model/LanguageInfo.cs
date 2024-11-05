using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using EawModinfo.Utilities;

namespace EawModinfo.Model;

/// <inheritdoc cref="ILanguageInfo"/>
public sealed class LanguageInfo : ILanguageInfo, IEquatable<LanguageInfo>
{
    private CultureInfo? _culture;

    /// <summary>
    /// Returns an instance of the <see cref="LanguageInfo"/> class representing full localized English.
    /// </summary>
    public static readonly LanguageInfo Default = new("en", LanguageSupportLevel.FullLocalized);

    /// <inheritdoc/>
    public string Code { get; }

    /// <inheritdoc/>
    public LanguageSupportLevel Support { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageInfo"/> class with of the specified language code and language support level.
    /// </summary>
    /// <param name="code">The ISO 639-1 two-letter code of the language.</param>
    /// <param name="supportLevel">The level of the supported language.</param>
    /// <exception cref="ArgumentNullException"><paramref name="code"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="code"/> is empty.</exception>
    public LanguageInfo(string code, LanguageSupportLevel supportLevel)
    {
        ThrowHelper.ThrowIfNullOrEmpty(code);
        if (supportLevel == 0)
            supportLevel = LanguageSupportLevel.Default;
        Code = code;
        Support = supportLevel;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageInfo"/> from the specified language info.
    /// </summary>
    /// <param name="languageInfo">The instance that will be copied.</param>
    public LanguageInfo(ILanguageInfo languageInfo)
    {
        if (languageInfo == null) 
            throw new ArgumentNullException(nameof(languageInfo));
        Code = languageInfo.Code;
        Support = languageInfo.Support == default ? LanguageSupportLevel.FullLocalized : languageInfo.Support;
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="LanguageInfo"/>.
    /// </summary>
    /// <param name="data">The raw json data.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    public static LanguageInfo Parse(string data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        var jsonData = ParseUtility.Parse<JsonLanguageInfo>(data);
        return new LanguageInfo(jsonData);
    }

    /// <summary>
    /// Gets a culture representation of the <see cref="Code"/> property.
    /// </summary>
    /// <exception cref="CultureNotFoundException"><see cref="Code"/> is not a valid culture name.</exception>
    public CultureInfo GetCulture()
    {
        return _culture ??= new CultureInfo(Code);
    }

    /// <inheritdoc/>
    public bool Equals(ILanguageInfo? other)
    {
        return LanguageInfoEqualityComparer.Default.Equals(this, other);
    }
    
    /// <inheritdoc />
    public bool Equals(LanguageInfo? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        return Equals((ILanguageInfo)other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        return obj is LanguageInfo other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return LanguageInfoEqualityComparer.Default.GetHashCode(this);
    }

    /// <inheritdoc/>
    public string ToJson()
    {
        return new JsonLanguageInfo(this).ToJson();
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"[Code:{Code}, Support:{Support}]";
    }
}