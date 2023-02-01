using System;
using Semver.Ranges;

namespace EawModinfo.Spec;

/// <summary>
/// Data type which is used for representing an external mod dependency. 
/// </summary>
public interface IModReference : IEquatable<IModReference>
{
    /// <summary>
    /// Unique identifier as a textual representation. The <see cref="Type"/> property indicates how the data shall get parsed. 
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// The <see cref="ModType"/> of this reference.
    /// </summary>
    ModType Type { get; }

    /// <summary>
    /// Optional, NPM-style compatible version range for this instance.
    /// 
<<<<<<< HEAD
<<<<<<< HEAD
    /// It's concrete data semantics is defined by the tool producing and using this property.
=======
    /// It concrete data semantics is defined by the tool producing and using this property.
>>>>>>> to c# 10 namespaces
=======
    /// It's concrete data semantics is defined by the tool producing and using this property.
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd
    /// </summary>
    /// <remarks>
    /// As stated in the specification this property is not used for equality matching.
    /// </remarks>
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
    SemVersionRange? VersionRange { get; }
=======
    SemanticVersioning.Range? VersionRange { get; }
>>>>>>> to c# 10 namespaces
=======
    SemVersionRange? VersionRange { get; }
>>>>>>> System text json (#134)
=======
    SemVersionRange? VersionRange { get; }
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd
}