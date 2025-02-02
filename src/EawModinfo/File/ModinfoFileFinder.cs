using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using EawModinfo.Spec;
#if NETSTANDARD2_1
using System.Runtime.InteropServices;
#endif

namespace EawModinfo.File;

/// <summary>
/// Provides a method to searches a directory for modinfo files.
/// </summary>
public static class ModinfoFileFinder
{
    /// <summary>
    /// Searches the directory specified for all modinfo files according to the modinfo specification in section II.3.
    /// </summary>
    /// <param name="directory">The directory to search for modinfo files.</param>
    /// <returns>A collection with all found modinfo files.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="directory"/> is <see langword="null"/>.</exception>
    /// <exception cref="DirectoryNotFoundException"><paramref name="directory"/>does not exists.</exception>
    public static ModinfoFinderCollection FindModinfoFiles(IDirectoryInfo directory)
    {
        if (directory == null) 
            throw new ArgumentNullException(nameof(directory));
        if (!directory.Exists)
            throw new DirectoryNotFoundException($"Directory could not be found at '{directory.FullName}'");

        var mainModinfoFile = FindMainModinfoFileCore(directory);

        return new ModinfoFinderCollection(directory, mainModinfoFile, FindModinfoVariantFilesCore(directory, mainModinfoFile));
    }

    private static MainModinfoFile? FindMainModinfoFileCore(IDirectoryInfo directory)
    {
        IFileInfo? file;
#if NETSTANDARD2_1
        file = directory.EnumerateFiles(MainModinfoFile.ModinfoFileName).FirstOrDefault();

        // On linux, if 'modinfo.json' was not found, try to find the file with any casing, as a fallback. 
        if (file is null && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            file = directory.EnumerateFiles(MainModinfoFile.ModinfoFileName, new EnumerationOptions
            {
                MatchCasing = MatchCasing.CaseInsensitive
            }).FirstOrDefault();
        }

#else
        file = directory.EnumerateFiles(MainModinfoFile.ModinfoFileName, SearchOption.TopDirectoryOnly).FirstOrDefault();
#endif
        if (file is null)
            return null;

        var modinfo = new MainModinfoFile(file);
        return !modinfo.IsFileValid(out _) ? null : modinfo;
    }

    private static IEnumerable<ModinfoVariantFile> FindModinfoVariantFilesCore(IDirectoryInfo directory, MainModinfoFile? mainModinfoData)
    {
        var possibleVariants = directory.EnumerateFiles(
            $"*{ModinfoVariantFile.VariantModinfoFileEnding}",
            SearchOption.TopDirectoryOnly);

        foreach (var possibleVariant in possibleVariants)
        {
            var file = new ModinfoVariantFile(possibleVariant, mainModinfoData);
            if (file.IsFileValid(out _))
                yield return new ModinfoVariantFile(possibleVariant, mainModinfoData);
        }
    }
}