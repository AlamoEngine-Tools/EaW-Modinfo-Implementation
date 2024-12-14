using System;
using System.IO.Abstractions;
using EawModinfo.Spec;

namespace EawModinfo.Utilities;

/// <summary>
/// Represents an information container for a mod reference with its associated directory and modinfo data.
/// </summary>
public sealed class DetectedModReference
{
    /// <summary>
    /// Gets the directory information of the mod reference.
    /// </summary>
    public IDirectoryInfo Directory { get; }

    /// <summary>
    /// Gets the optional modinfo data or <see langword="null"/> if none is present.
    /// </summary>
    public IModinfo? Modinfo { get; }

    /// <summary>
    /// Gets the mod reference.
    /// </summary>
    public IModReference ModReference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DetectedModReference"/> class with the specified mod reference, directory information and optional modinfo data
    /// </summary>
    /// <param name="modReference">The mod reference.</param>
    /// <param name="directory">The directory information of the mod reference.</param>
    /// <param name="modinfo">The optional modinfo data.</param>
    /// <exception cref="ArgumentNullException"><paramref name="modReference"/> or <paramref name="directory"/> is <see langword="null"/>.</exception>
    public DetectedModReference(IModReference modReference, IDirectoryInfo directory, IModinfo? modinfo)
    {
        ModReference = modReference ?? throw new ArgumentNullException(nameof(modReference));
        Directory = directory ?? throw new ArgumentNullException(nameof(directory));
        Modinfo = modinfo;
    }
}