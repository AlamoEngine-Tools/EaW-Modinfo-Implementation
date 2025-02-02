using System;
using System.Collections.Generic;
using System.Linq;

namespace AET.Modinfo.Spec.Equality;

/// <summary>
/// Compares two <see cref="IModDependencyList"/> using a deeps equals strategy.
/// </summary>
public sealed class ModDependencyListEqualityComparer : IEqualityComparer<IModDependencyList>
{
    /// <summary>
    /// Returns the default instance of the <see cref="ModDependencyListEqualityComparer"/>
    /// </summary>
    /// <remarks>
    /// Implements the modinfo specification section III.1.1
    /// </remarks>
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
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));
        var hashCode = new HashCode();
        hashCode.Add(obj.ResolveLayout);
        foreach (var modRef in obj)
            hashCode.Add(modRef);
        return hashCode.ToHashCode();
    }
}