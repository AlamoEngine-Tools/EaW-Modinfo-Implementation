using System.IO;
using EawModinfo.Spec;

namespace EawModinfo.File
{
    /// <inheritdoc/>
    public sealed class MainModinfoFile : ModinfoFile
    {
        public const string ModInfoFileName = "modinfo.json";
        
        public override ModinfoFileKind FileKind => ModinfoFileKind.MainFile;

        internal override IModFileNameValidator FileNameValidator => new Validator();

        public MainModinfoFile(FileInfo modInfoFile) : base(modInfoFile)
        {
        }
        
        private class Validator : IModFileNameValidator
        {
            public bool Validate(string fileName, out string error)
            {
                error = string.Empty;
                if (!fileName.ToUpperInvariant().Equals(ModInfoFileName.ToUpperInvariant()))
                {
                    error = "The file's name must be 'modinfo.json'.";
                    return false;
                }

                return true;
            }
        }
    }
}