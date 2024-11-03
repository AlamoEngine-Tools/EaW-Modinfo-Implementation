using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Threading.Tasks;
using EawModinfo.Model;
using EawModinfo.Spec;
using EawModinfo.Utilities;

namespace EawModinfo.File;

/// <summary>
/// Implementation of an <see cref="IModinfoFile"/>. The file's content will be loaded and validated lazily.
/// </summary>
public abstract class ModinfoFile : IModinfoFile
{
    private IModinfo? _data;

    /// <inheritdoc/>
    public abstract ModinfoFileKind FileKind { get; }

    /// <inheritdoc/>
    public IFileInfo File { get; }

    /// <summary>
    /// Validator for file names.
    /// </summary>
    internal abstract IModFileNameValidator FileNameValidator { get; }

    /// <summary>
    /// Creates a new <see cref="ModinfoFile"/> instance
    /// </summary>
    /// <param name="file">The file representation</param>
    protected ModinfoFile(IFileInfo file)
    {
        File = file ?? throw new ArgumentNullException(nameof(file));
    }

    /// <inheritdoc/>
    public async Task<IModinfo> GetModinfoAsync()
    {
        if (!IsFileValid(out var error))
            throw new ModinfoException(error);
        if (_data != null)
            return _data;
        _data = await GetModinfoCoreAsync().ConfigureAwait(false);
        _data.Validate();
        return _data;
    }


    /// <inheritdoc />
    public bool IsFileValid([NotNullWhen(false)]out string? error)
    {
        File.Refresh();
        if (!File.Exists)
        {
            error = $"The file '{File.FullName}' does not exist.";
            return false;
        }
        return FileNameValidator.Validate(File.Name, out error);
    }

    /// <inheritdoc/>
    public IModinfo GetModinfo()
    {
        if (!IsFileValid(out var error))
            throw new ModinfoException(error);
        if (_data != null)
            return _data;
        _data = GetModinfoCore();
        _data.Validate();
        return _data;
    }

    /// <inheritdoc/>
    public bool TryGetModinfo([NotNullWhen(true)] out IModinfo? modinfo)
    {
        try
        {
            modinfo = GetModinfo();
            return true;
        }
        catch
        {
            modinfo = null;
            return false;
        }
    }

    /// <summary>
    /// Deserializes the file's content async.
    /// </summary>
    /// <returns>The deserialization task.</returns>
    protected virtual Task<IModinfo> GetModinfoCoreAsync()
    {
        return ParseAsync();
    }

    /// <summary>
    /// Deserializes the file's content.
    /// </summary>
    /// <returns>The deserialized <see cref="IModinfo"/>.</returns>
    protected virtual IModinfo GetModinfoCore()
    {
        return Parse();
    }

    private async Task<IModinfo> ParseAsync()
    {
        var text = await ReadTextAsync().ConfigureAwait(false);
        return await Task.Run(() => ModinfoData.Parse(text)).ConfigureAwait(false);
    }

    private IModinfo Parse()
    {
        var fs = File.FileSystem;
        var text = fs.File.ReadAllText(File.FullName);
        return ModinfoData.Parse(text);
    }

    private async Task<string> ReadTextAsync()
    {
        var fs = File.FileSystem;

#if NETSTANDARD2_1
        return await fs.File.ReadAllTextAsync(File.FullName);
#else
        using var reader = fs.File.OpenText(File.FullName);
        return await reader.ReadToEndAsync();
#endif
    }
}