using System;
using System.Collections.Generic;
using System.Linq;

namespace EawModinfo.Spec.Equality;

/// <summary>
/// Compares two <see cref="IModDependencyList"/> using a deeps equals strategy.
/// </summary>
public sealed class ModDependencyListEqualityComparer : IEqualityComparer<IModDependencyList>
{
    /// <summary>
    /// The default instance of the <see cref="ModDependencyListEqualityComparer"/>
    /// </summary>
    public static readonly ModDependencyListEqualityComparer Default = new();

    private ModDependencyListEqualityComparer()
    {
    }

    /// <inheritdoc />
    public bool Equals(IModDependencyList? x, IModDependencyList? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;
        return x.ResolveLayout == y.ResolveLayout && x.SequenceEqual(y);
    }

    /// <inheritdoc />
    public int GetHashCode(IModDependencyList obj)
    {
        var hashCode = new HashCode();
        hashCode.Add(obj.ResolveLayout);
        foreach (var modRef in obj)
            hashCode.Add(modRef);
        return hashCode.ToHashCode();
    }
}