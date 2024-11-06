using System;
using System.Collections.Generic;

namespace EawModinfo.Spec;

/// <summary>
/// Represents a read-only list of mod references to specify a mod's dependencies.
/// </summary>
public interface IModDependencyList : IReadOnlyList<IModReference>, IEquatable<IModDependencyList>, IConvertibleToJson
{
    /// <summary>
    /// Gets the resolve layout of the <see cref="IModDependencyList"/> that describes how this dependency list shall get interpreted and processed.
    /// </summary>
    DependencyResolveLayout ResolveLayout { get; }
}