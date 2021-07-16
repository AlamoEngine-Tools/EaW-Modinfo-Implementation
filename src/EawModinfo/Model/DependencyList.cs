using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EawModinfo.Spec;

namespace EawModinfo.Model
{
    /// <inheritdoc cref="IModDependencyList"/>
    public class DependencyList : ReadOnlyCollection<IModReference>, IModDependencyList
    {
        /// <summary>
        /// An empty dependency list singleton instance.
        /// </summary>
        public static readonly IModDependencyList EmptyDependencyList =
            new DependencyList(new List<IModReference>(), DependencyResolveLayout.ResolveRecursive);

        /// <inheritdoc/>
        public DependencyResolveLayout ResolveLayout { get; }

        /// <summary>
        /// Creates a new instance from a given <see cref="IModDependencyList"/>.
        /// </summary>
        public DependencyList(IModDependencyList dependencyList) : base(dependencyList.ToList())
        {
            ResolveLayout = dependencyList.ResolveLayout;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public DependencyList(IList<IModReference> dependencies, DependencyResolveLayout resolveLayout) 
            : base(dependencies)
        {
            ResolveLayout = resolveLayout;
        }
    }
}