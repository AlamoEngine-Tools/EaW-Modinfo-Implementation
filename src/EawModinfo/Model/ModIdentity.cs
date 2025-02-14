﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using AET.Modinfo.Spec;
using AET.Modinfo.Spec.Equality;
using Semver;

namespace AET.Modinfo.Model;

/// <inheritdoc cref="IModIdentity"/> 
public sealed class ModIdentity : IModIdentity, IEquatable<ModIdentity>
{
    private readonly IModDependencyList _dependencies = DependencyList.EmptyDependencyList;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public SemVersion? Version { get; init; }

    /// <inheritdoc />
    public IModDependencyList Dependencies
    {
        get => _dependencies;
        init
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            _dependencies = new DependencyList(value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModIdentity"/> class with a specified name.
    /// </summary>
    /// <param name="name">The name of the mod identity.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
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