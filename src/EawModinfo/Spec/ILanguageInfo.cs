using System;
using System.ComponentModel;

namespace EawModinfo.Spec
{
    /// <summary>
    /// Data type that represents a supported language.
    /// <br></br>
    /// Implements <see cref="IEquatable{T}"/>. For equality, only <see cref="Code"/> is relevant.
    /// </summary>
    public interface ILanguageInfo : IEquatable<ILanguageInfo>
    {
        /// <summary>
        /// The ISO 639-1 two letter language code.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// The level of which this language is supported by the mod.
        /// </summary>
        /// <remarks>Default value is <see cref="LanguageSupportLevel.FullLocalized"/></remarks>
        [DefaultValue(LanguageSupportLevel.FullLocalized)]
        LanguageSupportLevel Support { get; }
    }
}