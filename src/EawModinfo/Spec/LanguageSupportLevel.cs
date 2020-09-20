using System;

namespace EawModinfo.Spec
{
    /// <summary>
    /// Flags that identify which parts of a mod is localized.
    /// </summary>
    [Flags]
    public enum LanguageSupportLevel
    {
        ///// <summary>
        ///// Same as <see cref="FullLocalized"/>
        ///// </summary>
        //Default = FullLocalized,
        /// <summary>
        /// There exists an dedicated "MasterTextFile" for this language
        /// </summary>
        Text = 1,
        /// <summary>
        /// Speech event files are in their own language folder.
        /// </summary>
        Speech = 2,
        /// <summary>
        /// Sound effects, such as unit actions, are localized.
        /// </summary>
        SFX = 4,
        /// <summary>
        /// Combines <see cref="Text"/>, <see cref="Speech"/> <see cref="SFX"/>
        /// </summary>
        FullLocalized = Text | Speech | SFX
    }
}