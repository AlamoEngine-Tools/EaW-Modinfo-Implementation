namespace AET.Modinfo.Spec;

/// <summary>
/// Defines how a dependency list shall get interpreted and processed.
/// </summary>
public enum DependencyResolveLayout
{
    /// <summary>
    /// The list only contains the mod's direct ancestors.
    /// Each entry may have their own dependencies which shall get resolved recursively.
    /// </summary>
    ResolveRecursive,
    /// <summary>
    /// Only the last item in the list get recursively resolved.
    /// All previous entries, if any, do not get resolved.
    /// </summary>
    ResolveLastItem,
    /// <summary>
    /// The list is already resolved. All entries in the list will be interpreted as is.
    /// </summary>
    FullResolved,
}