using System.Collections.Generic;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using SemanticVersioning;
using Validation;

namespace EawModinfo.Model
{
    /// <inheritdoc cref="IModIdentity"/> 
    public sealed record ModIdentity : IModIdentity
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Version? Version { get; init; }

        /// <inheritdoc />
        public IReadOnlyList<IModReference> Dependencies { get; init; } = new List<IModReference>();

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
        public bool Equals(IModIdentity other)
        {
            return ModIdentityEqualityComparer.Default.Equals(this, other);
        }
    }
}
