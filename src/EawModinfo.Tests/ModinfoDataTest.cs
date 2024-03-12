using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EawModinfo.Model;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using Semver;
using Xunit;
using Xunit.Abstractions;

namespace EawModinfo.Tests;

public class ModinfoDataTest(ITestOutputHelper output)
{
    private const string InvalidJsonData = @"{
  ""version"": ""1.0.0"",
}";

    [Fact]
    public void Test_Parse_Minimal()
    {
        var data = @"
{
    ""name"":""My Mod Name""
}";

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Null(modinfo.Version);
        Assert.Null(modinfo.SteamData);
    }

    [Fact]
    public void Test_Parse_Version()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""version"":""1.1.1-BETA""
}";

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Equal(SemVersion.ParsedFrom(1, 1, 1, "BETA"), modinfo.Version);
    }

    [Fact]
    public void Test_Parse_WithOneLanguage()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""languages"":[
        {
            ""code"":""en""
        }
    ]
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Single(modinfo.Languages);
    }

    [Fact]
    public void Test_Parse_WithManyLanguages()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""languages"":[
        {
            ""code"":""en""
        },
        {
            ""code"":""de""
        }
    ]
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Equal(2, modinfo.Languages.Count());
    }

    [Fact]
    public void Test_Parse_WithDuplicateLanguage()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""languages"":[
        {
            ""code"":""en""
        },
        {
            ""code"":""en""
        }
    ]
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Single(modinfo.Languages);
    }

    [Fact]
    public void Test_Parse_WithDuplicateLanguage2()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""languages"":[
        {
            ""code"":""en""
        },
        {
            ""code"":""en"",
            ""support"":3
        }
    ]
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Single(modinfo.Languages);
    }

    [Fact]
    public void Test_Parse_WithoutLanguage()
    {
        var data = @"
{
    ""name"":""My Mod Name""
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Single(modinfo.Languages);
        Assert.Equal("en", modinfo.Languages.ElementAt(0).Code);
        Assert.Equal(LanguageSupportLevel.FullLocalized, modinfo.Languages.ElementAt(0).Support);
    }

    [Fact]
    public void Test_Parse_WithoutDeps()
    {
        var data = @"
{
    ""name"":""My Mod Name""
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Empty(modinfo.Dependencies);
        Assert.Equal(DependencyResolveLayout.ResolveRecursive, modinfo.Dependencies.ResolveLayout);
    }


    [Fact]
    public void Test_Parse_WithManyDeps()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [
        {
            ""identifier"":""12313"",
            ""modtype"":1
        },
        {
            ""identifier"":""654987"",
            ""modtype"":1
        }
    ]
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Equal(2, modinfo.Dependencies.Count);
        Assert.Equal("12313", modinfo.Dependencies[0].Identifier);
        Assert.Equal("654987", modinfo.Dependencies[1].Identifier);
        Assert.Equal(DependencyResolveLayout.ResolveRecursive, modinfo.Dependencies.ResolveLayout);
    }

    [Fact]
    public void Test_Parse_WithDepsLayout()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [
        ""FullResolved"",
        {
            ""identifier"":""12313"",
            ""modtype"":1
        },
        {
            ""identifier"":""654987"",
            ""modtype"":1
        }
    ]
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Equal(2, modinfo.Dependencies.Count);
        Assert.Equal("12313", modinfo.Dependencies[0].Identifier);
        Assert.Equal("654987", modinfo.Dependencies[1].Identifier);
        Assert.Equal(DependencyResolveLayout.FullResolved, modinfo.Dependencies.ResolveLayout);
    }

    [Fact]
    public void Test_Parse_WithDepsLayout_ThrowsModinfoParseException()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [
        {
            ""identifier"":""12313"",
            ""modtype"":1
        },
        ""FullResolved"",
        {
            ""identifier"":""654987"",
            ""modtype"":1
        }
    ]
}";
        Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
    }

    [Fact]
    public void Test_Parse_WithUnknownDepsLayout_ThrowsModinfoParseException()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [
        ""BlaBlub"",
        {
            ""identifier"":""12313"",
            ""modtype"":1
        },
        {
            ""identifier"":""654987"",
            ""modtype"":1
        }
    ]
}";
        Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
    }

    [Fact]
    public void Test_Parse_WithEmptyDeps_ThrowsModinfoParseException()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [ ]
}";
        Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
    }

    [Fact]
    public void Test_Parse_WithEmptyDeps_ThrowsModinfoParseException2()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [ ""FullResolved"" ]
}";
        Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
    }


    [Fact]
    public void Test_Parse_Custom_Empty()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""custom"": {
        
    }
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Empty(modinfo.Custom);
    }

    [Fact]
    public void Test_Parse_Custom_TwoPairs()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""custom"": {
        ""test-key"":{},
        ""test-key2"":""123"",
    }
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Equal(2, modinfo.Custom.Count);
        Assert.Equal("123", ((JsonElement)modinfo.Custom["test-key2"]).GetString());

        Assert.IsType<JsonElement>(modinfo.Custom["test-key"]);
    }


    [Fact]
    public void Test_Parse_SteamData()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""steamdata"": {
        ""publishedfileid"":""123"",
        ""contentfolder"":""path"",
        ""metadata"": ""test"",
        ""visibility"":0,
        ""title"":""test"",
        ""tags"":[
            ""foc"", ""eaw""
        ]
    }
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.NotNull(modinfo.SteamData);
        Assert.Equal("123", modinfo.SteamData!.Id);
        Assert.Equal("test", modinfo.SteamData.Title);
        Assert.Equal("path", modinfo.SteamData.ContentFolder);
        Assert.Equal(SteamWorkshopVisibility.Public, modinfo.SteamData.Visibility);
        Assert.Equal("test", modinfo.SteamData.Metadata);
        Assert.Null(modinfo.SteamData.Description);
        Assert.Equal(2, modinfo.SteamData.Tags.Count());

    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        yield return
        [
            string.Empty
        ];
        yield return
        [
            @"
{
}"
        ];
        yield return
        [
            InvalidJsonData
        ];
    }

    [Theory]
    [MemberData(nameof(GetInvalidData))]
    public void Test_Parse_FailingParseTest(string data)
    {
        Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
    }

    [Fact]
    public void Test_ToJson()
    {
        var modinfo = new ModinfoData("Test") { Version = SemVersion.ParsedFrom(1, 1, 1, "BETA") };
        var data = modinfo.ToJson(false);
        output.WriteLine(data);
        Assert.Contains(@"""version"": ""1.1.1-BETA""", data);
        Assert.DoesNotContain(@"""custom"":", data);
    }

    [Fact]
    public void Test_ToJson_DependencyList()
    {
        var modinfo = new ModinfoData("Test")
        {
            Dependencies = new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default) }, DependencyResolveLayout.FullResolved)
        };
        var data = modinfo.ToJson(false);
        output.WriteLine(data);
        Assert.Contains(@"""FullResolved"",", data);
    }

    [Fact]
    public void Test_ToJson_ModRefRange()
    {
        var modinfo = new ModinfoData("Test")
        {
            Dependencies = new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default, SemVersionRange.Parse("1.*")) }, DependencyResolveLayout.ResolveRecursive)
        };
        var data = modinfo.ToJson(false);
        output.WriteLine(data);
        Assert.DoesNotContain(@"""ResolveRecursive"",", data);
        Assert.Contains(@"""version-range"": ""1.*""", data);
    }

    [Fact]
    public void Test_Parse_TolerantVersion()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""version"": ""1.0""
}";
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Equal(new SemVersion(1, 0, 0), modinfo.Version);
    }
}