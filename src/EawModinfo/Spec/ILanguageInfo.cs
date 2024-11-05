using System;
using System.ComponentModel;

namespace EawModinfo.Spec;

/// <summary>
/// Represents a supported game language.
/// </summary>
/// <remarks>
/// According to the modinfo specification, for equality, only <see cref="Code"/> is relevant.
/// </remarks>
public interface ILanguageInfo : IEquatable<ILanguageInfo>, IConvertibleToJson
{
    // TODO: Validate in Schema
    /// <summary>
    /// Gets the ISO 639-1 two letter language code.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// Gets the level of how this language is supported by the mod.
    /// </summary>
    /// <remarks>If not other value is provided, the value <see cref="LanguageSupportLevel.FullLocalized"/> shall be returned as default.</remarks>
    [DefaultValue(LanguageSupportLevel.FullLocalized)]
    LanguageSupportLevel Support { get; }
}