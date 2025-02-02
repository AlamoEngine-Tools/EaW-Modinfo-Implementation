using System;
using Semver;

namespace AET.Modinfo.Spec;

/// <summary>
/// Represents the minimal data that is required to uniquely identify a mod.
/// </summary>
public interface IModIdentity : IEquatable<IModIdentity>
{
    /// <summary>
    /// Gets the name of the mod.
    /// <remarks>It's recommended to ensure that the name is globally unique.</remarks>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the version of the mod or <see langword="null"/> if not version was provided.
    /// </summary>
    SemVersion? Version { get; }

    /// <summary>
    /// Gets the mod's dependencies.
    /// </summary>
    IModDependencyList Dependencies { get; }
}