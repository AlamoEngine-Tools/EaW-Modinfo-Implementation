using EawModinfo.Model;
using EawModinfo.Spec;
using Semver;
using Xunit;

namespace EawModinfo.Tests;

public class ModIdentityEqualityTest : ModIdentityEqualityTestsBase<ModIdentity>
{
    protected override ModIdentity CreateT(string name, IModDependencyList deps, SemVersion? version)
    {
        return new ModIdentity(name)
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

        EqualityTestHelpers.AssertDefaultEquals(false, false, data, null);
        EqualityTestHelpers.AssertDefaultEquals<IModIdentity>(false, false, data, null);

        EqualityTestHelpers.AssertDefaultEquals(true, true, data, data);
        EqualityTestHelpers.AssertDefaultEquals<IModIdentity>(true, true, data, data);

        // ModIdentity implements IEquatable<ModIdentity> and overrides Equals(object)
        EqualityTestHelpers.AssertDefaultEquals(true, true, data, sameish);
        EqualityTestHelpers.AssertDefaultEquals<IModIdentity>(true, true, data, sameish);

        EqualityTestHelpers.AssertDefaultEquals(false, false, data, diffDep);
        EqualityTestHelpers.AssertDefaultEquals<IModIdentity>(false, false, data, diffDep);
        EqualityTestHelpers.AssertDefaultEquals(false, false, data, diffVer);
        EqualityTestHelpers.AssertDefaultEquals<IModIdentity>(false, false, data, diffVer);
        EqualityTestHelpers.AssertDefaultEquals(false, false, data, diffListVer);
        EqualityTestHelpers.AssertDefaultEquals<IModIdentity>(false, false, data, diffListVer);

        EqualityTestHelpers.AssertDefaultEquals(true, true, data, insensitiveName);
        EqualityTestHelpers.AssertDefaultEquals<IModIdentity>(true, true, data, insensitiveName);
    }
}