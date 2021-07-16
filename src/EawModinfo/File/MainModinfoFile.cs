using System.IO.Abstractions;
using EawModinfo.Spec;

namespace EawModinfo.File
{
    /// <inheritdoc/>
    public sealed class MainModinfoFile : ModinfoFile
    {
        /// <summary>
        /// The file name for a main modinfo file.
        /// </summary>
        public const string ModinfoFileName = "modinfo.json";

        /// <inheritdoc/>
        public override ModinfoFileKind FileKind => ModinfoFileKind.MainFile;

        internal override IModFileNameValidator FileNameValidator => new Validator();

        /// <inheritdoc/>
        public MainModinfoFile(IFileInfo modinfoFile) : base(modinfoFile)
        {
        }
        
        private class Validator : IModFileNameValidator
        {
            public bool Validate(string fileName, out string error)
            {
                error = string.Empty;
                if (!fileName.ToUpperInvariant().Equals(ModinfoFileName.ToUpperInvariant()))
                {
                    error = "The file's name must be 'modinfo.json'.";
                    return false;
                }

                return true;
            }
        }
    }
}