namespace EawModinfo.Spec
{
    /// <summary>
    /// Identifier for a <see cref="IModinfoFile"/>
    /// </summary>
    public enum ModinfoFileKind
    {
        /// <summary>
        /// This is the "main" modinfo file, as described by the spec.
        /// </summary>
        MainFile,
        /// <summary>
        /// This is a "variant" modinfo file, as described by the spec.
        /// </summary>
        VariantFile
    }
}