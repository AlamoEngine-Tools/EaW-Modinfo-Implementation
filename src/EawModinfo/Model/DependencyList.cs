using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using EawModinfo.Utilities;

namespace EawModinfo.Model;

/// <inheritdoc cref="IModDependencyList"/>
public class DependencyList : ReadOnlyCollection<IModReference>, IModDependencyList, IEquatable<DependencyList>
{
    /// <summary>
    /// An empty dependency list singleton instance.
    /// </summary>
    public static readonly IModDependencyList EmptyDependencyList =
        new DependencyList(new List<IModReference>(), DependencyResolveLayout.FullResolved);

    /// <inheritdoc/>
    public DependencyResolveLayout ResolveLayout { get; }

    /// <summary>
    /// Creates a new instance from a given <see cref="IModDependencyList"/>.
    /// </summary>
    public DependencyList(IModDependencyList dependencyList) : base(dependencyList.ToList())
    {
        ResolveLayout = dependencyList.ResolveLayout;
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public DependencyList(IList<IModReference> dependencies, DependencyResolveLayout resolveLayout) 
        : base(dependencies)
    {
        ResolveLayout = resolveLayout;
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="DependencyList"/>.
    /// </summary>
    /// <param name="data">The raw json data.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">When parsing failed, e.g. due to missing required properties.</exception>
    public static DependencyList Parse(string data)
    {
        return new DependencyList(ParseUtility.Parse<JsonDependencyList>(data));
    }

    /// <inheritdoc />
    public bool Equals(DependencyList? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return ((IModDependencyList)this).Equals(other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not DependencyList other)
            return false;
        return Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return ModDependencyListEqualityComparer.Default.GetHashCode(this);
    }

    /// <inheritdoc />
    public bool Equals(IModDependencyList? other)
    {
        return ModDependencyListEqualityComparer.Default.Equals(this, other);
    }
}