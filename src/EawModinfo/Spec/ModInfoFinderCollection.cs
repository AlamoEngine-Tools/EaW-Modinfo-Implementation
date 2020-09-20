using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft;

namespace EawModinfo.Spec
{
    /// <summary>
    /// Collection class which holds multiple <see cref="IModinfoFile"/>s, which implements <see cref="IEnumerable{T}"/>.
    /// When enumerating an instance of this class all stored modinfo files will get yielded.
    /// </summary>
    public sealed class ModInfoFinderCollection : IEnumerable<IModinfoFile>
    {
        /// <summary>
        /// Returns <see langword="true"/> if this collection does contain a main modinfo file; <see langword="false"/> otherwise.
        /// </summary>
        public bool HasMainModinfoFile => MainModInfo != null;

        /// <summary>
        /// Returns <see langword="true"/> if this collection does contain a variant modinfo files; <see langword="false"/> otherwise.
        /// </summary>
        public bool HasVariantModinfoFiles => Variants.Any();

        /// <summary>
        /// The main modinfo file. This property may be <see langword="null"/>
        /// </summary>
        public IModinfoFile? MainModInfo { get; }

        /// <summary>
        /// Unmodifiable collection of variant modinfo file.
        /// </summary>
        public IReadOnlyCollection<IModinfoFile> Variants { get; }

        /// <summary>
        /// The directory where the files are from.
        /// </summary>
        public DirectoryInfo Directory { get; }

        /// <summary>
        /// Creates a new instance of this collection.
        /// Throws an <see cref="ModinfoException"/> if: <br/>
        ///     a) <paramref name="mainModInfo"/> is not null but <see cref="IModinfoFile.FileKind"/> is not <see cref="ModinfoFileKind.MainFile"/> <br/>
        ///     b) <paramref name="variants"/> has any item where <see cref="IModinfoFile.FileKind"/> is not <see cref="ModinfoFileKind.VariantFile"/>
        /// </summary>
        /// <param name="directory">The source directory.</param>
        /// <param name="mainModInfo">A potential main modinfo file.</param>
        /// <param name="variants">An enumeration of variant modinfo files.</param>
        /// <exception cref="ModinfoException">When illegal data was passed.</exception>
        internal ModInfoFinderCollection(DirectoryInfo directory, IModinfoFile? mainModInfo, IEnumerable<IModinfoFile> variants)
        {
            Requires.NotNull(directory, nameof(directory));
            Requires.NotNull(variants, nameof(variants));
            Directory = directory;
            if (mainModInfo != null && mainModInfo.FileKind != ModinfoFileKind.MainFile)
                throw new ModinfoException($"A main modinfo file must be of kind {ModinfoFileKind.MainFile}");
            if (variants.Any(file => file.FileKind != ModinfoFileKind.VariantFile))
                throw new ModinfoException($"All variant modinfo files must be of kind {ModinfoFileKind.VariantFile}");
            MainModInfo = mainModInfo;
            Variants = variants.ToList();
        }

        internal ModInfoFinderCollection(DirectoryInfo directory, IModinfoFile mainModInfo) : 
            this(directory, mainModInfo, Enumerable.Empty<IModinfoFile>())
        {
        }

        internal ModInfoFinderCollection(DirectoryInfo directory, IEnumerable<IModinfoFile> variants) :
            this(directory, null, variants)
        {
        }

        /// <inheritdoc/>
        public IEnumerator<IModinfoFile> GetEnumerator()
        {
            if (MainModInfo != null)
                yield return MainModInfo;
            foreach (var variant in Variants)
                yield return variant;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}