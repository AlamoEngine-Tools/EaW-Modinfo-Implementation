using System;
using System.IO.Abstractions;
using EawModinfo.Spec;

namespace EawModinfo.File;

/// <summary>
/// Represents a main "modinfo.json" file.
/// </summary>
public sealed class MainModinfoFile : ModinfoFile
{
    /// <summary>
    /// The file name for a main modinfo file.
    /// </summary>
    public const string ModinfoFileName = "modinfo.json";

    /// <inheritdoc/>
    public override ModinfoFileKind FileKind => ModinfoFileKind.MainFile;

    internal override IModinfoFileNameValidator FileNameValidator => new Validator();

    /// <summary>
    /// Initializes a new instance of the <see cref="MainModinfoFile"/> class with the specified file handle.
    /// </summary>
    /// <param name="modinfoFile">The file handle.</param>
    /// <exception cref="ArgumentNullException"><paramref name="modinfoFile"/> is <see langword="null"/>.</exception>
    public MainModinfoFile(IFileInfo modinfoFile) : base(modinfoFile)
    {
    }
        
    private class Validator : IModinfoFileNameValidator
    {
        public bool Validate(string fileName, out string error)
        {
            error = string.Empty;
            if (!fileName.Equals(ModinfoFileName, StringComparison.OrdinalIgnoreCase))
            {
                error = "The file's name must be 'modinfo.json'.";
                return false;
            }
            return true;
        }
    }
}