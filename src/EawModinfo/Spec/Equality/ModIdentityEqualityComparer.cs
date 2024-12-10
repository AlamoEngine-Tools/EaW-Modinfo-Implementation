using System;
using System.Collections.Generic;

namespace EawModinfo.Spec.Equality;

/// <summary>
/// Compares two <see cref="IModReference"/>.
/// </summary>
public sealed class ModIdentityEqualityComparer : IEqualityComparer<IModIdentity>
{
    /// <summary>
    /// The default instances of the <see cref="ModIdentityEqualityComparer"/>
    /// which compares two <see cref="IModIdentity"/> based on their name, version and dependencies.
    /// Name comparison is case-insensitive.
    /// </summary>
    /// <remarks>
    /// Implements the modinfo specification section III.1.1
    /// </remarks>
    public static readonly ModIdentityEqualityComparer Default = new(true, true, StringComparer.OrdinalIgnoreCase);

    private readonly bool _includeVersion;
    private readonly bool _includeDependencies;
    private readonly StringComparer _stringComparison;

    /// <summary>
    /// Creates a new <see cref="IModIdentity"/> equality comparer.
    /// </summary>
    /// <param name="includeVersion">Shall the versions get compared. When both versions are <see langword="null"/> the comparison matches</param>
    /// <param name="includeDependencies">Shall dependencies get compared.</param>
    /// <param name="stringComparison">Comparison mod for name equality.</param>
    public ModIdentityEqualityComparer(bool includeVersion, bool includeDependencies, StringComparer stringComparison)
    {
        _includeVersion = includeVersion;
        _includeDependencies = includeDependencies;
        _stringComparison = stringComparison;
    }

    /// <inheritdoc/>
    public bool Equals(IModIdentity? x, IModIdentity? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;
        if (!_stringComparison.Equals(x.Name, y.Name))
            return false;

        if (_includeVersion)
        {
            if (!(x.Version is null && y.Version is null))
            {
                if (!Equals(x.Version, y.Version))
                    return false;
            }
        }

        if (_includeDependencies)
        {
            if (!x.Dependencies.Equals(y.Dependencies))
                return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public int GetHashCode(IModIdentity obj)
    {
        var hashCode = new HashCode();
        hashCode.Add(obj.Name, _stringComparison);
        if (_includeVersion)
            hashCode.Add(obj.Version);
        if (_includeDependencies)
            hashCode.Add(obj.Dependencies);
        return hashCode.ToHashCode();
    }
}