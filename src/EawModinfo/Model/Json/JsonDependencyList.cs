using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using EawModinfo.Utilities;

namespace EawModinfo.Model.Json;

[JsonConverter(typeof(JsonDependencyListTypeConverter))]
internal class JsonDependencyList : IModDependencyList
{
    private readonly List<IModReference> _internalList = new();

    public DependencyResolveLayout ResolveLayout { get; internal set; }
    
    public int Count => _internalList.Count;

    public IModReference this[int index] => _internalList[index];

    public JsonDependencyList()
    {
    }

    internal void AddItemInternal(IModReference modReference)
    {
        _internalList.Add(modReference);
    }

    public JsonDependencyList(DependencyList dependencyList)
    {
        ResolveLayout = dependencyList.ResolveLayout;
        _internalList = [..dependencyList];
    }
        
    public IEnumerator<IModReference> GetEnumerator()
    {
        return _internalList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool IEquatable<IModDependencyList>.Equals(IModDependencyList? other)
    {
        return ModDependencyListEqualityComparer.Default.Equals(this, other);
    }

    public string ToJson()
    {
        this.Validate();
        return JsonSerializer.Serialize(this, ParseUtility.SerializerOptions);
    }

    public void ToJson(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        this.Validate();
        JsonSerializer.Serialize(stream, this, ParseUtility.SerializerOptions);
    }
}