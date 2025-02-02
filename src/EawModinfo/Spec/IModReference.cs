using System;
using Semver;

namespace AET.Modinfo.Spec;

/// <summary>
/// Represents a reference to a mod which can be used to describe a dependency. 
/// </summary>
public interface IModReference : IEquatable<IModReference>, IConvertibleToJson
{
    /// <summary>
    /// Gets the unique, predictable identifier of the mod.
    /// </summary>
    /// <remarks>
    /// The identifier can hold any data to uniquely identify a mod, however the concrete value shall be predictable,
    /// so that it can be used to code it into modinfo JSON files to reference mod dependencies.
    /// For Steam Workshop mods it is therefore recommended to use the Steam Workshops ID and for local 
    /// The <see cref="Type"/> property may indicate how the data can be interpreted.
    /// </remarks>
    string Identifier { get; }

    /// <summary>
    /// Gets the <see cref="ModType"/> of the mod reference.
    /// </summary>
    ModType Type { get; }

    /// <summary>
    /// Gets the NPM-style compatible version range for this instance or <see langword="null"/> is no version range was specified.
    /// </summary>
    /// <remarks>
    /// As stated in the specification this property is not used for equality matching.
    /// </remarks>
    SemVersionRange? VersionRange { get; }
}