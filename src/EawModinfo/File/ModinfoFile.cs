using System.IO.Abstractions;
using System.Threading.Tasks;
using EawModinfo.Model;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using Validation;

namespace EawModinfo.File
{
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
            Requires.NotNull(file, nameof(file));
            File = file;
        }

        /// <inheritdoc/>
        public void ValidateFile()
        {
            if (!File.Exists)
                throw new ModinfoException($"Modinfo variant file does not exists at '{File.FullName}'.");
            if (!FileNameValidator.Validate(File.Name, out var error))
                throw new ModinfoException(error);
        }

        /// <inheritdoc/>
        public async Task<IModinfo> GetModinfoAsync()
        {
            ValidateFile();
            if (_data != null)
                return _data;
            _data = await GetModinfoCoreAsync().ConfigureAwait(false);
            _data.Validate();
            return _data;
        }

        /// <inheritdoc/>
        public IModinfo GetModinfo()
        {
            ValidateFile();
            if (_data != null)
                return _data;
            _data = GetModinfoCore();
            _data.Validate();
            return _data;
        }

        /// <inheritdoc/>
        public IModinfo? TryGetModinfo()
        {
            try
            {
                return GetModinfo();
            }
            catch
            {
                return null;
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

        private Task<string> ReadTextAsync()
        {
            var fs = File.FileSystem;

#if NET || NETSTANDARD2_1
            return fs.File.ReadAllTextAsync(File.FullName);
#else
            using var reader = fs.File.OpenText(File.FullName);
            return reader.ReadToEndAsync();
#endif
        }

        /// <inheritdoc/>
        public bool Equals(IModinfoFile? other)
        {
            return File.Equals(other?.File);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is null) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            return obj.GetType() == GetType() && Equals((ModinfoFile) obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return File.GetHashCode();
        }
    }
}
