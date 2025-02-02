using AET.Modinfo.Model;
using AET.Modinfo.Spec;
using AET.Modinfo.Spec.Equality;
using Semver;
using Xunit;

namespace AET.Modinfo.Tests;

public class ModInfoEqualityTest : ModIdentityEqualityTestsBase<ModinfoData>
{
    protected override ModinfoData CreateT(string name, IModDependencyList deps, SemVersion? version)
    {
        return new ModinfoData(name)
        {
            Dependencies = deps,
            Version = version
        };
    }

    [Fact]
    public void Equal_GetHashCode_Instance()
    {
        var data = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.Version);
        var sameish = CreateT("A", EqualityTestHelpers.SameishList, EqualityTestHelpers.Version);
        var diffDep = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.Version);
        var diffVer = CreateT("A", EqualityTestHelpers.List, EqualityTestHelpers.OtherVersion);
        var diffListVer = CreateT("A", EqualityTestHelpers.AllDifferent, EqualityTestHelpers.OtherVersion);
        var insensitiveName = CreateT("a", EqualityTestHelpers.List, EqualityTestHelpers.Version);

        EqualityTestHelpers.AssertDefaultEquals(true , true, data, data);
        EqualityTestHelpers.AssertDefaultEquals<IModIdentity>(true , true, data, data);

        // ModInfoData does not implement IEquatable<ModInfoData> or overrides Equals(object),
        // thus it does not make sense to  AssertDefaultEquals<IModIdentity>() on non-equal objects
        EqualityTestHelpers.AssertDefaultEquals(false, true, data, sameish); 
        EqualityTestHelpers.HashSetTest(true, ModIdentityEqualityComparer.Default, data, sameish);

        EqualityTestHelpers.AssertDefaultEquals(false, false, data, diffDep);
        EqualityTestHelpers.HashSetTest(false, ModIdentityEqualityComparer.Default, data, diffDep);

        EqualityTestHelpers.AssertDefaultEquals(false, false, data, diffVer);
        EqualityTestHelpers.HashSetTest(false, ModIdentityEqualityComparer.Default, data, diffVer);

        EqualityTestHelpers.AssertDefaultEquals(false, false, data, diffListVer);
        EqualityTestHelpers.HashSetTest(false, ModIdentityEqualityComparer.Default, data, diffListVer);

        EqualityTestHelpers.AssertDefaultEquals(false, true, data, insensitiveName);
        EqualityTestHelpers.HashSetTest(true, ModIdentityEqualityComparer.Default, data, insensitiveName);
    }
}