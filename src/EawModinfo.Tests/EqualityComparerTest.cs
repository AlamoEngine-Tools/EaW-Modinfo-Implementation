using System;
using AET.Modinfo.Spec.Equality;
using Xunit;

namespace AET.Modinfo.Tests;

public class EqualityComparerTest
{
    [Fact]
    public void Ctor_Null_Args_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ModIdentityEqualityComparer(TestUtilities.GetRandomBool(), TestUtilities.GetRandomBool(), null!));
    }

    [Fact]
    public void GetHashCode_NullArg_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ModReferenceEqualityComparer.Default.GetHashCode(null!));

        Assert.Throws<ArgumentNullException>(() => LanguageInfoEqualityComparer.Default.GetHashCode(null!));
        Assert.Throws<ArgumentNullException>(() => LanguageInfoEqualityComparer.WithSupportLevel.GetHashCode(null!));

        Assert.Throws<ArgumentNullException>(() => ModDependencyListEqualityComparer.Default.GetHashCode(null!));

        Assert.Throws<ArgumentNullException>(() => ModIdentityEqualityComparer.Default.GetHashCode(null!));
        Assert.Throws<ArgumentNullException>(() =>
            new ModIdentityEqualityComparer(TestUtilities.GetRandomBool(), TestUtilities.GetRandomBool(),
                StringComparer.Ordinal).GetHashCode(null!));
    }
}