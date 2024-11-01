using System;
using System.Collections.Generic;

namespace EawModinfo.Spec.Equality;

/// <summary>
/// Compares two <see cref="ILanguageInfo"/>.
/// </summary>
public sealed class LanguageInfoEqualityComparer : IEqualityComparer<ILanguageInfo>
{
    private readonly bool _includeSupport;

    /// <summary>
    /// Returns a default equality comparer for a <see cref="ILanguageInfo"/> type.
    /// </summary>
    /// <remarks>
    /// Implements the modinfo specification section III.4.2
    /// </remarks>
    public static readonly LanguageInfoEqualityComparer Default = new(false);

    /// <summary>
    /// Returns an equality comparer for a <see cref="ILanguageInfo"/> type, which includes the support level.
    /// </summary>
    public static readonly LanguageInfoEqualityComparer WithSupportLevel = new(true);

    private LanguageInfoEqualityComparer(bool includeSupport)
    {
        _includeSupport = includeSupport;
    }

    /// <inheritdoc />
    public bool Equals(ILanguageInfo? x, ILanguageInfo? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;
        if (_includeSupport && x.Support != y.Support)
            return false;
        return string.Equals(x.Code, y.Code, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public int GetHashCode(ILanguageInfo obj)
    {
        var hash = new HashCode();
        hash.Add(obj.Code, StringComparer.OrdinalIgnoreCase);
        if (_includeSupport)
            hash.Add(obj.Support);
        return hash.ToHashCode();
    }
}