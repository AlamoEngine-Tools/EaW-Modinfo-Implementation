namespace EawModinfo.Spec
{
    /// <summary>
    /// Describes how a dependency list shall get interpreted and processed.
    /// </summary>
    public enum DependencyResolveLayout
    {
        /// <summary>
        /// The list shall only contain the mods direct ancestors.
        /// Each entry may have their own dependencies which shall get resolved recursively.
        /// </summary>
        ResolveRecursive,
        /// <summary>
        /// Only the last item in the list shall be recursively resolved.
        /// There shall be no resolving for any of the previous entries.
        /// </summary>
        ResolveLastItem,
        /// <summary>
        /// The list shall contain all ancestors of the target mod.
        /// The entries in the list shall be interpreted as is.
        /// </summary>
        FullResolved,
    }
}