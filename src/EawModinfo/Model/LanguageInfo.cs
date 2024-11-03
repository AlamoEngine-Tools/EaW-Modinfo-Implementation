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
    /// <summary>
    /// Returns an <see cref="ILanguageInfo"/> which represents ENGLISH (en) where <see cref="ILanguageInfo.Support"/> is set to <see cref="LanguageSupportLevel.Default"/>
    /// </summary>
    public static readonly LanguageInfo Default = new() { Code = "en", Support = LanguageSupportLevel.FullLocalized };

    private CultureInfo? _culture;

    /// <summary>
    /// Gets a culture representation of the <see cref="Code"/> property.
    /// </summary>
    public CultureInfo Culture => _culture ??= new CultureInfo(Code);

    /// <inheritdoc/>
    public string Code { get; init; } = string.Empty;

    /// <inheritdoc/>
    public LanguageSupportLevel Support { get; init; }

    internal LanguageInfo()
    {
    }

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
    /// <param name="languageInfo">The instance that will be copied.</param>
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