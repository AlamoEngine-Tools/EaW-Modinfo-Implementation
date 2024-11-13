using System;
using EawModinfo.Model;
using EawModinfo.Spec;
using Semver;
using Xunit;

namespace EawModinfo.Tests;

public class ModIdentityTest
{
    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ModIdentity(null!));
        Assert.Throws<ArgumentException>(() => new ModIdentity(""));
        Assert.Throws<ArgumentNullException>(() => new ModIdentity("name") { Dependencies = null! });
    }

    [Fact]
    public void Ctor()
    {
        var id = new ModIdentity("name")
        {
            Version = SemVersion.Parse("1.2.3"),
            Dependencies = new DependencyList([new ModReference("other", ModType.Virtual, SemVersionRange.All)], DependencyResolveLayout.FullResolved)
        };

        Assert.Equal("name", id.Name);
        Assert.Equal(SemVersion.Parse("1.2.3"), id.Version);
        Assert.Equal(new DependencyList([new ModReference("other", ModType.Virtual, SemVersionRange.All)], 
            DependencyResolveLayout.FullResolved), id.Dependencies);
    }

    [Fact]
    public void Ctor_NameOnly()
    {
        var id = new ModIdentity("name");
        Assert.Equal("name", id.Name);
        Assert.Null(id.Version);
        Assert.Equal(DependencyList.EmptyDependencyList, id.Dependencies);
    }
}