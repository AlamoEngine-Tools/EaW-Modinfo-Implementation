using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using EawModinfo.Spec;
using Validation;

namespace EawModinfo.File
{
    /// <inheritdoc/>
    public sealed class ModinfoFileFinder : IModinfoFileFinder
    {
        /// <inheritdoc/>
        public IDirectoryInfo Directory { get; set; }

        /// <inheritdoc/>
        public IModinfo? BaseModinfo { get; set; }

        /// <summary>
        /// Creates a new <see cref="ModinfoFileFinder"/> instance
        /// </summary>
        /// <param name="directoryInfo">The directory where to search.</param>
        /// <param name="baseModinfo">The base <see cref="IModinfo"/> that shall get merged from, if present.</param>
        public ModinfoFileFinder(IDirectoryInfo directoryInfo, IModinfo? baseModinfo)
        {
            Directory = directoryInfo;
            BaseModinfo = baseModinfo;
        }

        /// <summary>
        /// Creates a new <see cref="ModinfoFileFinder"/> instance.
        /// </summary>
        /// <param name="directoryInfo">The directory where to search.</param>
        public ModinfoFileFinder(IDirectoryInfo directoryInfo) : this(directoryInfo, null)
        {
        }

        /// <summary>
        /// Searches for a main modinfo file (modinfo.json) at the given directory and creates a new <see cref="IModinfoFile"/> instance.
        /// </summary>
        /// <param name="directory">The directory where to search.</param>
        /// <returns>A new instance of a <see cref="IModinfoFile"/></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="directory"/> is null.</exception>
        /// <exception cref="DirectoryNotFoundException">When <paramref name="directory"/>does not exists.</exception>
        public static IModinfoFile? FindMain(IDirectoryInfo directory)
        {
            Requires.NotNull(directory, nameof(directory));
            var result = CreateInstanceAndFind(directory, FindOptions.FindMain);
            return result.MainModinfo;
        }

        /// <summary>
        /// Searches for all variant modinfo files at the given directory and returns a new collection.
        /// </summary>
        /// <param name="directory">The directory where to search.</param>
        /// <returns>An collection with all found variant files.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="directory"/> is null.</exception>
        /// <exception cref="DirectoryNotFoundException">When <paramref name="directory"/>does not exists.</exception>
        public static ICollection<IModinfoFile> FindVariants(IDirectoryInfo directory)
        {
            Requires.NotNull(directory, nameof(directory));
            var result = CreateInstanceAndFind(directory, FindOptions.FindVariants);
            return new List<IModinfoFile>(result.Variants);
        }

        /// <summary>
        /// Searches for all variant modinfo files at the given directory and returns a new collection.
        /// Each variant modinfo will get merged with passed <paramref name="baseModinfo"/>.
        /// </summary>
        /// <param name="directory">The directory where to search.</param>
        /// <param name="baseModinfo">The base <see cref="IModinfo"/> that shall get merged from.</param>
        /// <returns>An collection with all found variant files.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="directory"/> is null.</exception>
        /// <exception cref="DirectoryNotFoundException">When <paramref name="directory"/>does not exists.</exception>
        public static IEnumerable<IModinfoFile> FindVariants(IDirectoryInfo directory, IModinfo? baseModinfo)
        {
            Requires.NotNull(directory, nameof(directory));
            var result = CreateInstanceAndFind(directory, FindOptions.FindVariants, baseModinfo);
            return new List<IModinfoFile>(result.Variants);
        }

        internal static ModinfoFinderCollection CreateInstanceAndFind(IDirectoryInfo directory, FindOptions options, IModinfo? baseData = null)
        {
            var finder = new ModinfoFileFinder(directory, baseData);
            return finder.Find(options);
        }

        /// <inheritdoc/>
        public ModinfoFinderCollection Find(FindOptions findOptions)
        {
            return FindCore(findOptions);
        }

        /// <inheritdoc/>
        public ModinfoFinderCollection FindThrow(FindOptions findOptions)
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
        
        private ModinfoFinderCollection FindCore(FindOptions options)
        {
            if (Directory is null)
                throw new DirectoryNotFoundException("Directory information must not be null.");
            if (!Directory.Exists)
                throw new DirectoryNotFoundException($"Directory could not be found at '{Directory.FullName}'");
            ModinfoFile? mainModinfoFile = FindMainModinfoFileCore();
            List<ModinfoVariantFile> variantFiles = new List<ModinfoVariantFile>();
            if (options.HasFlag(FindOptions.FindVariants))
                variantFiles.AddRange(FindModinfoVariantFilesCore(mainModinfoFile?.GetModinfo() ?? BaseModinfo));


            if (!options.HasFlag(FindOptions.FindMain))
                return new ModinfoFinderCollection(Directory, null, variantFiles);
            return new ModinfoFinderCollection(Directory, mainModinfoFile, variantFiles);
        }

        private MainModinfoFile? FindMainModinfoFileCore()
        {
            var file = Directory.EnumerateFiles(MainModinfoFile.ModinfoFileName, SearchOption.TopDirectoryOnly).FirstOrDefault();
            return file is null ? null : new MainModinfoFile(file);
        }
         
        private IEnumerable<ModinfoVariantFile> FindModinfoVariantFilesCore(IModinfo? mainModinfoData)
        {
            var possibleVariants = Directory.EnumerateFiles($"*{ModinfoVariantFile.VariantModinfoFileEnding}", SearchOption.TopDirectoryOnly).ToList();
            return from possibleVariant in possibleVariants select new ModinfoVariantFile(possibleVariant, mainModinfoData);
        }
    }
}