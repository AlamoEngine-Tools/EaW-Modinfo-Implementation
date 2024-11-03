using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace EawModinfo.Spec;

/// <summary>
/// Container that represents a modinfo file on the filesystem.
/// </summary>
public interface IModinfoFile
{
    /// <summary>
    /// Distinguishes the contained file between a "main" or "variant" modinfo file.
    /// </summary>
    ModinfoFileKind FileKind { get; }
        
    /// <summary>
    /// The file handle.
    /// </summary>
    IFileInfo File { get; }

    /// <summary>
    /// Checks whether the file exists and has a valid file name according to the modinfo specification.
    /// </summary>
    /// <remarks>The file's content is not validated.</remarks>
    /// <param name="error">When this method returns an error message is stored to this variable, or <see langword="null"/> if this method returns <see langword="true"/>.</param>
    /// <returns><see langword="true"/> if the file exists and its name is valid; otherwise, <see langword="false"/>.</returns>
    bool IsFileValid([NotNullWhen(false)] out string? error);

    /// <summary>
    /// Gets the content of the <see cref="File"/> and deserializes it into an <see cref="IModinfo"/>.
    /// </summary>
    /// <returns>The <see cref="IModinfo"/></returns>
    /// <exception cref="ModinfoException">Throws if it was not possible to get the<see cref="IModinfo"/> or the result was not valid.</exception>
    IModinfo GetModinfo();

    /// <summary>
    /// Asynchronously gets the content of the <see cref="File"/> and deserializes it into an <see cref="IModinfo"/>.
    /// </summary>
    /// <returns>The <see cref="IModinfo"/></returns>
    /// <exception cref="ModinfoException">Throws if it was not possible to get the<see cref="IModinfo"/> or the result was not valid.</exception>
    Task<IModinfo> GetModinfoAsync();

    /// <summary>
    /// Tries to get the content of the <see cref="File"/> and deserializes it into an <see cref="IModinfo"/>.
    /// </summary>
    /// <param name="modinfo">When this method returns, contains the deserialized modinfo or <see langword="null"/> if an error occurred.</param>
    /// <returns><see langword="true"/> when the file could be successfully deserialized to a modinfo; otherwise, <see langword="false"/>.</returns>
    bool TryGetModinfo([NotNullWhen(true)] out IModinfo? modinfo);
}