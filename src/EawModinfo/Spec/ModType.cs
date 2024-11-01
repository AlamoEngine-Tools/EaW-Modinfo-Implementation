namespace EawModinfo.Spec;

/// <summary>
/// Represents possible flavors of which a mod can be. 
/// </summary>
public enum ModType
{
    /// <summary>
    /// The mod resides at a well known file system location (e.g. the game's .\Mods\ directory).
    /// The associated <see cref="IModReference.Identifier"/> should be able to parse into an relative or absolute file system path.
    /// </summary>
    Default = 0,
    /// <summary>
    /// The mod is a Steam Workshops Item.
    /// The associated <see cref="IModReference.Identifier"/> must be able to parse into an <see cref="uint"/>.
    /// </summary>
    Workshops = 1,
    /// <summary>
    /// A mod which does not exist on disk but may exist during a tool's runtime.
    /// </summary>
    Virtual = 2
}