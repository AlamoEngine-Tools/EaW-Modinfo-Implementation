using System;
using Semver;

namespace EawModinfo.Spec;

/// <summary>
/// Represents a reference to a mod which can be used to describe a dependency. 
/// </summary>
public interface IModReference : IEquatable<IModReference>, IConvertibleToJson
{
    /// <summary>
    /// Gets the unique identifier as a textual representation. The <see cref="Type"/> property my indicate how the data can be interpreted. 
    /// </summary>
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