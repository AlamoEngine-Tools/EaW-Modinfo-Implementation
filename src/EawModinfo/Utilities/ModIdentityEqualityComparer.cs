using System;
using System.Collections.Generic;
using System.Linq;
using EawModinfo.Spec;

namespace EawModinfo.Utilities;

/// <summary>
/// Compares two <see cref="IModIdentity"/>
/// </summary>
public class ModIdentityEqualityComparer : IEqualityComparer<IModIdentity>
{
    /// <summary>
    /// Compares two <see cref="IModIdentity"/> based on their name, version and dependencies. Name comparison is case-sensitive
    /// </summary>
    public static readonly ModIdentityEqualityComparer Default = new(true, true, StringComparison.Ordinal);

    private readonly bool _includeVersion;
    private readonly bool _includeDependencies;
    private readonly StringComparison _stringComparison;
        
    /// <summary>
    /// Creates a new <see cref="IModIdentity"/> equality comparer.
    /// </summary>
    /// <param name="includeVersion">Shall the versions get compared. When both versions are <see langword="null"/> the comparison matches</param>
    /// <param name="includeDependencies">Shall dependencies get compared.</param>
    /// <param name="stringComparison">Comparison mod for name equality.</param>
    public ModIdentityEqualityComparer(bool includeVersion, bool includeDependencies, StringComparison stringComparison)
    {
        _includeVersion = includeVersion;
        _includeDependencies = includeDependencies;
        _stringComparison = stringComparison;
    }
<<<<<<< HEAD

    /// <inheritdoc/>
    public bool Equals(IModIdentity? x, IModIdentity? y)
    {
        if (y is null || x is null)
            return false;
        if (ReferenceEquals(x, y))
            return true;

        if (!x.Name.Equals(y.Name, _stringComparison))
            return false;

        if (_includeVersion)
        {
            if (!(x.Version is null && y.Version is null))
            {
                if (!Equals(x.Version, y.Version))
                    return false;
            }
=======

    /// <inheritdoc/>
    public bool Equals(IModIdentity? x, IModIdentity? y)
    {
        if (y is null || x is null)
            return false;
        if (ReferenceEquals(x, y))
            return true;

        if (!x.Name.Equals(y.Name, _stringComparison))
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
            if (x.Dependencies.Count != y.Dependencies.Count)
                return false;

            if (!x.Dependencies.SequenceEqual(y.Dependencies))
                return false;
>>>>>>> to c# 10 namespaces
        }
        return true;
    }

<<<<<<< HEAD
        if (_includeDependencies)
        {
            if (x.Dependencies.Count != y.Dependencies.Count)
                return false;

            if (!x.Dependencies.SequenceEqual(y.Dependencies))
                return false;
=======
    /// <inheritdoc/>
    public int GetHashCode(IModIdentity obj)
    {
<<<<<<< HEAD
        unchecked
        {
            var hashCode = obj.Name.GetHashCode();
            if (_includeVersion) 
                hashCode = (hashCode * 397) ^ (obj.Version != null ? obj.Version.GetHashCode() : 0);
            if (_includeDependencies) 
                hashCode = (hashCode * 397) ^ obj.Dependencies.GetHashCode();
            return hashCode;
>>>>>>> to c# 10 namespaces
        }
        return true;
    }

    /// <inheritdoc/>
    public int GetHashCode(IModIdentity obj)
    {
=======
>>>>>>> System text json (#134)
        if (_includeVersion)
            return _includeDependencies
                ? HashCode.Combine(obj.Name, obj.Version, obj.Dependencies)
                : HashCode.Combine(obj.Name, obj.Version);
        return HashCode.Combine(obj.Name);
    }
}