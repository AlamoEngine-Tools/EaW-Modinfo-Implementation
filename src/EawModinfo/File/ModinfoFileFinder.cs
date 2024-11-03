using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using EawModinfo.Spec;

namespace EawModinfo.File;

/// <summary>
/// Searches a directory for modinfo files.
/// </summary>
public static class ModinfoFileFinder
{
    /// <summary>
    /// Searches the directory specified for all modinfo files according to the modinfo specification in section III.3.
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
        return FindCore(directory);
    }

    private static ModinfoFinderCollection FindCore(IDirectoryInfo directory)
    {
        var mainModinfoFile = FindMainModinfoFileCore(directory);
        var variantFiles = new List<ModinfoVariantFile>(FindModinfoVariantFilesCore(directory, mainModinfoFile?.GetModinfo()));
        
        return new ModinfoFinderCollection(directory, mainModinfoFile, variantFiles);
    }

    private static MainModinfoFile? FindMainModinfoFileCore(IDirectoryInfo directory)
    {
        var file = directory.EnumerateFiles(MainModinfoFile.ModinfoFileName, SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (file is null)
            return null;
        
        var modinfo = new MainModinfoFile(file);
        return !modinfo.IsFileValid(out _) ? null : modinfo;
    }
         
    private static IEnumerable<ModinfoVariantFile> FindModinfoVariantFilesCore(IDirectoryInfo directory, IModinfo? mainModinfoData)
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