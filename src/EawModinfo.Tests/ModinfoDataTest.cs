using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AET.Modinfo.Model;
using AET.Modinfo.Model.Json.Schema;
using AET.Modinfo.Spec;
using AET.Modinfo.Spec.Steam;
using Semver;
using Xunit;
using Xunit.Abstractions;

namespace AET.Modinfo.Tests;

public class ModinfoDataTest(ITestOutputHelper output)
{
    #region Parse

    [Fact]
    public void Parse_AllowTrailingCommaAndComments()
    {
        var data = @"
{
    /*Multi-lin
      comment */
    // This is a comment
    ""name"":""My Mod Name"",
}";

        // Parsing supports it, but in this test, we ensure evaluation also supports it.
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(
            JsonNode.Parse(data, null, new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip }),
            EvaluationType.ModInfo, out _));
        
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
    }

    [Fact]
    public void Parse_Minimal()
    {
        var data = @"
{
    ""name"":""My Mod Name""
}";

        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Null(modinfo.Version);
        Assert.Null(modinfo.SteamData);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Null(modinfo.Version);
        Assert.Null(modinfo.SteamData);
    }

    [Fact]
    public void Parse_Version()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""version"":""1.1.1-BETA""
}";

        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal(SemVersion.ParsedFrom(1, 1, 1, "BETA"), modinfo.Version);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal(SemVersion.ParsedFrom(1, 1, 1, "BETA"), modinfo.Version);
    }

    [Fact]
    public void Parse_SummaryIcon()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""summary"":""Some Text"",
    ""icon"":""icon.ico""
}";

        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal((string?)"Some Text", (string?)modinfo.Summary);
        Assert.Equal((string?)"icon.ico", (string?)modinfo.Icon);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Null(modinfo.Version);
        Assert.Null(modinfo.SteamData);
    }

    [Fact]
    public void Parse_WithOneLanguage()
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
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Single(modinfo.Languages);
        Assert.True(modinfo.LanguagesExplicitlySet);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Null(modinfo.Version);
        Assert.Null(modinfo.SteamData);
        Assert.True(modinfo.LanguagesExplicitlySet);
    }

    [Fact]
    public void Parse_WithManyLanguages()
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
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal(2, modinfo.Languages.Count);
        Assert.True(modinfo.LanguagesExplicitlySet);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Null(modinfo.Version);
        Assert.Null(modinfo.SteamData);
        Assert.True(modinfo.LanguagesExplicitlySet);
    }

    [Fact]
    public void Parse_WithDuplicateLanguage()
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
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Single(modinfo.Languages);
        Assert.True(modinfo.LanguagesExplicitlySet);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Single(modinfo.Languages);
        Assert.True(modinfo.LanguagesExplicitlySet);
    }

    [Fact]
    public void Parse_WithDuplicateLanguage2()
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
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Single(modinfo.Languages);
        Assert.True(modinfo.LanguagesExplicitlySet);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Single(modinfo.Languages);
        Assert.True(modinfo.LanguagesExplicitlySet);
    }

    [Fact]
    public void Parse_WithoutLanguage()
    {
        var data = @"
{
    ""name"":""My Mod Name""
}";
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Single(modinfo.Languages);
        Assert.Equal((string?)"en", (string?)modinfo.Languages.ElementAt(0).Code);
        Assert.Equal(LanguageSupportLevel.FullLocalized, modinfo.Languages.ElementAt(0).Support);
        Assert.False(modinfo.LanguagesExplicitlySet);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Single(modinfo.Languages);
        Assert.Equal((string?)"en", (string?)modinfo.Languages.ElementAt(0).Code);
        Assert.Equal(LanguageSupportLevel.FullLocalized, modinfo.Languages.ElementAt(0).Support);
        Assert.False(modinfo.LanguagesExplicitlySet);
    }

    [Fact]
    public void Parse_WithEmptyLanguage()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""languages"":[]
}";
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Single(modinfo.Languages);
        Assert.False(modinfo.LanguagesExplicitlySet);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Null(modinfo.Version);
        Assert.Null(modinfo.SteamData);
        Assert.False(modinfo.LanguagesExplicitlySet);
    }

    [Fact]
    public void Parse_WithLanguageAndArbitraryObject_Throws()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""languages"":[
        {
            ""code"":""en""
        },
        {
            ""other"": ""value""
        }
    ]
}";
        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModInfo));
        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out var errors));
        Assert.Equivalent(new List<string>{"required", ""}, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key), true);

        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(data)));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)))));
    }

    [Fact]
    public void Parse_OnlyArbitraryObject_Throws()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""languages"":[
        {
            ""other"": ""value""
        }
    ]
}";
        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModInfo));
        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out var errors));
        Assert.Equivalent(new List<string> { "required", "" }, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key), true);

        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(data)));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)))));
    }

    [Fact]
    public void Parse_WithoutDeps()
    {
        var data = @"
{
    ""name"":""My Mod Name""
}";
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Empty(modinfo.Dependencies);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Empty(modinfo.Dependencies);
    }


    [Fact]
    public void Parse_WithManyDeps()
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
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal(2, modinfo.Dependencies.Count);
        Assert.Equal((string?)"12313", (string?)modinfo.Dependencies[0].Identifier);
        Assert.Equal((string?)"654987", (string?)modinfo.Dependencies[1].Identifier);
        Assert.Equal(DependencyResolveLayout.ResolveRecursive, modinfo.Dependencies.ResolveLayout);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal(2, modinfo.Dependencies.Count);
        Assert.Equal((string?)"12313", (string?)modinfo.Dependencies[0].Identifier);
        Assert.Equal((string?)"654987", (string?)modinfo.Dependencies[1].Identifier);
        Assert.Equal(DependencyResolveLayout.ResolveRecursive, modinfo.Dependencies.ResolveLayout);
    }

    [Fact]
    public void Parse_WithDepsLayout()
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
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal(2, modinfo.Dependencies.Count);
        Assert.Equal((string?)"12313", (string?)modinfo.Dependencies[0].Identifier);
        Assert.Equal((string?)"654987", (string?)modinfo.Dependencies[1].Identifier);
        Assert.Equal(DependencyResolveLayout.FullResolved, modinfo.Dependencies.ResolveLayout);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal(2, modinfo.Dependencies.Count);
        Assert.Equal((string?)"12313", (string?)modinfo.Dependencies[0].Identifier);
        Assert.Equal((string?)"654987", (string?)modinfo.Dependencies[1].Identifier);
        Assert.Equal(DependencyResolveLayout.FullResolved, modinfo.Dependencies.ResolveLayout);
    }

    [Fact]
    public void Parse_WithDepsLayout_ThrowsModinfoParseException()
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

        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModInfo));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(data)));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)))));

        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out var errors));
        Assert.Equivalent(new List<string>{ "oneOf", "enum", "type"}, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key).Distinct(), true);
    }

    [Fact]
    public void Parse_WithUnknownDepsLayout_ThrowsModinfoParseException()
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
        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModInfo));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(data)));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)))));

        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out var errors));
        Assert.Equivalent(new List<string> { "oneOf", "enum", "type" }, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key).Distinct(), true);
    }

    [Fact]
    public void Parse_WithEmptyDeps_ThrowsModinfoParseException()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [ ]
}";
        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModInfo));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(data)));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)))));

        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out var errors));
        Assert.Equivalent(new List<string> { "oneOf", "contains" }, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key).Distinct(), true);
    }

    [Fact]
    public void Parse_WithEmptyDeps_ThrowsModinfoParseException2()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [ ""FullResolved"" ]
}";
        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModInfo));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(data)));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)))));

        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out var errors));
        Assert.Equivalent(new List<string> { "contains", "type" }, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key).Distinct(), true);
    }

    [Fact]
    public void Parse_WithIncompleteModRef_ThrowsModinfoParseException2()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""dependencies"": [ ""FullResolved"", {""identifier"":""123""} ]
}";
        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModInfo));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(data)));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)))));

        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out var errors));
        Assert.Equivalent(new List<string> { "oneOf", "contains", "required", "type" }, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key).Distinct(), true);
    }

    [Fact]
    public void Parse_WithIncompatibleCustom()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""custom"": [ {""key"":""value""}, {""key2"": ""value""} ]
}";
        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModInfo));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(data)));
        Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)))));

        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out var errors));
        Assert.Equivalent(new List<string> { "type" }, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key).Distinct(), true);
    }


    [Fact]
    public void Parse_Custom_Empty()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""custom"": {}
}";
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Empty(modinfo.Custom);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Empty(modinfo.Custom);
    }

    [Fact]
    public void Parse_SteamData()
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
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.NotNull(modinfo.SteamData);
        Assert.Equal((string?)"123", (string?)modinfo.SteamData!.Id);
        Assert.Equal((string?)"test", (string?)modinfo.SteamData.Title);
        Assert.Equal((string?)"path", (string?)modinfo.SteamData.ContentFolder);
        Assert.Equal(SteamWorkshopVisibility.Public, modinfo.SteamData.Visibility);
        Assert.Equal((string?)"test", (string?)modinfo.SteamData.Metadata);
        Assert.Null(modinfo.SteamData.Description);
        Assert.Equal(2, Enumerable.Count<string>(modinfo.SteamData.Tags));

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.NotNull(modinfo.SteamData);
        Assert.Equal((string?)"123", (string?)modinfo.SteamData!.Id);
        Assert.Equal((string?)"test", (string?)modinfo.SteamData.Title);
        Assert.Equal((string?)"path", (string?)modinfo.SteamData.ContentFolder);
        Assert.Equal(SteamWorkshopVisibility.Public, modinfo.SteamData.Visibility);
        Assert.Equal((string?)"test", (string?)modinfo.SteamData.Metadata);
        Assert.Null(modinfo.SteamData.Description);
        Assert.Equal(2, Enumerable.Count<string>(modinfo.SteamData.Tags));
    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        yield return [null!, new[] { "type" }];
        yield return [string.Empty, new[] { "required" }];
        yield return ["{}", new[] { "required" }];
        yield return
        [
            @"{
  ""version"": ""1.0.0""
}", new[] { "required" }
        ];
    }

    [Theory]
    [MemberData(nameof(GetInvalidData))]
    public void Parse_FailingParseTest(string? data, IList<string> expectedErrorKeys)
    {
        if (data is null)
        {
            Assert.Throws<ArgumentNullException>(() => ModInfoJsonSchema.Evaluate(data!, EvaluationType.ModInfo));
            Assert.Throws<ArgumentNullException>(() => ModinfoData.Parse(data!));
            Assert.Throws<ArgumentNullException>(() => ModinfoData.Parse((Stream)null!));
            Assert.False(ModInfoJsonSchema.IsValid(null, EvaluationType.ModInfo, out var errors));
            Assert.Equivalent(expectedErrorKeys, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key), true);
        }
        else
        {
            if (data != string.Empty)
            {
                Assert.Throws<ModinfoParseException>((Action)(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data)!, EvaluationType.ModInfo)));
                Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out var errors));
                Assert.Equivalent(expectedErrorKeys, Enumerable.Select<KeyValuePair<string, string>, string>(errors, x => x.Key), true);
            }
            Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(data)));
            Assert.Throws<ModinfoParseException>((Func<object?>)(() => ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)))));
        }
    }

    [Fact]
    public void Parse_Custom()
    {
        var data = @"{
  ""name"": ""Test"",
  ""custom"": {
    ""key1"": ""value"",
    ""key2"": 2,
    ""key3"": {},
    ""key4"": null
  }
}";

        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        var modinfo = ModinfoData.Parse(data);
        Assert.Equal(JsonValueKind.String, ((JsonElement)modinfo.Custom["key1"]).ValueKind);
        Assert.Equal("value", ((JsonElement)modinfo.Custom["key1"]).GetString());
        Assert.Equal(JsonValueKind.Number, ((JsonElement)modinfo.Custom["key2"]).ValueKind);
        Assert.Equal(2, ((JsonElement)modinfo.Custom["key2"]).GetInt32());
        Assert.Equal(JsonValueKind.Object, ((JsonElement)modinfo.Custom["key3"]).ValueKind);
        Assert.Null(modinfo.Custom["key4"]);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal(JsonValueKind.String, ((JsonElement)modinfo.Custom["key1"]).ValueKind);
        Assert.Equal("value", ((JsonElement)modinfo.Custom["key1"]).GetString());
        Assert.Equal(JsonValueKind.Number, ((JsonElement)modinfo.Custom["key2"]).ValueKind);
        Assert.Equal(2, ((JsonElement)modinfo.Custom["key2"]).GetInt32());
        Assert.Equal(JsonValueKind.Object, ((JsonElement)modinfo.Custom["key3"]).ValueKind);
        Assert.Null(modinfo.Custom["key4"]);
    }

    [Fact]
    public void Parse_Custom_WithNonUniqueKeys()
    {
        var data = @"{
  ""name"": ""Test"",
  ""custom"": {
    ""key1"": ""value"",
    ""key1"": 2,
    ""key1"": {},
    ""key1"": null
  }
}";

        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equivalent(new Dictionary<string, object?> { { "key1", null } }, modinfo.Custom, true);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equivalent(new Dictionary<string, object?> { { "key1", null } }, modinfo.Custom, true);
    }

    #endregion

    #region ToJson

    [Fact]
    public void ToJson()
    {
        var modinfo = new ModinfoData("Test") { Version = SemVersion.ParsedFrom(1, 1, 1, "BETA") };
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.Contains((string)@"""version"": ""1.1.1-BETA""", (string?)data);
        Assert.DoesNotContain((string)@"""custom"":", (string?)data);

        data = GetStringFromStream(modinfo);
        Assert.Contains((string)@"""version"": ""1.1.1-BETA""", (string?)data);
        Assert.DoesNotContain((string)@"""custom"":", (string?)data);
    }

    [Fact]
    public void ToJson_DependencyList()
    {
        var modinfo = new ModinfoData("Test")
        {
            Dependencies = new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default) }, DependencyResolveLayout.FullResolved)
        };
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.Contains((string)@"""FullResolved"",", (string?)data);
        Assert.Contains((string)@"""identifier"": ""123""", (string?)data);
        Assert.Contains((string)@"""modtype"": 0", (string?)data);

        data = GetStringFromStream(modinfo);
        Assert.Contains((string)@"""FullResolved"",", (string?)data);
        Assert.Contains((string)@"""identifier"": ""123""", (string?)data);
        Assert.Contains((string)@"""modtype"": 0", (string?)data);
    }

    [Fact]
    public void ToJson_EmptyDependencyList()
    {
        var modinfo = new ModinfoData("Test")
        {
            Dependencies = DependencyList.EmptyDependencyList
        };
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.DoesNotContain((string)@"""dependencies""", (string?)data);

        data = GetStringFromStream(modinfo);
        Assert.DoesNotContain((string)@"""dependencies""", (string?)data);
    }

    [Fact]
    public void ToJson_ModRefRange()
    {
        var modinfo = new ModinfoData("Test")
        {
            Dependencies = new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default, SemVersionRange.Parse("1.*")) }, DependencyResolveLayout.ResolveRecursive)
        };
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.DoesNotContain((string)@"""ResolveRecursive"",", (string?)data);
        Assert.Contains((string)@"""version-range"": ""1.*""", (string?)data);

        data = GetStringFromStream(modinfo);
        Assert.DoesNotContain((string)@"""ResolveRecursive"",", (string?)data);
        Assert.Contains((string)@"""version-range"": ""1.*""", (string?)data);
    }

    [Fact]
    public void ToJson_SummaryIcon()
    {
        var modinfo = new ModinfoData("Test")
        {
            Summary = "Summary",
            Icon = "icon.ico"
        };
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.Contains((string)@"""summary"": ""Summary""", (string?)data);
        Assert.Contains((string)@"""icon"": ""icon.ico""", (string?)data);

        data = GetStringFromStream(modinfo);
        Assert.Contains((string)@"""summary"": ""Summary""", (string?)data);
        Assert.Contains((string)@"""icon"": ""icon.ico""", (string?)data);
    }

    [Fact]
    public void ToJson_Custom()
    {
        var modinfo = new ModinfoData("Test")
        {
            Custom = new Dictionary<string, object>
            {
                {"key1", "value"},
                {"key2", 2},
                {"key3", new object()},
                {"key4", null!},
            }
        };

        var expected = @"{
  ""name"": ""Test"",
  ""custom"": {
    ""key1"": ""value"",
    ""key2"": 2,
    ""key3"": {},
    ""key4"": null
  }
}";
        var data = modinfo.ToJson();
        output.WriteLine(data);
        Assert.Equal(expected, (string?)data);

        data = GetStringFromStream(modinfo);
        Assert.Equal(expected, (string?)data);
    }

    [Fact]
    public void ToJson_Full()
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

        Assert.True(modinfo.LanguagesExplicitlySet);
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
      ""code"": ""fr""
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

        Assert.Equal(expected, (string?)data);

        data = GetStringFromStream(modinfo);
        Assert.Equal(expected, (string?)data);
    }

    [Fact]
    public void ToJson_CustomDefaultLanguageOnly()
    {
        var modinfo = new ModinfoData("Test")
        {
            Languages = new List<ILanguageInfo>
            {
                new LanguageInfo("en", 0),
            }
        };
        Assert.True(modinfo.LanguagesExplicitlySet);
        var data = modinfo.ToJson();
        Assert.DoesNotContain((string)@"""languages""", (string?)data);
        output.WriteLine(data);

        data = GetStringFromStream(modinfo);
        Assert.DoesNotContain((string)@"""languages""", (string?)data);
    }

    [Fact]
    public void ToJson_DefaultOnlyLanguage()
    {
        var modinfo = new ModinfoData("Test")
        {
            Languages = new List<ILanguageInfo>
            {
                LanguageInfo.Default
            }
        };
        Assert.True(modinfo.LanguagesExplicitlySet);

        var data = modinfo.ToJson();
        Assert.DoesNotContain((string)@"""languages""", (string?)data);
        output.WriteLine(data);

        data = GetStringFromStream(modinfo);
        Assert.DoesNotContain((string)@"""languages""", (string?)data);
    }

    [Fact]
    public void ToJson_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ModinfoData("name").ToJson(null!));
    }

    [Fact]
    public void Parse_TolerantVersion()
    {
        var data = @"
{
    ""name"":""My Mod Name"",
    ""version"": ""1.0""
}";
        TestUtilities.Evaluate(data, EvaluationType.ModInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModInfo, out _));

        var modinfo = ModinfoData.Parse(data);
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal(new SemVersion(1, 0, 0), modinfo.Version);

        modinfo = ModinfoData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal((string?)"My Mod Name", (string?)modinfo.Name);
        Assert.Equal(new SemVersion(1, 0, 0), modinfo.Version);
    }

    #endregion
    
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
                { "key1", "value" },
                { "key2", 2 },
                { "key3", new object() },
                { "key4", null! },
            },
            SteamData = new SteamData("123", "folder", SteamWorkshopVisibility.Public, "Test", ["FOC"])
        };

        Assert.Equal((string?)"Test", (string?)modinfo.Name);
        Assert.Equal((string?)"Summary", (string?)modinfo.Summary);
        Assert.Equal((string?)"icon.ico", (string?)modinfo.Icon);
        Assert.Equivalent(
            new List<LanguageInfo>{new("en", 0), new("de", LanguageSupportLevel.Text), new("fr", LanguageSupportLevel.FullLocalized)}, 
            modinfo.Languages,
            true);
        Assert.Equivalent(new Dictionary<string, object>
        {
            { "key1", "value" },
            { "key2", 2 },
            { "key3", new object() },
            { "key4", null! },
        }, modinfo.Custom, true);
        Assert.Equal((string?)"123", (string?)modinfo.SteamData.Id);
        Assert.Equal((string?)"folder", (string?)modinfo.SteamData.ContentFolder);
        Assert.Equal(SteamWorkshopVisibility.Public, modinfo.SteamData.Visibility);
        Assert.Equal((string?)"Test", (string?)modinfo.SteamData.Title);
        Assert.Equivalent(new List<string>{"FOC"}, modinfo.SteamData.Tags, true);
        Assert.True(modinfo.LanguagesExplicitlySet);

        var other = new ModinfoData(modinfo);

        Assert.Equal((string?)modinfo.Name, (string?)other.Name);
        Assert.Equal((string?)modinfo.Summary, (string?)other.Summary);
        Assert.Equal((string?)modinfo.Icon, (string?)other.Icon);
        Assert.Equal(modinfo.Languages, other.Languages);
        Assert.Equivalent(modinfo.Custom, other.Custom, true);
        Assert.Equal((string?)modinfo.SteamData.Id, (string?)other.SteamData!.Id);
        Assert.Equal((string?)modinfo.SteamData.ContentFolder, (string?)other.SteamData.ContentFolder);
        Assert.Equal(modinfo.SteamData.Visibility, other.SteamData.Visibility);
        Assert.Equal((string?)modinfo.SteamData.Title, (string?)other.SteamData.Title);
        Assert.Equivalent(modinfo.SteamData.Tags, other.SteamData.Tags, true);
        Assert.True(other.LanguagesExplicitlySet);
    }

    [Fact]
    public void Ctor_EmptyLanguage_ShouldFallbackToDefault()
    {
        var modinfo = new ModinfoData("name") { Languages = [] };
        var lang = Assert.Single(modinfo.Languages);
        Assert.Equal(LanguageInfo.Default, lang);
        Assert.False(modinfo.LanguagesExplicitlySet);

        var newInfo = new ModinfoData(modinfo);
        lang = Assert.Single(newInfo.Languages);
        Assert.Equal(LanguageInfo.Default, lang);
        Assert.False(modinfo.LanguagesExplicitlySet);

        var newParsed = ModinfoData.Parse(newInfo.ToJson());
        lang = Assert.Single(newParsed.Languages);
        Assert.Equal(LanguageInfo.Default, lang);
        Assert.False(modinfo.LanguagesExplicitlySet);
    }

    [Fact]
    public void Ctor_EmptyDependencies()
    {
        var modinfo = new ModinfoData("name") { Dependencies = DependencyList.EmptyDependencyList };
        Assert.Empty(modinfo.Dependencies);

        var newInfo = new ModinfoData(modinfo);
        Assert.Empty(newInfo.Dependencies);

        var newParsed = ModinfoData.Parse(newInfo.ToJson());
        Assert.Empty(newParsed.Dependencies);
    }

    [Fact]
    public void Languages_Dependencies_InitNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ModinfoData("name") { Languages = null! });
        Assert.Throws<ArgumentNullException>(() => new ModinfoData("name") { Dependencies = null! });
        Assert.Throws<ArgumentNullException>(() => new ModinfoData("name") { Custom = null! });
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
                { "key1", "value" },
                { "key2", 2 },
                { "key3", new object() },
                { "key4", null! },
            },
            SteamData = new SteamData("123", "folder", SteamWorkshopVisibility.Public, "Test", ["FOC"])
        };

        var json = modinfo.ToJson();
        var other = ModinfoData.Parse(json);
        var otherJson = other.ToJson();

        Assert.Equal((string?)json, (string?)otherJson);

        Assert.Equal((string?)modinfo.Name, (string?)other.Name);
        Assert.Equal((string?)modinfo.Summary, (string?)other.Summary);
        Assert.Equal((string?)modinfo.Icon, (string?)other.Icon);
        Assert.Equal(modinfo.Languages, other.Languages);
        Assert.True(other.LanguagesExplicitlySet);
        Assert.Equal((string?)modinfo.SteamData.Id, (string?)other.SteamData!.Id);
        Assert.Equal((string?)modinfo.SteamData.ContentFolder, (string?)other.SteamData.ContentFolder);
        Assert.Equal(modinfo.SteamData.Visibility, other.SteamData.Visibility);
        Assert.Equal((string?)modinfo.SteamData.Title, (string?)other.SteamData.Title);
        Assert.Equivalent(modinfo.SteamData.Tags, other.SteamData.Tags, true);

        Assert.Equal(JsonValueKind.String, ((JsonElement)other.Custom["key1"]).ValueKind);
        Assert.Equal("value", ((JsonElement)other.Custom["key1"]).GetString());
        Assert.Equal(JsonValueKind.Number, ((JsonElement)other.Custom["key2"]).ValueKind);
        Assert.Equal(2, ((JsonElement)other.Custom["key2"]).GetInt32());
        Assert.Equal(JsonValueKind.Object, ((JsonElement)other.Custom["key3"]).ValueKind);
        Assert.Null(other.Custom["key4"]);
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

        Assert.Equal((string?)"Name", (string?)modinfo.Name);
        Assert.Equal(SemVersion.Parse("1.2.3-beta1"), modinfo.Version);
        Assert.Equal(DependencyResolveLayout.FullResolved, modinfo.Dependencies.ResolveLayout);
        Assert.Equal([new ModReference("1", ModType.Default), new ModReference("2", ModType.Workshops, SemVersionRange.Parse("*"))], Enumerable.ToList(modinfo.Dependencies));
        Assert.Equivalent(new Dictionary<string, string>(), modinfo.Custom, true);
        Assert.Equal([LanguageInfo.Default], modinfo.Languages);
        Assert.False(modinfo.LanguagesExplicitlySet);
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

    private static string GetStringFromStream(ModinfoData modinfo)
    {
        var ms = new MemoryStream();
        modinfo.ToJson(ms);
        return Encoding.UTF8.GetString(ms.ToArray());
    }
}