using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using EawModinfo.Spec;
using EawModinfo.Utilities;

namespace EawModinfo.File;

/// <summary>
/// A <see cref="ModinfoFile"/> representing a variant modinfo file.
/// </summary>
public sealed class ModinfoVariantFile : ModinfoFile
{
    /// <summary>
    /// Postfix for variant modinfo file names, including file extension.
    /// </summary>
    public const string VariantModinfoFileEnding = "-modinfo.json";

    private readonly MainModinfoFile? _mainModinfoFile;
    private IModinfo? _mainModinfoData;

    /// <inheritdoc/>
    public override ModinfoFileKind FileKind => ModinfoFileKind.VariantFile;

    internal override IModFileNameValidator FileNameValidator => new Validator();

    /// <summary>
    /// Creates a file modinfo file from on a handle.
    /// </summary>
    /// <param name="file">The file handle.</param>
    public ModinfoVariantFile(IFileInfo file) : this(file, (IModinfo?) null)
    {
    }

    /// <summary>
    /// Creates a file modinfo file from on a handle with a main modinfo file 
    /// </summary>
    /// <param name="file">The file handle.</param>
    /// <param name="mainModinfoFile">The main modinfo file</param>
    public ModinfoVariantFile(IFileInfo file, MainModinfoFile? mainModinfoFile) : base(file)
    {
        _mainModinfoFile = mainModinfoFile;
    }

    /// <summary>
    /// Creates a file modinfo file from on a handle with a main modinfo data 
    /// </summary>
    /// <param name="file">The file handle.</param>
    /// <param name="mainModinfoData">The main modinfo data</param>
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

    private class Validator : IModFileNameValidator
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