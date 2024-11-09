using System;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using Semver;
using Xunit;

namespace EawModinfo.Tests;

public abstract class ModIdentityEqualityTestsBase<T> where T : IModIdentity
{
    protected abstract T CreateT(string name, IModDependencyList deps, SemVersion? version);

    [Fact]
    public void Equal_GetHashCode_Default()
    {
        var comparer = ModIdentityEqualityComparer.Default;

        var data = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.Version);
        var sameish = CreateT("A", EqualityTestHelpers.SameishList, EqualityTestHelpers.Version);
        var diffDep = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.Version);
        var diffVer = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.OtherVersion);
        var diffListVer = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.OtherVersion);
        var insensitiveName = CreateT("a", EqualityTestHelpers.List, EqualityTestHelpers.Version);

        EqualityTestHelpers.AssertWithComparer(true, comparer, default, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, default, data);

        EqualityTestHelpers.AssertWithComparer(true, comparer, data, data);
        EqualityTestHelpers.AssertWithComparer(true,  comparer, data, sameish);

        EqualityTestHelpers.AssertWithComparer(false,  comparer, data, diffDep);
        EqualityTestHelpers.AssertWithComparer(false,  comparer, data, diffVer);
        EqualityTestHelpers.AssertWithComparer(false,  comparer, data, diffListVer);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, insensitiveName);
    }

    [Fact]
    public void Equal_GetHashCode_VersionIndependent()
    {
        var comparer = new ModIdentityEqualityComparer(false, true, StringComparer.Ordinal);

        var data = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.Version);
        var sameish = CreateT("A", EqualityTestHelpers.SameishList, EqualityTestHelpers.Version);
        var diffDep = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.Version);
        var diffVer = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.OtherVersion);
        var diffListVer = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.OtherVersion);
        var insensitiveName = CreateT("a", EqualityTestHelpers.List, EqualityTestHelpers.Version);

        EqualityTestHelpers.AssertWithComparer(true, comparer, default, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, default, data);

        EqualityTestHelpers.AssertWithComparer(true, comparer, data, data);
        EqualityTestHelpers.AssertWithComparer(true, comparer, data, sameish);

        EqualityTestHelpers.AssertWithComparer(false, comparer, data, diffDep);
        EqualityTestHelpers.AssertWithComparer(true, comparer, data, diffVer);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, diffListVer);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, insensitiveName);
    }

    [Fact]
    public void Equal_GetHashCode_DependenciesIndependent()
    {
        var comparer = new ModIdentityEqualityComparer(true, false, StringComparer.Ordinal);

        var data = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.Version);
        var sameish = CreateT("A", EqualityTestHelpers.SameishList, EqualityTestHelpers.Version);
        var diffDep = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.Version);
        var diffVer = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.OtherVersion);
        var diffListVer = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.OtherVersion);
        var insensitiveName = CreateT("a", EqualityTestHelpers.List, EqualityTestHelpers.Version);

        EqualityTestHelpers.AssertWithComparer(true, comparer, default, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, default, data);

        EqualityTestHelpers.AssertWithComparer(true, comparer, data, data);
        EqualityTestHelpers.AssertWithComparer(true, comparer, data, sameish);

        EqualityTestHelpers.AssertWithComparer(true, comparer, data, diffDep);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, diffVer);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, diffListVer);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, insensitiveName);
    }

    [Fact]
    public void Equal_GetHashCode_VersionAndDependenciesIndependent()
    {
        var comparer = new ModIdentityEqualityComparer(false, false, StringComparer.Ordinal);

        var data = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.Version);
        var sameish = CreateT("A", EqualityTestHelpers.SameishList, EqualityTestHelpers.Version);
        var diffDep = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.Version);
        var diffVer = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.OtherVersion);
        var diffListVer = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.OtherVersion);
        var insensitiveName = CreateT("a", EqualityTestHelpers.List, EqualityTestHelpers.Version);

        EqualityTestHelpers.AssertWithComparer(true, comparer, default, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, default, data);

        EqualityTestHelpers.AssertWithComparer(true, comparer, data, data);
        EqualityTestHelpers.AssertWithComparer(true, comparer, data, sameish);

        EqualityTestHelpers.AssertWithComparer(true, comparer, data, diffDep);
        EqualityTestHelpers.AssertWithComparer(true, comparer, data, diffVer);
        EqualityTestHelpers.AssertWithComparer(true, comparer, data, diffListVer);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, insensitiveName);
    }

    [Fact]
    public void Equal_GetHashCode_IdNameCaseInsensitive()
    {
        var comparer = new ModIdentityEqualityComparer(true, true, StringComparer.OrdinalIgnoreCase);

        var data = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.Version);
        var sameish = CreateT("A", EqualityTestHelpers.SameishList, EqualityTestHelpers.Version);
        var diffDep = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.Version);
        var diffVer = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.OtherVersion);
        var diffListVer = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.OtherVersion);
        var insensitiveName = CreateT("a", EqualityTestHelpers.List, EqualityTestHelpers.Version);

        EqualityTestHelpers.AssertWithComparer(true, comparer, default, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, default);
        EqualityTestHelpers.AssertWithComparer(false, comparer, default, data);

        EqualityTestHelpers.AssertWithComparer(true, comparer, data, data);
        EqualityTestHelpers.AssertWithComparer(true, comparer, data, sameish);

        EqualityTestHelpers.AssertWithComparer(false, comparer, data, diffDep);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, diffVer);
        EqualityTestHelpers.AssertWithComparer(false, comparer, data, diffListVer);
        EqualityTestHelpers.AssertWithComparer(true, comparer, data, insensitiveName);
    }
}