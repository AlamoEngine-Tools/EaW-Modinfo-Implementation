using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using AET.Modinfo.Spec;
using AET.Modinfo.Utilities;

namespace AET.Modinfo.File;

/// <summary>
/// Represents a variant modinfo file.
/// </summary>
public sealed class ModinfoVariantFile : ModinfoFile
{
    /// <summary>
    /// Suffix for variant modinfo file names, including file extension.
    /// </summary>
    public const string VariantModinfoFileEnding = "-modinfo.json";

    private readonly MainModinfoFile? _mainModinfoFile;
    private IModinfo? _mainModinfoData;

    /// <inheritdoc/>
    public override ModinfoFileKind FileKind => ModinfoFileKind.VariantFile;

    internal override IModinfoFileNameValidator FileNameValidator => new Validator();

    /// <summary>
    /// Initializes a new instance of the <see cref="ModinfoVariantFile"/> class with the specified file handle.
    /// </summary>
    /// <param name="file">The file handle.</param>
    /// <exception cref="ArgumentNullException"><paramref name="file"/>is <see langword="null"/>.</exception>
    public ModinfoVariantFile(IFileInfo file) : this(file, (IModinfo?) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModinfoVariantFile"/> class with the specified file handle on optional main modinfo file.
    /// </summary>
    /// <remarks>
    /// If <paramref name="mainModinfoFile"/> is not <see langword="null"/>, the deserialized modinfo of this file will inherit
    /// all properties from <paramref name="mainModinfoFile"/> unless not overwritten by this file.
    /// </remarks>
    /// <param name="file">The file handle.</param>
    /// <param name="mainModinfoFile">The main modinfo file or <see langword="null"/> if no main modinfo shall be used.</param>
    /// <exception cref="ArgumentNullException"><paramref name="file"/>is <see langword="null"/>.</exception>
    public ModinfoVariantFile(IFileInfo file, MainModinfoFile? mainModinfoFile) : base(file)
    {
        _mainModinfoFile = mainModinfoFile;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModinfoVariantFile"/> class with the specified file handle on optional modinfo data.
    /// </summary>
    /// <remarks>
    /// If <paramref name="mainModinfoData"/> is not <see langword="null"/>, the deserialized modinfo of this file will inherit
    /// all properties from <paramref name="mainModinfoData"/> unless not overwritten by this file.
    /// </remarks>
    /// <param name="file">The file handle.</param>
    /// <param name="mainModinfoData">The main modinfo data or <see langword="null"/> if no main modinfo shall be used.</param>
    /// <exception cref="ArgumentNullException"><paramref name="file"/>is <see langword="null"/>.</exception>
    public ModinfoVariantFile(IFileInfo file, IModinfo? mainModinfoData) : base(file)
    {
        _mainModinfoData = mainModinfoData;
    }

    /// <inheritdoc/>
    protected override async Task<IModinfo> GetModinfoCoreAsync()
    {
        var data = await base.GetModinfoCoreAsync().ConfigureAwait(false);
        if (_mainModinfoData is null && _mainModinfoFile != null)
        {
            var mainData = await _mainModinfoFile.GetModinfoAsync().ConfigureAwait(false);
            _mainModinfoData = mainData ?? throw new ModinfoException($"Invalid Main Modinfo data: '{_mainModinfoFile.File.FullName}'");
        }
        return data.MergeInto(_mainModinfoData);
    }

    /// <inheritdoc/>
    protected override IModinfo GetModinfoCore()
    {
        var data = base.GetModinfoCore();
        if (_mainModinfoData is null && _mainModinfoFile != null)
        {
            var mainData = _mainModinfoFile.GetModinfo();
            _mainModinfoData = mainData ?? throw new ModinfoException($"Invalid Main Modinfo data: '{_mainModinfoFile.File.FullName}'");
        }
        return data.MergeInto(_mainModinfoData);
    }

    private class Validator : IModinfoFileNameValidator
    {
        public bool Validate(string fileName, out string error)
        {
            error = string.Empty;
            if (!fileName.EndsWith(VariantModinfoFileEnding, StringComparison.OrdinalIgnoreCase))
            {
                error = "The file's name must end with '-modinfo.json'.";
                return false;
            }

            if (fileName.Length == VariantModinfoFileEnding.Length)
            {
                error = "The file's name cannot be just '-modinfo.json'";
                return false;
            }
            return true;
        }
    }
}