using System.Collections.Generic;
using System.Linq;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Semver;
using Xunit;

namespace EawModinfo.Tests;

public class MergeTests
{
    [Fact]
    public void Merge()
    {
        var mainData = new ModinfoData("Mod")
        {
            Icon = "icon.ico",
            Languages = new[] {new LanguageInfo { Code = "en"}},
            Dependencies = new DependencyList(new IModReference[] { new ModReference { Identifier = "bla" } }, DependencyResolveLayout.FullResolved),
            Custom = new Dictionary<string, object> { { "testKey1", "value" } }
        };

        var variantData = new ModinfoData("Variant of Mod")
        {
            Languages = new[] {new LanguageInfo { Code = "en"}, new LanguageInfo { Code = "de"}},
            SteamData = new JsonSteamData {Id = "123", Tags = new[] {"FOC"}, ContentFolder = "bla", Title = "Title"},
            Custom = new Dictionary<string, object> { { "testKey2", "value" } },
            Dependencies = new DependencyList(new IModReference[]
            {
                new ModReference {Identifier = "bla"}, new ModReference {Identifier = "blub"}
            }, DependencyResolveLayout.FullResolved),

            Version = SemVersion.Parse("1.2.2", SemVersionStyles.Any)
        };

        var newData = variantData.MergeInto(mainData);
            
        Assert.Equal(variantData.Name, newData.Name);
        Assert.Equal(mainData.Icon, newData.Icon);
        Assert.Equal(2, newData.Languages.Count());
        Assert.Equal(2, newData.Dependencies.Count);
        Assert.Equal("bla", newData.Dependencies[0].Identifier);
        Assert.NotSame(mainData.Dependencies[0], newData.Dependencies[0]);
        Assert.NotNull(newData.SteamData);
        Assert.Equal(variantData.SteamData!.Id, newData.SteamData?.Id);
        Assert.Equal(variantData.SteamData!.Title, newData.SteamData?.Title);
        Assert.Equal(2, newData.Custom.Count);
        Assert.Equal(new SemVersion(1,2,2), newData.Version);
        Assert.Equal(variantData.Version, newData.Version);


        var invalid = new InvalidModinfoMock();
        Assert.Throws<ModinfoException>(() => invalid.MergeInto(newData));
        Assert.Throws<ModinfoException>(() => newData.MergeInto(invalid));
    }

    internal class InvalidModinfoMock : IModinfo
    {
        public bool Equals(IModIdentity? other)
        {
            return false;
        }

        public string Name { get; } = null!;

        public SemVersion? Version { get; } = null!;

        public IModDependencyList Dependencies { get; } = null!;
        public string ToJson(bool validate)
        {
            return string.Empty;
        }

        public string? Summary { get; } = null!;
        public string? Icon { get; } = null!;
        public IDictionary<string, object> Custom { get; } = null!;
        public ISteamData? SteamData { get; } = null!;
        public IEnumerable<ILanguageInfo> Languages { get; } = null!;
    }
}