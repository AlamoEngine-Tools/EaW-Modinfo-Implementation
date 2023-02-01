using System;
using Semver;

namespace EawModinfo.Spec;

/// <summary>
/// Minimal data type that is required to uniquely identify a mod.
/// </summary>
public interface IModIdentity : IEquatable<IModIdentity>
{
    /// <summary>
    /// The name of the mod.
    /// <remarks>It's recommended to ensure that the name is globally unique.</remarks>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The version of the mod.
    /// <remarks>Since mod developers cannot be forced to specify a version this property is nullable.</remarks>
    /// </summary>
    SemVersion? Version { get; }

    /// <summary>
    /// Ordered list of <see cref="IModReference"/>s which this mod is dependent on.
    /// Returns an empty list if the mod has no dependencies.
    /// </summary>
    IModDependencyList Dependencies { get; }
}