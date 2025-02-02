using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Threading.Tasks;
using EawModinfo.Model;
using EawModinfo.Spec;
using EawModinfo.Utilities;

namespace EawModinfo.File;

/// <summary>
/// Provides the base class for both <see cref="MainModinfoFile"/> and <see cref="ModinfoVariantFile"/> objects.
/// </summary>
public abstract class ModinfoFile : IModinfoFile
{
    private IModinfo? _data;

    /// <inheritdoc/>
    public abstract ModinfoFileKind FileKind { get; }

    /// <inheritdoc/>
    public IFileInfo File { get; }
    
    internal abstract IModinfoFileNameValidator FileNameValidator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModinfoFile"/> class with the specified file handle.
    /// </summary>
    /// <param name="file">The file handle.</param>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> is <see langword="null"/>.</exception>
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
    /// Asynchronously deserializes the file's content to an <see cref="IModinfo"/>.
    /// </summary>
    /// <returns>A task that represents the asynchronous read operation. The value of the task is the deserialized modinfo instance.</returns>
    protected virtual Task<IModinfo> GetModinfoCoreAsync()
    {
        return ParseAsync();
    }

    /// <summary>
    /// Deserializes the file's content to an <see cref="IModinfo"/>.
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