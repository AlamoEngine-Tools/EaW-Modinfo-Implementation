using System;
using System.IO;
using System.Threading.Tasks;

namespace EawModinfo.Spec
{
    /// <summary>
    /// Container that represents a modinfo file on the filesystem.
    /// </summary>
    public interface IModinfoFile : IEquatable<IModinfoFile>
    {
        /// <summary>
        /// Distinguishes the contained file between a "main" or "variant" modinfo file.
        /// </summary>
        public ModinfoFileKind FileKind { get; }
        
        /// <summary>
        /// The file handle.
        /// </summary>
        FileInfo File { get; }

        /// <summary>
        /// Validates whether the file exists and has a valid name. Throws <see cref="ModinfoException"/> on fail.
        /// <remarks>This does not validate the correctness of the file's content.</remarks>
        /// <exception cref="ModinfoException">Throws this exception when validation fails.</exception>
        /// </summary>
        void ValidateFile();

        /// <summary>
        /// Gets the content of the <see cref="File"/> and deserializes it into an <see cref="IModInfo"/>.
        /// </summary>
        /// <returns>The <see cref="IModInfo"/></returns>
        /// <exception cref="ModinfoException">Throws if it was not possible to get the<see cref="IModInfo"/> or the result was not valid.</exception>
        IModinfo GetModInfo();

        /// <summary>
        /// Asynchronously gets the content of the <see cref="File"/> and deserializes it into an <see cref="IModInfo"/>.
        /// </summary>
        /// <returns>The <see cref="IModInfo"/></returns>
        /// <exception cref="ModinfoException">Throws if it was not possible to get the<see cref="IModInfo"/> or the result was not valid.</exception>
        Task<IModinfo> GetModInfoAsync();

        /// <summary>
        /// Tries to get the content of the <see cref="File"/> and deserializes it into an <see cref="IModInfo"/>.
        /// </summary>
        /// <returns>An <see cref="IModInfo"/> if operation was successful; <see langword="null"/> otherwise.</returns>
        /// <exception cref="ModinfoException">Throws if it was not possible to get the<see cref="IModInfo"/> or the result was not valid.</exception>
        IModinfo? TryGetModInfo();
    }
}