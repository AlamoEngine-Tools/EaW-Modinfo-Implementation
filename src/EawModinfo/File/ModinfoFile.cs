using System.IO;
using System.Text;
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
        public FileInfo File { get; }

        /// <summary>
        /// Validator for file names.
        /// </summary>
        internal abstract IModFileNameValidator FileNameValidator { get; }

        protected ModinfoFile(FileInfo file)
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
        protected virtual async Task<IModinfo> GetModinfoCoreAsync()
        {
            return await ParseAsync().ConfigureAwait(false);
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
            var text = await ReadTextAsync(File).ConfigureAwait(false);
            return await Task.Run(() => ModinfoData.Parse(text)).ConfigureAwait(false);
        }

        private IModinfo Parse()
        {
            var text = System.IO.File.ReadAllText(File.FullName);
            return ModinfoData.Parse(text);
        }

        private static async Task<string> ReadTextAsync(FileSystemInfo fileInfo)
        {
            using var sourceStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            var sb = new StringBuilder();

            var buffer = new byte[0x1000];
            int numRead;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
            {
                var text = Encoding.Unicode.GetString(buffer, 0, numRead);
                sb.Append(text);
            }
            return sb.ToString();
        }

        public bool Equals(IModinfoFile other)
        {
            return File.Equals(other.File);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ModinfoFile) obj);
        }

        public override int GetHashCode()
        {
            return File.GetHashCode();
        }
    }
}
