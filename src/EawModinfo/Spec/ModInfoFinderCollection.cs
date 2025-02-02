using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using AET.Modinfo.File;

namespace AET.Modinfo.Spec;

/// <summary>
/// Represents the result when a modinfo file query was performed.
/// </summary>
public sealed class ModinfoFinderCollection
{
    /// <summary>
    /// Returns <see langword="true"/> if this collection does contain a main modinfo file; <see langword="false"/> otherwise.
    /// </summary>
    [MemberNotNullWhen(true, nameof(MainModinfo))]
    public bool HasMainModinfoFile => MainModinfo != null;

    /// <summary>
    /// Returns <see langword="true"/> if this collection does contain a variant modinfo files; <see langword="false"/> otherwise.
    /// </summary>
    public bool HasVariantModinfoFiles => Variants.Any();

    /// <summary>
    /// Gets the main modinfo file or <see langword="null"/> if no main modinfo file was found.
    /// </summary>
    public MainModinfoFile? MainModinfo { get; }

    /// <summary>
    /// Gets a collection of found variant modinfo file.
    /// </summary>
    public IReadOnlyCollection<ModinfoVariantFile> Variants { get; }

    /// <summary>
    /// Gets the directory where the files were searched.
    /// </summary>
    public IDirectoryInfo Directory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModinfoFinderCollection"/> of the specified directory and found modinfo files.
    /// </summary>
    /// <param name="directory">The directory that was searched.</param>
    /// <param name="mainModinfo">The found main modinfo file or <see langword="null"/>.</param>
    /// <param name="variants">A collection of variant modinfo files.</param>
    /// <exception cref="ArgumentNullException"><paramref name="directory"/> or <paramref name="variants"/> is <see langword="null"/>.</exception>
    public ModinfoFinderCollection(IDirectoryInfo directory, MainModinfoFile? mainModinfo, IEnumerable<ModinfoVariantFile> variants)
    {
        if (variants == null)
            throw new ArgumentNullException(nameof(variants));
        Directory = directory ?? throw new ArgumentNullException(nameof(directory));
        MainModinfo = mainModinfo;
        Variants = variants.ToList();
    }
}