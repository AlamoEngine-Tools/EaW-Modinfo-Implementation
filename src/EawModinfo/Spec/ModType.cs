namespace AET.Modinfo.Spec;

/// <summary>
/// Represents the possible locations of mod instance. 
/// </summary>
public enum ModType
{
    /// <summary>
    /// The mod îs stored on the file system at a well known file system location (e.g. the game's 'Mods' directory).
    /// </summary>
    Default = 0,
    /// <summary>
    /// The mod is a Steam Workshops Item.
    /// </summary>
    Workshops = 1,
    /// <summary>
    /// A mod which does not exist on physically disk but may exist during a tool's runtime, e.g, by composition of multiple mods.
    /// </summary>
    Virtual = 2
}