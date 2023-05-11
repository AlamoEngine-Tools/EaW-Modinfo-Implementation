using EawModinfo.Spec;
using EawModinfo.Utilities;
using Semver;
using Validation;

namespace EawModinfo.Model;

/// <inheritdoc cref="IModIdentity"/> 
public sealed record ModIdentity : IModIdentity
{
    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public SemVersion? Version { get; init; }

    /// <inheritdoc />
    public IModDependencyList Dependencies { get; init; } = DependencyList.EmptyDependencyList;

    /// <summary>
    /// Creates a new instance with a given name.
    /// </summary>
    /// <param name="name"></param>
    public ModIdentity(string name)
    {
        Requires.NotNullOrEmpty(name, nameof(name));
        Name = name;
    }

    /// <summary>
    /// Creates a new instance from a given <see cref="IModIdentity"/> instance.
    /// </summary>
    /// <param name="modIdentity">The instance that will copied.</param>
    public ModIdentity(IModIdentity modIdentity)
    {
        Requires.NotNull(modIdentity, nameof(modIdentity));
        Name = modIdentity.Name;
        Version = modIdentity.Version;
        Dependencies = modIdentity.Dependencies;
    }

    /// <inheritdoc />
    public bool Equals(IModIdentity? other)
    {
        return ModIdentityEqualityComparer.Default.Equals(this, other);
    }
}