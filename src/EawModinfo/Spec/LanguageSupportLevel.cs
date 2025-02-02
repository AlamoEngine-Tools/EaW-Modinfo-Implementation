using System;

namespace AET.Modinfo.Spec;

/// <summary>
/// Flags that define which parts of a mod are localized.
/// </summary>
[Flags]
public enum LanguageSupportLevel
{
    /// <summary>
    /// Text is localized for this language.
    /// </summary>
    Text = 1,
    /// <summary>
    /// Speech event files are localized for this language.
    /// </summary>
    Speech = 2,
    /// <summary>
    /// Unit voice-overs are localized for this language.
    /// </summary>
    SFX = 4,
    /// <summary>
    /// The mod is fully localized for this language, including text, speech and SFX.
    /// </summary>
    FullLocalized = Text | Speech | SFX,
    /// <summary>
    /// Same as <see cref="FullLocalized"/>.
    /// </summary>
    Default = FullLocalized
}