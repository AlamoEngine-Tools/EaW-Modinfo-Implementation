namespace EawModinfo.Model.Json.Schema;

/// <summary>
/// Specifies which schema to use for modinfo JSON validation.
/// </summary>
public enum EvaluationType
{
    /// <summary>
    /// The modinfo schema.
    /// </summary>
    ModInfo,
    /// <summary>
    /// The mod reference schema.
    /// </summary>
    ModReference,
    /// <summary>
    /// The mod dependency list schema.
    /// </summary>
    ModDependencyList,
    /// <summary>
    /// The mod language info schema.
    /// </summary>
    ModLanguageInfo,
    /// <summary>
    /// The steam data schema.
    /// </summary>
    SteamData
}