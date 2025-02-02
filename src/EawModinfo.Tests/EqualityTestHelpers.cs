using System;
using System.Collections.Generic;
using AET.Modinfo.Model;
using AET.Modinfo.Spec;
using Semver;
using Xunit;

namespace AET.Modinfo.Tests;

public class EqualityTestHelpers
{
    public static readonly SemVersion Version = new(1, 2, 3);
    public static readonly SemVersion OtherVersion = new(9, 8, 7);

    public static readonly DependencyList List = new(
        new List<IModReference> { new ModReference("name", ModType.Default) },
        DependencyResolveLayout.FullResolved);

    public static readonly DependencyList SameishList = new(
        new List<IModReference> { new ModReference("name", ModType.Default) },
        DependencyResolveLayout.FullResolved);

    public static readonly DependencyList DifferentRef = new(
        new List<IModReference> { new ModReference("other", ModType.Default) },
        DependencyResolveLayout.FullResolved);

    public static readonly DependencyList DifferentLayout = new(
        new List<IModReference> { new ModReference("name", ModType.Default) },
        DependencyResolveLayout.ResolveRecursive);

    public static readonly DependencyList AllDifferent = new(
        new List<IModReference> { new ModReference("other", ModType.Default) },
        DependencyResolveLayout.ResolveRecursive);

    public static void AssertWithComparer<T>(bool idEqual, IEqualityComparer<T> comparer, T? x, T? y)
    {
        Assert.Equal(idEqual, comparer.Equals(x, y));
        Assert.Equal(idEqual, comparer.Equals(y, x));

        if (x is not null && y is not null)
        {
            Assert.Equal(idEqual, Equals(comparer.GetHashCode(x), comparer.GetHashCode(y)));
            HashSetTest(idEqual, comparer, x, y);
        }
    }

    public static void AssertDefaultEquals<T>(bool equal, bool sameish, T? x, T? y)
    {
        AssertDefaultEquals(equal, x, y);
        if (x is not null)
        {
            if (x is IEquatable<T> eq) 
                Assert.Equal(sameish, eq.Equals(y));

            
        }

        if (y is not null)
        {
            if (y is IEquatable<T> eq)
                Assert.Equal(sameish, eq.Equals(x));
        }
    }

    public static void AssertDefaultEquals(bool equal, object? x, object? y)
    {
        if (x is not null) 
            Assert.Equal(equal, x.Equals(y));
        if (y is not null) 
            Assert.Equal(equal, y.Equals(x));
        Assert.Equal(equal, Equals(x, y));
        Assert.Equal(equal, Equals(y, x));

        if (x is not null && y is not null)
        {
            if (equal) 
                Assert.Equal(x.GetHashCode(), y.GetHashCode());

            HashSetTest(equal, null, x, y);
        }
    }

    public static void HashSetTest<T>(bool equal, IEqualityComparer<T>? comparer, T x, T y)
    {
        var hashSet = new HashSet<T>(comparer);
        Assert.True(hashSet.Add(x));
        Assert.Equal(!equal, hashSet.Add(y));

        hashSet.Clear();
        Assert.True(hashSet.Add(y));
        Assert.Equal(!equal, hashSet.Add(x));
    }
}