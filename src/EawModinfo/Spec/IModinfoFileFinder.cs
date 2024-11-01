using System.IO;
using System.IO.Abstractions;

namespace EawModinfo.Spec;

// TODO move properties to method params

/// <summary>
/// An instance that allows a certain directory to search for installed modinfo files.
/// </summary>
public interface IModinfoFileFinder
{
    /// <summary>
    /// Gets the directory where the <see cref="IModinfoFileFinder"/> will search. 
    /// </summary>
    IDirectoryInfo Directory { get; }

    /// <summary>
    /// Gets the base modinfo which, when not <see langword="null"/>, gets merged into found variant files. 
    /// </summary>
    IModinfo? BaseModinfo { get; }

    /// <summary>
    /// Searches <see cref="Directory"/> for installed modinfo files with the specified options.
    /// </summary>
    /// <param name="findOptions">Search option flags.</param>
    /// <returns>Result data which may have null/empty results for the request.</returns>
    /// <exception cref="DirectoryNotFoundException"><see cref="Directory"/> is null or not existent.</exception>
    ModinfoFinderCollection Find(FindOptions findOptions);
}