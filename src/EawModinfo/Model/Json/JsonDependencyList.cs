using System.Collections;
using System.Collections.Generic;
using EawModinfo.Spec;

namespace EawModinfo.Model.Json;

internal class JsonDependencyList : IModDependencyList
{
    private readonly List<IModReference> _internalList = new();
    public DependencyResolveLayout ResolveLayout { get; internal set; }
    public int Count => _internalList.Count;

    public IModReference this[int index] => _internalList[index];

    internal void AddItemInternal(IModReference modReference)
    {
        _internalList.Add(modReference);
    }
        
    public IEnumerator<IModReference> GetEnumerator()
    {
        return _internalList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}