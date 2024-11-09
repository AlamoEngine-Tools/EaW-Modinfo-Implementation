using System;
using System.Collections.Generic;
using System.IO;
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
    public void MergeInto_VariantDefinesNameOnly()
    {
        var mainData = new ModinfoData("Mod")
        {
            Icon = "icon.ico",
            Summary = "summary",
            Languages = [LanguageInfo.Default, new LanguageInfo("de", LanguageSupportLevel.FullLocalized)],
            SteamData = new JsonSteamData { Id = "123", Tags = ["FOC"], ContentFolder = "bla", Title = "Title" },
            Custom = new Dictionary<string, object> { { "testKey2", "value" } },
            Dependencies = new DependencyList([
                new ModReference {Identifier = "bla"}, new ModReference {Identifier = "blub"}
            ], DependencyResolveLayout.FullResolved),

            Version = SemVersion.Parse("1.2.2", SemVersionStyles.Any)
        };

        var variantData = new ModinfoData("Variant of Mod");

        var newData = variantData.MergeInto(mainData);

        Assert.Equal(variantData.Name, newData.Name);
        Assert.Equal(mainData.Icon, newData.Icon);
        Assert.Equal(mainData.Summary, newData.Summary);
        Assert.Equal(2, newData.Languages.Count); // As stated by the specification in III.3.2, languages was not explicitly set.
        Assert.Equal(LanguageInfo.Default, newData.Languages.First());
        Assert.Equal(2, newData.Dependencies.Count);
        Assert.Equal("bla", newData.Dependencies[0].Identifier);
        Assert.NotNull(newData.SteamData);
        Assert.Equal(mainData.SteamData!.Id, newData.SteamData?.Id);
        Assert.Equal(mainData.SteamData!.Title, newData.SteamData?.Title);
        Assert.Single(newData.Custom);
        Assert.Equal(new SemVersion(1, 2, 2), newData.Version);
        Assert.Equal(mainData.Version, newData.Version);


        var invalid = new InvalidModinfo();
        Assert.Throws<ModinfoException>(() => invalid.MergeInto(newData));
        Assert.Throws<ModinfoException>(() => newData.MergeInto(invalid));
    }

    [Fact]
    public void MergeInto_VariantDefinesAll()
    {
        var mainData = new ModinfoData("Mod")
        {
            Icon = "icon.ico",
            Summary = "summary",
            Languages = [LanguageInfo.Default],
            SteamData = new JsonSteamData { Id = "123", Tags = ["FOC"], ContentFolder = "bla", Title = "Title" },
            Dependencies = new DependencyList([
                new ModReference { Identifier = "foo" }, new ModReference { Identifier = "bar" }
            ], DependencyResolveLayout.FullResolved),

            Version = SemVersion.Parse("1.2.2", SemVersionStyles.Any)
        };

        var variantData = new ModinfoData("Variant of Mod")
        {
            Icon = "other.ico",
            Summary = "other",
            Languages = [LanguageInfo.Default, new LanguageInfo("de", LanguageSupportLevel.FullLocalized)],
            SteamData = new JsonSteamData { Id = "890", Tags = ["EAW"], ContentFolder = "foo", Title = "Steam Title" },
            Dependencies = new DependencyList([
                new ModReference { Identifier = "bar" }, new ModReference { Identifier = "foo" }
            ], DependencyResolveLayout.FullResolved),

            Version = SemVersion.Parse("9.9.9", SemVersionStyles.Any)
        };

        var newData = variantData.MergeInto(mainData);

        Assert.Equal(variantData.Name, newData.Name);
        Assert.Equal(variantData.Icon, newData.Icon);
        Assert.Equal(variantData.Summary, newData.Summary);
        Assert.Equal(2, newData.Languages.Count());
        Assert.Equal(2, newData.Dependencies.Count);
        Assert.Equal("bar", newData.Dependencies[0].Identifier);
        Assert.NotNull(newData.SteamData);
        Assert.Equal(variantData.SteamData!.Id, newData.SteamData?.Id);
        Assert.Equal(variantData.SteamData!.Title, newData.SteamData?.Title);
        Assert.Equal(new SemVersion(9, 9, 9), newData.Version);
        Assert.Equal(variantData.Version, newData.Version);


        var invalid = new InvalidModinfo();
        Assert.Throws<ModinfoException>(() => invalid.MergeInto(newData));
        Assert.Throws<ModinfoException>(() => newData.MergeInto(invalid));
    }

    [Fact]
    public void MergeInto_CustomMerge()
    {
        var mainData = new ModinfoData("Mod")
        {
            Custom = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", "value2" },
            },
        };

        var variantData = new ModinfoData("Variant of Mod")
        {
            Custom = new Dictionary<string, object>
            {
                { "key3", "value3" },
                { "key2", 2 },
            },
        };

        var newData = variantData.MergeInto(mainData);

        Assert.Equal(3, newData.Custom.Count);

        Assert.Equal("value1", newData.Custom["key1"]);
        Assert.Equal(2, newData.Custom["key2"]); // Value should be 2 as specified in III.3.2
        Assert.Equal("value3", newData.Custom["key3"]);
    }

    [Fact]
    public void MergeInto_BaseIsNull()
    { 
        var variantData = new ModinfoData("Variant of Mod")
        {
            Icon = "other.ico",
            Summary = "other",
            Languages = [LanguageInfo.Default, new LanguageInfo("de", LanguageSupportLevel.FullLocalized)],
            SteamData = new JsonSteamData { Id = "890", Tags = ["EAW"], ContentFolder = "foo", Title = "Steam Title" },
            Dependencies = new DependencyList([
                new ModReference { Identifier = "bar" }, new ModReference { Identifier = "foo" }
            ], DependencyResolveLayout.FullResolved),

            Version = SemVersion.Parse("9.9.9", SemVersionStyles.Any)
        };

        var newData = variantData.MergeInto(null);
        Assert.Same(variantData, newData);
    }


    [Fact]
    public void MergeInto_Throws()
    {
        var mainData = new ModinfoData("Mod");
        Assert.Throws<ArgumentNullException>(() => ModinfoDataUtilities.MergeInto(null!, mainData));
    }

    internal class InvalidModinfo : IModinfo
    {
        public string Name => null!;
        public SemVersion? Version => null;
        public IModDependencyList Dependencies => null!;
        public string? Summary => null;
        public string? Icon => null;
        public IDictionary<string, object> Custom => null!;
        public ISteamData? SteamData => null;
        public IReadOnlyCollection<ILanguageInfo> Languages => null!;

        public string ToJson() => string.Empty;
        public void ToJson(Stream stream)
        {
        }

        public bool Equals(IModIdentity? other) => false;
    }
}