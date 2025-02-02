using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AET.Modinfo.Model.Json;
using AET.Modinfo.Spec;
using AET.Modinfo.Spec.Equality;
using AET.Modinfo.Utilities;

namespace AET.Modinfo.Model;

/// <inheritdoc cref="IModDependencyList"/>
public class DependencyList : ReadOnlyCollection<IModReference>, IModDependencyList, IEquatable<DependencyList>
{
    /// <summary>
    /// Returns a singleton instance of an empty dependency list.
    /// </summary>
    public static readonly IModDependencyList EmptyDependencyList =
        new DependencyList(new List<IModReference>(), DependencyResolveLayout.FullResolved);

    /// <inheritdoc/>
    public DependencyResolveLayout ResolveLayout { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyList"/> class from an <see cref="IModDependencyList"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="dependencyList"/> is <see langword="null"/>.</exception>
    public DependencyList(IModDependencyList dependencyList) 
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        : base(dependencyList?.ToList() ?? throw new ArgumentNullException(nameof(dependencyList)))
    {
        ResolveLayout = dependencyList.ResolveLayout;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyList"/> class from list of mod references and a resolve layout.
    /// </summary>
    /// <param name="dependencies">List of mod references.</param>
    /// <param name="resolveLayout">The resolve layout of this dependency list.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dependencies"/> is <see langword="null"/>.</exception>
    public DependencyList(IList<IModReference> dependencies, DependencyResolveLayout resolveLayout) 
        : base(dependencies)
    {
        ResolveLayout = resolveLayout;
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="DependencyList"/>.
    /// </summary>
    /// <param name="data">The raw json data.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">When parsing failed, e.g. due to missing required properties.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    public static DependencyList Parse(string data)
    {
        if (data == null) 
            throw new ArgumentNullException(nameof(data));
        return new DependencyList(ParseUtility.Parse<JsonDependencyList>(data));
    }

    /// <summary>
    /// Parses and deserializes a json data into a <see cref="DependencyList"/>.
    /// </summary>
    /// <param name="data">The json data stream.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ModinfoParseException">When parsing failed, e.g. due to missing required properties.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    public static DependencyList Parse(Stream data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        return new DependencyList(ParseUtility.Parse<JsonDependencyList>(data));
    }

    /// <inheritdoc />
    public bool Equals(DependencyList? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return ((IModDependencyList)this).Equals(other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not DependencyList other)
            return false;
        return Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return ModDependencyListEqualityComparer.Default.GetHashCode(this);
    }

    /// <inheritdoc />
    public bool Equals(IModDependencyList? other)
    {
        return ModDependencyListEqualityComparer.Default.Equals(this, other);
    }

    /// <inheritdoc />
    public string ToJson()
    {
        return new JsonDependencyList(this).ToJson();
    }

    /// <inheritdoc />
    public void ToJson(Stream stream)
    {
        new JsonDependencyList(this).ToJson(stream);
    }
}