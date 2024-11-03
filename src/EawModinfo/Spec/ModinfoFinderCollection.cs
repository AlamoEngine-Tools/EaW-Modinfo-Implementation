using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using EawModinfo.File;

namespace EawModinfo.Spec;

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
    /// The main modinfo file. This property may be <see langword="null"/>
    /// </summary>
    public MainModinfoFile? MainModinfo { get; }

    /// <summary>
    /// Unmodifiable collection of variant modinfo file.
    /// </summary>
    public IReadOnlyCollection<ModinfoVariantFile> Variants { get; }

    /// <summary>
    /// The directory where the files are from.
    /// </summary>
    public IDirectoryInfo Directory { get; }

    /// <summary>
    /// Creates a new instance of this collection.
    /// </summary>
    /// <param name="directory">The source directory.</param>
    /// <param name="mainModinfo">An optional main modinfo file.</param>
    /// <param name="variants">An enumeration of variant modinfo files.</param>
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