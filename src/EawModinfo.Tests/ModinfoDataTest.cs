using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using Semver;
using Xunit;
using Xunit.Abstractions;

namespace EawModinfo.Tests;

public class ModinfoDataTest(ITestOutputHelper output)
{
    private const string InvalidJsonData = @"{
  ""version"": ""1.0.0""
}";

    [Fact]
    public void Test_Parse_AllowTrailingCommaAndComments()
    {
        var data = @"
{
    // This is a comment
    ""name"":""My Mod Name"",
}";

        // Parsing supports it, but in this test, we ensure evaluation also supports it.
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data, null, new JsonDocumentOptions{AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip}), EvaluationType.ModInfo);
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
    }

    [Fact]
    public void Test_Parse_Minimal()
    {
        var data = @"
{
    ""name"":""My Mod Name""
}";

        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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

        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Equal(SemVersion.ParsedFrom(1, 1, 1, "BETA"), modinfo.Version);
    }

    [Fact]
    public void Test_Parse_SummaryIcon()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""summary"":""Some Text"",
    ""icon"":""icon.ico""
}";

        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Equal("Some Text", modinfo.Summary);
        Assert.Equal("icon.ico", modinfo.Icon);
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
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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

        Assert.Throws<ModinfoParseException>(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo));
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
        Assert.Throws<ModinfoParseException>(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo));
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
        Assert.Throws<ModinfoParseException>(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo));
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
        Assert.Throws<ModinfoParseException>(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo));
        Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
    }

    [Fact]
    public void Test_Parse_WithIncompleteModRef_ThrowsModinfoParseException2()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [ ""FullResolved"", {""identifier"":""123""} ]
}";
        Assert.Throws<ModinfoParseException>(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo));
        Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
    }


    [Fact]
    public void Test_Parse_Custom_Empty()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""custom"": {}
}";
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
        ""test-key2"":""123""
    }
}";
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
            ""FOC"", ""EAW""
        ]
    }
}";
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
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
            null!
        ];
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
    public void Test_Parse_FailingParseTest(string? data)
    {
        if (string.IsNullOrEmpty(data))
            Assert.Throws<ModinfoParseException>(() =>
                ModInfoJsonSchema.Evaluate(null, EvaluationType.ModInfo));
        else
            Assert.Throws<ModinfoParseException>(() =>
                ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo));

        Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data!));
    }

    [Fact]
    public void Test_ToJson()
    {
        var modinfo = new ModinfoData("Test") { Version = SemVersion.ParsedFrom(1, 1, 1, "BETA") };
        var data = modinfo.ToJson();
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
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.Contains(@"""FullResolved"",", data);
        Assert.Contains(@"""identifier"": ""123""", data);
        Assert.Contains(@"""modtype"": 0", data);
    }

    [Fact]
    public void Test_ToJson_EmptyDependencyList()
    {
        var modinfo = new ModinfoData("Test")
        {
            Dependencies = DependencyList.EmptyDependencyList
        };
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.DoesNotContain(@"""dependencies""", data);
    }

    [Fact]
    public void Test_ToJson_ModRefRange()
    {
        var modinfo = new ModinfoData("Test")
        {
            Dependencies = new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default, SemVersionRange.Parse("1.*")) }, DependencyResolveLayout.ResolveRecursive)
        };
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.DoesNotContain(@"""ResolveRecursive"",", data);
        Assert.Contains(@"""version-range"": ""1.*""", data);
    }

    [Fact]
    public void Test_ToJson_SummaryIcon()
    {
        var modinfo = new ModinfoData("Test")
        {
            Summary = "Summary",
            Icon = "icon.ico"
        };
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.Contains(@"""summary"": ""Summary""", data);
        Assert.Contains(@"""icon"": ""icon.ico""", data);
    }


    [Fact]
    public void Test_ToJson_Full()
    {
        var modinfo = new ModinfoData("Test")
        {
            Summary = "Summary",
            Icon = "icon.ico",
            Languages = new List<ILanguageInfo>
            {
                new LanguageInfo("en", 0),
                new LanguageInfo("de", LanguageSupportLevel.Text),
                new LanguageInfo("fr", LanguageSupportLevel.FullLocalized),
            },
            Custom = new Dictionary<string, object>
            {
                { "key", "value" }
            },
            SteamData = new SteamData("123", "folder", SteamWorkshopVisibility.Public, "Test", ["FOC"])
        };
        var data = modinfo.ToJson();
        output.WriteLine(data);

        var expected = @"{
  ""name"": ""Test"",
  ""summary"": ""Summary"",
  ""icon"": ""icon.ico"",
  ""languages"": [
    {
      ""code"": ""en""
    },
    {
      ""code"": ""de"",
      ""support"": 1
    },
    {
      ""code"": ""fr"",
      ""support"": 7
    }
  ],
  ""custom"": {
    ""key"": ""value""
  },
  ""steamdata"": {
    ""publishedfileid"": ""123"",
    ""contentfolder"": ""folder"",
    ""previewfile"": """",
    ""visibility"": 0,
    ""title"": ""Test"",
    ""description"": """",
    ""metadata"": """",
    ""tags"": [
      ""FOC""
    ]
  }
}";

        Assert.Equal(expected, data);
    }

    [Fact]
    public void Test_ToJson_CustomDefaultLanguageOnly()
    {
        var modinfo = new ModinfoData("Test")
        {
            Languages = new List<ILanguageInfo>
            {
                new LanguageInfo("en", 0),
            }
        };
        var data = modinfo.ToJson();
        Assert.DoesNotContain(@"""languages""", data);
        output.WriteLine(data);
    }

    [Fact]
    public void Test_ToJson_DefaultOnlyLanguage()
    {
        var modinfo = new ModinfoData("Test")
        {
            Languages = new List<ILanguageInfo>
            {
                LanguageInfo.Default
            }
        };
        var data = modinfo.ToJson();
        Assert.DoesNotContain(@"""languages""", data);
        output.WriteLine(data);
    }

    [Fact]
    public void Test_Parse_TolerantVersion()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""version"": ""1.0""
}";
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModInfo);
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal("My Mod Name", modinfo.Name);
        Assert.Equal(new SemVersion(1, 0, 0), modinfo.Version);
    }

    [Fact]
    public void Ctor_Copy()
    {
        var modinfo = new ModinfoData("Test")
        {
            Summary = "Summary",
            Icon = "icon.ico",
            Languages = new List<ILanguageInfo>
            {
                new LanguageInfo("en", 0),
                new LanguageInfo("de", LanguageSupportLevel.Text),
                new LanguageInfo("fr", LanguageSupportLevel.FullLocalized),
            },
            Custom = new Dictionary<string, object>
            {
                { "key", "value" }
            },
            SteamData = new SteamData("123", "folder", SteamWorkshopVisibility.Public, "Test", ["FOC"])
        };

        Assert.Equal("Test", modinfo.Name);
        Assert.Equal("Summary", modinfo.Summary);
        Assert.Equal("icon.ico", modinfo.Icon);
        Assert.Equivalent(
            new List<LanguageInfo>{new("en", 0), new("de", LanguageSupportLevel.Text), new("fr", LanguageSupportLevel.FullLocalized)}, 
            modinfo.Languages);
        Assert.Equivalent(new Dictionary<string, object>
        {
            { "key", "value" }
        }, modinfo.Custom);
        Assert.Equal("123", modinfo.SteamData.Id);
        Assert.Equal("folder", modinfo.SteamData.ContentFolder);
        Assert.Equal(SteamWorkshopVisibility.Public, modinfo.SteamData.Visibility);
        Assert.Equal("Test", modinfo.SteamData.Title);
        Assert.Equivalent(new List<string>{"FOC"}, modinfo.SteamData.Tags);

        var other = new ModinfoData(modinfo);

        Assert.Equal(modinfo.Name, other.Name);
        Assert.Equal(modinfo.Summary, other.Summary);
        Assert.Equal(modinfo.Icon, other.Icon);
        Assert.Equal(modinfo.Languages, other.Languages);
        Assert.Equivalent(modinfo.Custom, other.Custom);
        Assert.Equal(modinfo.SteamData.Id, other.SteamData.Id);
        Assert.Equal(modinfo.SteamData.ContentFolder, other.SteamData.ContentFolder);
        Assert.Equal(modinfo.SteamData.Visibility, other.SteamData.Visibility);
        Assert.Equal(modinfo.SteamData.Title, other.SteamData.Title);
        Assert.Equivalent(modinfo.SteamData.Tags, other.SteamData.Tags);
    }

    [Fact]
    public void ToJsonFromJson()
    {
        var modinfo = new ModinfoData("Test")
        {
            Summary = "Summary",
            Icon = "icon.ico",
            Languages = new List<ILanguageInfo>
            {
                new LanguageInfo("en", 0),
                new LanguageInfo("de", LanguageSupportLevel.Text),
                new LanguageInfo("fr", LanguageSupportLevel.FullLocalized),
            },
            Custom = new Dictionary<string, object>
            {
                { "key", "value" }
            },
            SteamData = new SteamData("123", "folder", SteamWorkshopVisibility.Public, "Test", ["FOC"])
        };

        var other = new ModinfoData(modinfo);

        Assert.Equal(modinfo.Name, other.Name);
        Assert.Equal(modinfo.Summary, other.Summary);
        Assert.Equal(modinfo.Icon, other.Icon);
        Assert.Equal(modinfo.Languages, other.Languages);
        Assert.Equivalent(modinfo.Custom, other.Custom);
        Assert.Equal(modinfo.SteamData.Id, other.SteamData.Id);
        Assert.Equal(modinfo.SteamData.ContentFolder, other.SteamData.ContentFolder);
        Assert.Equal(modinfo.SteamData.Visibility, other.SteamData.Visibility);
        Assert.Equal(modinfo.SteamData.Title, other.SteamData.Title);
        Assert.Equivalent(modinfo.SteamData.Tags, other.SteamData.Tags);
    }

    [Fact]
    public void Ctor_FromIdentity()
    {
        var id = new ModIdentity("Name")
        {
            Dependencies = new DependencyList(new List<IModReference>
            {
                new ModReference("1", ModType.Default),
                new ModReference("2", ModType.Workshops, SemVersionRange.Parse("*")),
            }, DependencyResolveLayout.FullResolved),
            Version = new SemVersion(1,2,3, ["beta1"])
        };

        var modinfo = new ModinfoData(id);

        Assert.Equal("Name", modinfo.Name);
        Assert.Equal(SemVersion.Parse("1.2.3-beta1"), modinfo.Version);
        Assert.Equal(DependencyResolveLayout.FullResolved, modinfo.Dependencies.ResolveLayout);
        Assert.Equal([new ModReference("1", ModType.Default), new ModReference("2", ModType.Workshops, SemVersionRange.Parse("*"))], modinfo.Dependencies.ToList());
        Assert.Equivalent(new Dictionary<string, string>(), modinfo.Custom);
        Assert.Equal([LanguageInfo.Default], modinfo.Languages);
        Assert.Null(modinfo.Icon);
        Assert.Null(modinfo.SteamData);
        Assert.Null(modinfo.Summary);
    }

    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ModinfoData((string)null!));
        Assert.Throws<ArgumentException>(() => new ModinfoData(string.Empty));
        Assert.Throws<ArgumentNullException>(() => new ModinfoData((IModIdentity)null!));
        Assert.Throws<ArgumentNullException>(() => new ModinfoData((IModinfo)null!));
    }
}