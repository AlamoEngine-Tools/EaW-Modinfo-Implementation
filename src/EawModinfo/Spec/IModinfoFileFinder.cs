using System.IO;

namespace EawModinfo.Spec
{
    /// <summary>
    /// An instance that allows a certain directory to search for installed modinfo files.
    /// </summary>
    public interface IModinfoFileFinder
    {
        /// <summary>
        /// The directory where the instance will search. 
        /// </summary>
        DirectoryInfo Directory { get; set; }

        /// <summary>
        /// When set, variant files will get merged with this data; 
        /// </summary>
        IModinfo? BaseModinfo { get; set; }

        /// <summary>
        /// Based on <paramref name="findOptions"/> searches the <see cref="Directory"/> for installed modinfo files.
        /// </summary>
        /// <param name="findOptions">Search option flags.</param>
        /// <returns>Result data which may have null/empty results for the request.</returns>
        /// <exception cref="DirectoryNotFoundException">Gets thrown when <see cref="Directory"/> is null or not existent.</exception>
        ModInfoFinderCollection Find(FindOptions findOptions);

        /// <summary>
        /// Based on <paramref name="findOptions"/> searches the <see cref="Directory"/> for installed modinfo files.
        /// Throws an <see cref="ModinfoException"/> if the result does not contain the requested data.
        /// If <see cref="FindOptions.FindAny"/> the exception will only be thrown if both requested data are not present.
        /// </summary>
        /// <param name="findOptions"></param>
        /// <returns>Result data</returns>
        /// <exception cref="DirectoryNotFoundException">Gets thrown when <see cref="Directory"/> is null or not existent.</exception>
        /// <exception cref="ModinfoException">Throws when on unsuccessful search.</exception>
        ModInfoFinderCollection FindThrow(FindOptions findOptions);

    }
}