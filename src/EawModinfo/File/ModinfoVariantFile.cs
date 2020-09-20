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
        public const string VariantModInfoFileEnding = "-modinfo.json";

        private readonly IModinfoFile? _mainModInfoFile;
        private IModinfo? _mainModInfoData;

        public override ModinfoFileKind FileKind => ModinfoFileKind.VariantFile;

        internal override IModFileNameValidator FileNameValidator => new Validator();

        public ModinfoVariantFile(FileInfo variant) : base(variant)
        {
        }

        public ModinfoVariantFile(FileInfo variant, IModinfoFile? mainModInfoFile) : base(variant)
        {
            if (mainModInfoFile?.FileKind == ModinfoFileKind.VariantFile)
                throw new ModinfoException("A ModInfoFile's base cannot be a variant file too.");
            _mainModInfoFile = mainModInfoFile;
        }

        public ModinfoVariantFile(FileInfo variant, IModinfo? mainModInfoData) : base(variant)
        {
            _mainModInfoData = mainModInfoData;
        }
        
        protected override async Task<IModinfo> GetModInfoCoreAsync()
        {
            var data = await base.GetModInfoCoreAsync();
            if (_mainModInfoData is null && _mainModInfoFile != null)
            {
                if (!(await _mainModInfoFile.GetModInfoAsync().ConfigureAwait(false) is ModinfoData mainData))
                    throw new ModinfoException($"Invalid Main Modinfo data: '{_mainModInfoFile.File.FullName}'");
                _mainModInfoData = mainData;
            }
            return data.MergeInto(_mainModInfoData);
        }

        protected override IModinfo GetModInfoCore()
        {
            var data = base.GetModInfoCore();
            if (_mainModInfoData is null && _mainModInfoFile != null)
            {
                var mainData = _mainModInfoFile.GetModInfo() as ModinfoData;
                _mainModInfoData = mainData ?? throw new ModinfoException($"Invalid Main Modinfo data: '{_mainModInfoFile.File.FullName}'");
            }
            return data.MergeInto(_mainModInfoData);
        }

        private class Validator : IModFileNameValidator
        {
            public bool Validate(string fileName, out string error)
            {
                error = string.Empty;
                if (!fileName.ToUpperInvariant().EndsWith(VariantModInfoFileEnding.ToUpperInvariant(),
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