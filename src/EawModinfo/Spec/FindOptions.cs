using System;

namespace EawModinfo.Spec
{
    /// <summary>
    /// Flags used by an <see cref="IModinfoFileFinder"/> to change the search behavior.
    /// </summary>
    [Flags]
    public enum FindOptions
    {
        /// <summary>
        /// Search for a main modinfo file.
        /// </summary>
        FindMain = 1,
        /// <summary>
        /// Search for all variant modinfo files
        /// </summary>
        FindVariants = 2,
        /// <summary>
        /// Searches for a main and all variant modinfo files.
        /// </summary>
        FindAny = FindMain | FindVariants
    }
}