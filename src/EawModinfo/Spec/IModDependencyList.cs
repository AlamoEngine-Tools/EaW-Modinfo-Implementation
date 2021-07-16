using System.Collections.Generic;

namespace EawModinfo.Spec
{
    /// <summary>
    /// Readonly list which contains <see cref="IModReference"/> of dependencies of a target mod.
    /// </summary>
    public interface IModDependencyList : IReadOnlyList<IModReference>
    {
        /// <summary>
        /// Describes how this dependency list shall get interpreted and processed.
        /// </summary>
        DependencyResolveLayout ResolveLayout { get; }
    }
}
