using System;
using System.Collections.Generic;

namespace AET.Modinfo.Spec.Equality;

/// <summary>
/// Compares two <see cref="IModReference"/> by their identifier and mod type.
/// </summary>
/// <remarks>
/// Implements the modinfo specification section III.2.2
/// </remarks>
public sealed class ModReferenceEqualityComparer : IEqualityComparer<IModReference>
{
    /// <summary>
    /// Returns the default instance of the <see cref="ModReferenceEqualityComparer"/>.
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
        return x.Identifier.Equals(y.Identifier, StringComparison.OrdinalIgnoreCase) && x.Type == y.Type;
    }

    /// <inheritdoc />
    public int GetHashCode(IModReference obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));
        return HashCode.Combine(obj.Identifier.ToUpperInvariant(), obj.Type);
    }
}