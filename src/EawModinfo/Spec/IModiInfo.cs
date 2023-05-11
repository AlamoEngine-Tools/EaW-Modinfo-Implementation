using System.Collections.Generic;
using EawModinfo.Spec.Steam;

namespace EawModinfo.Spec;

/// <summary>
/// Immutable definition of a modinfo as specified in <see href="https://github.com/AlamoEngine-Tools/eaw.modinfo"/>
/// <remarks>This interface always references the latest stable version of the specification.</remarks>
/// </summary>
public interface IModinfo : IModIdentity, IConvertibleToJson
{
    /// <summary>
    /// Short summary about the mod.
    /// </summary>
    string? Summary { get; }

    /// <summary>
    /// The path to the mod's icon file relative to the mod's root directory or an absolute path.
    /// </summary>
    string? Icon { get; }

    /// <summary>
    /// Container which allows to define arbitrary extensions for tool support.
    /// </summary>
    IDictionary<string, object> Custom { get; }

    /// <summary>
    ///  Holds additional info that is required for the Steam Version of the game. 
    /// </summary>
    ISteamData? SteamData { get; }

    /// <summary>
    /// Supported languages of the mod
    /// </summary>
    IEnumerable<ILanguageInfo> Languages { get; }
}