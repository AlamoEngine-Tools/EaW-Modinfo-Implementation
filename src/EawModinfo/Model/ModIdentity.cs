using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using Semver;

namespace EawModinfo.Model;

/// <inheritdoc cref="IModIdentity"/> 
public sealed class ModIdentity : IModIdentity, IEquatable<ModIdentity>
{
    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public SemVersion? Version { get; init; }

    /// <inheritdoc />
    public IModDependencyList Dependencies { get; init; } = DependencyList.EmptyDependencyList;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModIdentity"/> class with a specified name.
    /// </summary>
    /// <param name="name">The name of the mod identity.</param>
    public ModIdentity(string name)
    {
        ThrowHelper.ThrowIfNullOrEmpty(name);
        Name = name;
    }

    /// <inheritdoc />
    public bool Equals(IModIdentity? other)
    {
        return ModIdentityEqualityComparer.Default.Equals(this, other);
    }

    /// <inheritdoc />
    public bool Equals(ModIdentity? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Equals((IModIdentity)other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ModIdentity other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return ModIdentityEqualityComparer.Default.GetHashCode(this);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Name:{Name}");
        if (Version is not null)
            sb.Append($" Version:{Version}");

        if (Dependencies.Count > 0)
            sb.Append(" [");

#if NETSTANDARD2_1
        sb.Append(string.Join(',', Dependencies));
#else
        sb.Append(string.Join(",", Dependencies));
#endif
        if (Dependencies.Count > 0)
            sb.Append("]");

        return sb.ToString();
    }
}