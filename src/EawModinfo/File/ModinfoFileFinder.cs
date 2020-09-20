using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EawModinfo.Spec;
using Microsoft;

namespace EawModinfo.File
{
    public sealed class ModinfoFileFinder : IModinfoFileFinder
    {
        /// <inheritdoc/>
        public DirectoryInfo Directory { get; set; }

        /// <inheritdoc/>
        public IModinfo? BaseModinfo { get; set; }

        public ModinfoFileFinder(DirectoryInfo directoryInfo, IModinfo? baseModinfo)
        {
            Directory = directoryInfo;
            BaseModinfo = baseModinfo;
        }

        public ModinfoFileFinder(DirectoryInfo directoryInfo) : this(directoryInfo, null)
        {
        }

        /// <summary>
        /// Searches for a main modinfo file (modinfo.json) at the given directory and creates a new <see cref="IModInfoFile"/> instance.
        /// </summary>
        /// <param name="directory">The directory where to search.</param>
        /// <returns>A new instance of a <see cref="IModInfoFile"/></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="directory"/> is null.</exception>
        /// <exception cref="DirectoryNotFoundException">When <paramref name="directory"/>does not exists.</exception>
        public static IModinfoFile? FindMain(DirectoryInfo directory)
        {
            Requires.NotNull(directory, nameof(directory));
            var result = CreateInstanceAndFind(directory, FindOptions.FindMain);
            return result.MainModInfo;
        }

        /// <summary>
        /// Searches for all variant modinfo files at the given directory and returns a new collection.
        /// </summary>
        /// <param name="directory">The directory where to search.</param>
        /// <returns>An collection with all found variant files.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="directory"/> is null.</exception>
        /// <exception cref="DirectoryNotFoundException">When <paramref name="directory"/>does not exists.</exception>
        public static ICollection<IModinfoFile> FindVariants(DirectoryInfo directory)
        {
            Requires.NotNull(directory, nameof(directory));
            var result = CreateInstanceAndFind(directory, FindOptions.FindVariants);
            return new List<IModinfoFile>(result.Variants);
        }

        /// <summary>
        /// Searches for all variant modinfo files at the given directory and returns a new collection.
        /// Each variant modinfo will get merged with passed <paramref name="baseModInfo"/>.
        /// </summary>
        /// <param name="directory">The directory where to search.</param>
        /// <param name="baseModInfo">The base <see cref="IModInfo"/> that shall get merged from.</param>
        /// <returns>An collection with all found variant files.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="directory"/> is null.</exception>
        /// <exception cref="DirectoryNotFoundException">When <paramref name="directory"/>does not exists.</exception>
        public static IEnumerable<IModinfoFile> FindVariants(DirectoryInfo directory, IModinfo? baseModInfo)
        {
            Requires.NotNull(directory, nameof(directory));
            var result = CreateInstanceAndFind(directory, FindOptions.FindVariants, baseModInfo);
            return new List<IModinfoFile>(result.Variants);
        }

        internal static ModInfoFinderCollection CreateInstanceAndFind(DirectoryInfo directory, FindOptions options, IModinfo? baseData = null)
        {
            var finder = new ModinfoFileFinder(directory, baseData);
            return finder.Find(options);
        }

        /// <inheritdoc/>
        public ModInfoFinderCollection Find(FindOptions findOptions)
        {
            return FindCore(findOptions);
        }

        /// <inheritdoc/>
        public ModInfoFinderCollection FindThrow(FindOptions findOptions)
        {
            var result = Find(findOptions);
            switch (findOptions)
            {
                case FindOptions.FindMain:
                    if (!result.HasMainModinfoFile)
                        throw new ModinfoException($"Unable to find a main modinfo file in: '{Directory.FullName}'");
                    break;
                case FindOptions.FindVariants:
                    if (!result.HasVariantModinfoFiles)
                        throw new ModinfoException($"Unable to find a variant modinfo files in: '{Directory.FullName}'");
                    break;
                case FindOptions.FindAny:
                    if (!result.HasMainModinfoFile && !result.HasVariantModinfoFiles)
                        throw new ModinfoException($"Unable to find a any modinfo file in: '{Directory.FullName}'");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(findOptions), findOptions, null);
            }
            return result;
        }
        
        private ModInfoFinderCollection FindCore(FindOptions options)
        {
            if (Directory is null)
                throw new DirectoryNotFoundException("Directory information must not be null.");
            if (!Directory.Exists)
                throw new DirectoryNotFoundException($"Directory could not be found at '{Directory.FullName}'");
            ModinfoFile? mainModInfoFile = FindMainModInfoFileCore();
            List<ModinfoVariantFile> variantFiles = new List<ModinfoVariantFile>();
            if (options.HasFlag(FindOptions.FindVariants))
                variantFiles.AddRange(FindModInfoVariantFilesCore(mainModInfoFile?.GetModInfo() ?? BaseModinfo));


            if (!options.HasFlag(FindOptions.FindMain))
                return new ModInfoFinderCollection(Directory, null, variantFiles);
            return new ModInfoFinderCollection(Directory, mainModInfoFile, variantFiles);
        }

        private MainModinfoFile? FindMainModInfoFileCore()
        {
            var file = Directory.EnumerateFiles(MainModinfoFile.ModInfoFileName, SearchOption.TopDirectoryOnly).FirstOrDefault();
            return file is null ? null : new MainModinfoFile(file);
        }
         
        private IEnumerable<ModinfoVariantFile> FindModInfoVariantFilesCore(IModinfo? mainModInfoData)
        {
            var possibleVariants = Directory.EnumerateFiles($"*{ModinfoVariantFile.VariantModInfoFileEnding}", SearchOption.TopDirectoryOnly).ToList();
            return from possibleVariant in possibleVariants select new ModinfoVariantFile(possibleVariant, mainModInfoData);
        }
    }
}