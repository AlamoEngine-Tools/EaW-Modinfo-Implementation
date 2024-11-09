using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace EawModinfo.Spec;

/// <summary>
/// Represents a file handle represents a modinfo file on the filesystem.
/// </summary>
public interface IModinfoFile
{
    /// <summary>
    /// Gets the file kind of the modinfo file which is either <see cref="ModinfoFileKind.MainFile"/> or <see cref="ModinfoFileKind.VariantFile"/>.
    /// </summary>
    ModinfoFileKind FileKind { get; }
        
    /// <summary>
    /// Gets the underlying file handle.
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
    /// Gets the content of the file and deserializes it into an <see cref="IModinfo"/>.
    /// </summary>
    /// <returns>The deserialized <see cref="IModinfo"/>.</returns>
    /// <exception cref="ModinfoException">The file does not exist, has an invalid name or its content is not a valid modinfo instance.</exception>
    IModinfo GetModinfo();

    /// <summary>
    /// Asynchronously gets the content of the file and deserializes it into an <see cref="IModinfo"/>.
    /// </summary>
    /// <returns>A task that represents the asynchronous read operation. The value of the task is the deserialized modinfo instance.</returns>
    /// <exception cref="ModinfoException">The file does not exist, has an invalid name or its content is not a valid modinfo instance.</exception>
    Task<IModinfo> GetModinfoAsync();

    /// <summary>
    /// Tries to get the content of the file and deserializes it into an <see cref="IModinfo"/>.
    /// </summary>
    /// <param name="modinfo">When this method returns, contains the deserialized modinfo or <see langword="null"/> if an error occurred.</param>
    /// <returns><see langword="true"/> when the file could be successfully deserialized to a modinfo; otherwise, <see langword="false"/>.</returns>
    bool TryGetModinfo([NotNullWhen(true)] out IModinfo? modinfo);
}