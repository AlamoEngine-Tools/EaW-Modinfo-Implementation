namespace EawModinfo.Spec;

/// <summary>
/// Represents the possible locations of mod instance. 
/// </summary>
public enum ModType
{
    /// <summary>
    /// The mod îs stored on the file system at a well known file system location (e.g. the game's 'Mods' directory).
    /// The associated <see cref="IModReference.Identifier"/> should be able to parse into a relative or absolute file system path.
    /// </summary>
    Default = 0,
    /// <summary>
    /// The mod is a Steam Workshops Item.
    /// The associated <see cref="IModReference.Identifier"/> <b>must</b> be able to parse into an <see cref="uint"/> which represents a Steam Workshops identifier.
    /// </summary>
    Workshops = 1,
    /// <summary>
    /// A mod which does not exist on physically disk but may exist during a tool's runtime, e.g, by composition of multiple mods.
    /// </summary>
    Virtual = 2
}