using System;
using System.Collections.Generic;

namespace EawModinfo.Spec.Equality;

/// <summary>
/// Compares two <see cref="IModReference"/> by their identifier and mod type.
/// </summary>
/// <remarks>
/// Implements the modinfo specification section III.2.2
/// </remarks>
public sealed class ModReferenceEqualityComparer : IEqualityComparer<IModReference>
{
    /// <summary>
    /// The default instance of the <see cref="ModReferenceEqualityComparer"/>
    /// </summary>
    public static readonly ModReferenceEqualityComparer Default = new();

    private ModReferenceEqualityComparer()
    {
    }

    /// <inheritdoc />
    public bool Equals(IModReference? x, IModReference? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null)
            return false;
        if (y is null)
            return false;
        return x.Identifier == y.Identifier && x.Type == y.Type;
    }

    /// <inheritdoc />
    public int GetHashCode(IModReference obj)
    {
        return HashCode.Combine(obj.Identifier, obj.Type);
    }
}