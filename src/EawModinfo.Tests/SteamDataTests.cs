﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using EawModinfo.Model;
using EawModinfo.Model.Json.Schema;
using EawModinfo.Spec.Steam;
using Xunit;

namespace EawModinfo.Tests;

public class SteamDataTests
{
    public static IEnumerable<object[]> GetInvalidJsonData()
    {
        yield return [@"{}", new[] { "required" }];
        yield return [@"{""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""EAW""]}", new[] { "required" }];
        yield return [@"{""title"":""123"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""EAW""]}", new[] { "required" }];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""visibility"":0, ""tags"":[""EAW""]}", new[] { "required" }];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""tags"":[""EAW""]}", new[] { "required" }];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0}", new[] { "required" }];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[]}", new[] { "minItems", "contains" }];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""other""]}", new[] { "contains", "oneOf", "const", "const" }];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""eaw""]}", new[] { "contains", "oneOf", "const", "const" }];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""FOC"", ""a,b""]}", new[] { "pattern", "oneOf", "const", "const" }];
    }

    [Theory]
    [MemberData(nameof(GetInvalidJsonData))]
    public void Parse_Throws(string data, IList<string> expectedErrorKeys)
    {
            Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.SteamData));
            Assert.Throws<ModinfoParseException>(() => SteamData.Parse(data));
            Assert.Throws<ModinfoParseException>(() => SteamData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data))));

            Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.SteamData, out var errors));
            Assert.Equivalent(expectedErrorKeys, errors.Select(x => x.Key), true);
    }

    public static IEnumerable<object[]> GetJsonData()
    {
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""EAW""]}",
            new SteamData("123", "path", SteamWorkshopVisibility.Public, "123", ["EAW"])];
        yield return
        [
            @"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""EAW""], 
""metadata"":""test"", ""description"":""test"", ""previewfile"":""file""}",
            new SteamData("123", "path", SteamWorkshopVisibility.Public, "123", ["EAW"])
                { Description = "test", Metadata = "test", PreviewFile = "file" }
        ];
    }

    [Theory]
    [MemberData(nameof(GetJsonData))]
    public void Parse(string data, SteamData? expected)
    {
        Assert.NotNull(expected);

        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.SteamData, out _));
        TestUtilities.Evaluate(data, EvaluationType.SteamData);

        var steamData = SteamData.Parse(data);

        AssertSteamDataEquals(expected, steamData);

        steamData = SteamData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        AssertSteamDataEquals(expected, steamData);
    }

    [Fact]
    public void Parse_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => SteamData.Parse((string)null!));
        Assert.Throws<ArgumentNullException>(() => SteamData.Parse((Stream)null!));
    }

    [Fact]
    public static void ToJson()
    {
        var steamData = new SteamData("123", "Test", SteamWorkshopVisibility.Private, "Title", ["FOC"]);

        var data = steamData.ToJson();
        Assert.Contains(@"""contentfolder"": ""Test""", data);
        Assert.Contains(@"""publishedfileid"": ""123""", data);
        Assert.Contains(@"""visibility"": 2", data);
        Assert.Contains(@"""metadata"": """"", data);


        var steamDat2 = new SteamData("123", "Test", SteamWorkshopVisibility.Private, "Title", ["FOC"])
        {
            Metadata = "test"
        };
        var data2 = steamDat2.ToJson();
        Assert.Contains(@"""contentfolder"": ""Test""", data2);
        Assert.Contains(@"""publishedfileid"": ""123""", data2);
        Assert.Contains(@"""metadata"": ""test""", data2);
        Assert.Contains(@"""visibility"": 2", data2);
    }

    [Fact]
    public static void ToJson_DefaultData()
    {
        var steamData = new SteamData("123", "Test Folder", SteamWorkshopVisibility.Public, "Title", ["FOC", "SinglePlayer"])
        {
            Metadata = "some metadata",
            Description = "test123",
            PreviewFile = "preview"
        };


        var expected = @"{
  ""publishedfileid"": ""123"",
  ""contentfolder"": ""Test Folder"",
  ""previewfile"": ""preview"",
  ""visibility"": 0,
  ""title"": ""Title"",
  ""description"": ""test123"",
  ""metadata"": ""some metadata"",
  ""tags"": [
    ""FOC"",
    ""SinglePlayer""
  ]
}";
        Assert.Equal(expected, steamData.ToJson());

        var ms = new MemoryStream();
        steamData.ToJson(ms);
        Assert.Equal(expected, Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ToJson_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SteamData("name", "path", SteamWorkshopVisibility.Private, "title", ["FOC"]).ToJson(null!));
    }


    [Fact]
    public static void Ctor_TagsAreUnique()
    {
        var steamData = new SteamData("123", "Test", SteamWorkshopVisibility.Public, "Title", ["FOC", "foc", "FOC"]);
        Assert.Equivalent(new List<string>{"FOC", "foc"}, steamData.Tags, true);
    }


    [Fact]
    public void WriteReadEquals()
    {
        var steamData = new SteamData("123", "Test", SteamWorkshopVisibility.Private, "Title", ["FOC", "SinglePlayer"])
        {
            Description = "This is some text",
            Metadata = "some metadata",
            PreviewFile = "preview.png"
        };

        Assert.Equal("Test", steamData.ContentFolder);
        Assert.Equal("123", steamData.Id);
        Assert.Equivalent(new List<string>{"FOC", "SinglePlayer"}, steamData.Tags, true);
        Assert.Equal("Title", steamData.Title);
        Assert.Equal(SteamWorkshopVisibility.Private, steamData.Visibility);
        Assert.Equal("This is some text", steamData.Description);
        Assert.Equal("some metadata", steamData.Metadata);
        Assert.Equal("preview.png", steamData.PreviewFile);


        var json = steamData.ToJson();
        AssertSteamDataEquals(steamData, SteamData.Parse(json));

        using var ms = new MemoryStream();
        steamData.ToJson(ms);
        ms.Position = 0;
        AssertSteamDataEquals(steamData, SteamData.Parse(ms));
    }

    private static void AssertSteamDataEquals(ISteamData expected, ISteamData actual)
    {
        Assert.Equal(expected.ContentFolder, actual.ContentFolder);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equivalent(expected.Tags, actual.Tags, true);
        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.Visibility, actual.Visibility);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.Metadata, actual.Metadata);
        Assert.Equal(expected.PreviewFile, actual.PreviewFile);
    }

    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new SteamData(null!));
        Assert.Throws<ArgumentNullException>(() => new SteamData(null!, "path", SteamWorkshopVisibility.Private, "title", []));
        Assert.Throws<ArgumentNullException>(() => new SteamData("123", null!, SteamWorkshopVisibility.Private, "title", []));
        Assert.Throws<ArgumentNullException>(() => new SteamData("123", "path", SteamWorkshopVisibility.Private, null!,[]));
        Assert.Throws<ArgumentNullException>(() => new SteamData("123", "path", SteamWorkshopVisibility.Private, "title", null!));
    }
}