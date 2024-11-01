using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;

namespace EawModinfo.Utilities;

/// <summary>
/// Provides validation methods for data types introduced by the modinfo specification.
/// </summary>
public static class ModinfoValidator
{
    /// <summary>
    /// Validates an <see cref="IModinfo"/> data. Throws an <see cref="ModinfoException"/> when validation failed.
    /// Also throws is subsequent data such as <see cref="ISteamData"/> are invalid if present.
    /// </summary>
    /// <param name="modinfo">The data to check</param>
    /// <exception cref="ModinfoException">When validation failed.</exception>
    public static void Validate(this IModinfo modinfo)
    {
        if (string.IsNullOrEmpty(modinfo.Name))
            throw new ModinfoException("Name must not be null or empty.");
        modinfo.SteamData?.Validate();
        foreach (var languageInfo in modinfo.Languages) 
            languageInfo.Validate();
        foreach (var dependency in modinfo.Dependencies)
            dependency.Validate();
    }

    /// <summary>
    /// Validates an <see cref="ISteamData"/> data. Throws an <see cref="ModinfoException"/> when validation failed.
    /// </summary>
    /// <param name="steamData">The data to check</param>
    /// <exception cref="ModinfoException">When validation failed.</exception>
    public static void Validate(this ISteamData steamData)
    {
        if (string.IsNullOrEmpty(steamData.Id))
            throw new ModinfoException("Steam data is invalid: Identifier is missing.");
        ValidateSteamId(steamData.Id, "Steam data is invalid: ");

        if (string.IsNullOrEmpty(steamData.ContentFolder))
            throw new ModinfoException("Steam data is invalid: ContentFolder is missing.");
        if (string.IsNullOrEmpty(steamData.Title))
            throw new ModinfoException("Steam data is invalid: Title is missing.");
        if (steamData.Tags == null || !steamData.Tags.Any())
            throw new ModinfoException("Steam data is invalid: No tags specified.");
        if (!steamData.Tags.Intersect(JsonSteamData.GameTags, StringComparer.InvariantCulture).Any())
            throw new ModinfoException("Steam data is missing game tag FOC or EAW");

        var tags = new HashSet<string>();
        var containsGame = false;
        foreach (var tag in steamData.Tags)
        {
            if (tag.Length > 255)
                throw new ModinfoException("A tag is longer than 255 characters.");
            if (ContainsInvalidCharacter(tag.AsSpan()))
                throw new ModinfoException("A tag contains a comma character.");
            if (!tags.Add(tag))
                throw new ModinfoException("The tag list contains duplicates.");
            if (tag is "EAW" or "FOC")
                containsGame = true;
        }
        if (!containsGame)
            throw new ModinfoException("Steam data is invalid: Title is missing.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsInvalidCharacter(ReadOnlySpan<char> value)
    {
        foreach (var c in value)
        {
            if (c == ',' || (uint)(c - '\x0020') > '\x007F' - '\x0020') // (c >= '\x0020' && c <= '\x007F'
                return true;
        }

        return false;
    }

    /// <summary>
    /// Validates an <see cref="IModReference"/> data. Throws an <see cref="ModinfoException"/> when validation failed.
    /// </summary>
    /// <remarks>
    /// When <see cref="IModReference.Type"/> is <see cref="ModType.Workshops"/>: <see cref="IModReference.Identifier"/> must parse into an <see cref="uint"/>
    /// However the value must also not be 0.
    /// <br></br>
    /// The validator will not check for if the <see cref="IModReference.Identifier"/> is a valid relative or absolute path.
    /// Tools will have to check this themselves based on the current operating system and file system.
    /// </remarks>
    /// <param name="modReference">The data to check</param>
    /// <exception cref="ModinfoException">When validation failed.</exception>
    public static void Validate(this IModReference modReference)
    {
        if (string.IsNullOrEmpty(modReference.Identifier))
            throw new ModinfoException("Mod-Reference data is invalid: Identifier is missing.");
        switch (modReference.Type)
        {
            case ModType.Workshops:
                ValidateSteamId(modReference.Identifier, "Mod-Reference data is invalid: ");
                break;
            case ModType.Default:
            case ModType.Virtual:
                break;
            default:
                throw new ModinfoException($"ERROR: Unknown ModType! ({modReference.Type})");
        }
    }

    /// <summary>
    /// Validates an <see cref="ILanguageInfo"/> data. Throws an <see cref="ModinfoException"/> when validation failed.
    /// </summary>
    /// <remarks>
    /// Validates whether the <see cref="ILanguageInfo.Code"/> has only two letters.
    /// <br></br>
    /// Validates whether the <see cref="ILanguageInfo.Code"/> can be parsed into a <see cref="CultureInfo"/>.
    /// <br></br>
    /// Validation fails on the <see cref="CultureInfo.InvariantCulture"/>.
    /// </remarks>
    /// <param name="languageInfo">The data to check</param>
    /// <exception cref="ModinfoException">When validation failed.</exception>
    public static void Validate(this ILanguageInfo languageInfo)
    {
        if (string.IsNullOrEmpty(languageInfo.Code))
            throw new ModinfoException("Language-Info data is invalid: Language Code is missing.");
        if (languageInfo.Code.Length != 2)
            throw new ModinfoException("Language-Info data is invalid: Code must be an ISO 639-1 two letter code.");
        if (languageInfo.Code.Equals("iv", StringComparison.InvariantCultureIgnoreCase))
            throw new ModinfoException($"Language-Info data is invalid: Code must not be an invariant culture '{languageInfo.Code}'");
        try
        {
            _ = new CultureInfo(languageInfo.Code);
        }
        catch (CultureNotFoundException)
        {
            throw new ModinfoException($"Language-Info data is invalid: {languageInfo.Code} is not a valid language code.");
        }
    }

    private static void ValidateSteamId(string data, string errorSourceMessage)
    {
        var (isValid, error) = ValidateSteamId(data);
        if (!isValid)
            throw new ModinfoException($"{errorSourceMessage}: {error}");
    }

    private static (bool Valid, string Error) ValidateSteamId(string data)
    {
        if (!uint.TryParse(data, out var id))
            return (false, "Workshops ID must be an uint number.");
        if (id == 0)
            return (false, "Workshops ID cannot be 0.");
        return (true, string.Empty);
    }
}