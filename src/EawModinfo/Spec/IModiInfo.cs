using System.Collections.Generic;
using EawModinfo.Spec.Steam;

namespace EawModinfo.Spec;

/// <summary>
/// Represents structured data to provide metadata of a mod as specified in <see href="https://github.com/AlamoEngine-Tools/eaw.modinfo"/>.
/// </summary>
public interface IModinfo : IModIdentity, IConvertibleToJson
{
    /// <summary>
    /// Gets a short summary about the mod or <see langword="null"/> if none was provided.
    /// </summary>
    string? Summary { get; }

    /// <summary>
    /// Gets the path to the mod's icon file path or <see langword="null"/> if none was provided.
    /// </summary>
    string? Icon { get; }

    /// <summary>
    /// Gets a container which allows to define arbitrary data for 3rd party tool support.
    /// </summary>
    IDictionary<string, object> Custom { get; }

    /// <summary>
    ///  Gets the Steam information of mod or <see langword="null"/> if none was provided.
    /// </summary>
    ISteamData? SteamData { get; }

    /// <summary>
    /// Gets the supported languages of the mod.
    /// </summary>
    /// <remarks>
    /// If no other language infos are provided, a default language info gets returned. The default language info is English - FullLocalized.
    /// </remarks>
    IReadOnlyCollection<ILanguageInfo> Languages { get; }
}