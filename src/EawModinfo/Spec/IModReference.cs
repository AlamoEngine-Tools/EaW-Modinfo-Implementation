using System;

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
    /// It concrete data semantics is defined by the tool producing and using this property.
    /// </summary>
    /// <remarks>
    /// As stated in the specification this property is not used for equality matching.
    /// </remarks>
    SemanticVersioning.Range? VersionRange { get; }
}