using System;
using System.IO;
using System.Threading.Tasks;
using EawModinfo.Model;
using EawModinfo.Spec;
using EawModinfo.Utilities;

namespace EawModinfo.File
{
    /// <inheritdoc/>
    public sealed class ModinfoVariantFile : ModinfoFile
    {
        public const string VariantModinfoFileEnding = "-modinfo.json";

        private readonly IModinfoFile? _mainModinfoFile;
        private IModinfo? _mainModinfoData;

        public override ModinfoFileKind FileKind => ModinfoFileKind.VariantFile;

        internal override IModFileNameValidator FileNameValidator => new Validator();

        public ModinfoVariantFile(FileInfo variant) : base(variant)
        {
        }

        public ModinfoVariantFile(FileInfo variant, IModinfoFile? mainModinfoFile) : base(variant)
        {
            if (mainModinfoFile?.FileKind == ModinfoFileKind.VariantFile)
                throw new ModinfoException("A ModinfoFile's base cannot be a variant file too.");
            _mainModinfoFile = mainModinfoFile;
        }

        public ModinfoVariantFile(FileInfo variant, IModinfo? mainModinfoData) : base(variant)
        {
            _mainModinfoData = mainModinfoData;
        }
        
        protected override async Task<IModinfo> GetModinfoCoreAsync()
        {
            var data = await base.GetModinfoCoreAsync();
            if (_mainModinfoData is null && _mainModinfoFile != null)
            {
                if (!(await _mainModinfoFile.GetModinfoAsync().ConfigureAwait(false) is ModinfoData mainData))
                    throw new ModinfoException($"Invalid Main Modinfo data: '{_mainModinfoFile.File.FullName}'");
                _mainModinfoData = mainData;
            }
            return data.MergeInto(_mainModinfoData);
        }

        protected override IModinfo GetModinfoCore()
        {
            var data = base.GetModinfoCore();
            if (_mainModinfoData is null && _mainModinfoFile != null)
            {
                var mainData = _mainModinfoFile.GetModinfo() as ModinfoData;
                _mainModinfoData = mainData ?? throw new ModinfoException($"Invalid Main Modinfo data: '{_mainModinfoFile.File.FullName}'");
            }
            return data.MergeInto(_mainModinfoData);
        }

        private class Validator : IModFileNameValidator
        {
            public bool Validate(string fileName, out string error)
            {
                error = string.Empty;
                if (!fileName.ToUpperInvariant().EndsWith(VariantModinfoFileEnding.ToUpperInvariant(),
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    error = "The file's name must end with '-modinfo.json'.";
                    return false;
                }
                return true;
            }
        }
    }
}