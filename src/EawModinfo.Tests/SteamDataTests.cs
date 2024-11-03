using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec.Steam;
using Xunit;

namespace EawModinfo.Tests;

public class SteamDataTests
{
    public static IEnumerable<object[]> GetJsonData()
    {
        yield return [@"{}", null!, true];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""EAW""]}",
            new SteamData("123", "path", SteamWorkshopVisibility.Public, "123", ["EAW"]), false];
        yield return
        [
            @"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""EAW""], 
""metadata"":""test"", ""description"":""test"", ""previewfile"":""file""}",
            new SteamData("123", "path", SteamWorkshopVisibility.Public, "123", ["EAW"])
                { Description = "test", Metadata = "test", PreviewFile = "file" },
            false
        ];
        yield return [@"{""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""EAW""]}", null!, true];
        yield return [@"{""title"":""123"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""EAW""]}", null!, true];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""visibility"":0, ""tags"":[""EAW""]}", null!, true];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""tags"":[""EAW""]}", null!, true];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0}", null!, true];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[]}", null!, true];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""other""]}", null!, true];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""eaw""]}", null!, true];
        yield return [@"{""title"":""123"", ""contentfolder"":""path"", ""publishedfileid"":""123"", ""visibility"":0, ""tags"":[""FOC"", ""a,b""]}", null!, true];
    }

    [Theory]
    [MemberData(nameof(GetJsonData))]
    public void Test_Parse(string data, SteamData? expected, bool throws)
    {
        if (throws)
        {
            Assert.Throws<ModinfoParseException>(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.SteamData));
            Assert.Throws<ModinfoParseException>(() => SteamData.Parse(data));
        }
        else
        {
            Assert.NotNull(expected);
            var steamData = SteamData.Parse(data);

            Assert.Equal(expected.Title, steamData.Title);
            Assert.Equal(expected.ContentFolder, steamData.ContentFolder);
            Assert.Equal(expected.Description, steamData.Description);
            Assert.Equal(expected.Id, steamData.Id);
            Assert.Equal(expected.Metadata, steamData.Metadata);
            Assert.Equal(expected.Tags, steamData.Tags);
            Assert.Equal(expected.Visibility, steamData.Visibility);
            Assert.Equal(expected.PreviewFile, steamData.PreviewFile);
        }
    }

    [Fact]
    public static void Test_ToJson()
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
    public static void Test_ToJson_DefaultData()
    {
        var steamData = new SteamData("123", "Test Folder", SteamWorkshopVisibility.Public, "Title", ["FOC", "SinglePlayer"])
        {
            Metadata = "some metadata",
            Description = "test123",
            PreviewFile = "preview"
        };


        var data = steamData.ToJson();

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
        Assert.Equal(expected, data);
    }


    [Fact]
    public static void Test_TagsAreUnique()
    {
        var steamData = new SteamData("123", "Test", SteamWorkshopVisibility.Public, "Title", ["FOC", "foc", "FOC"]);
        Assert.Equivalent(new List<string>{"FOC", "foc"}, steamData.Tags);
    }


    [Fact]
    public static void WriteReadEquals()
    {
        var steamData = new SteamData("123", "Test", SteamWorkshopVisibility.Private, "Title", ["FOC", "SinglePlayer"])
        {
            Description = "This is some text",
            Metadata = "some metadata",
            PreviewFile = "preview.png"
        };

        Assert.Equal("Test", steamData.ContentFolder);
        Assert.Equal("123", steamData.Id);
        Assert.Equivalent(new List<string>{"FOC", "SinglePlayer"}, steamData.Tags);
        Assert.Equal("Title", steamData.Title);
        Assert.Equal(SteamWorkshopVisibility.Private, steamData.Visibility);
        Assert.Equal("This is some text", steamData.Description);
        Assert.Equal("some metadata", steamData.Metadata);
        Assert.Equal("preview.png", steamData.PreviewFile);


        var json = steamData.ToJson();
        var newSteamData = SteamData.Parse(json);

        Assert.Equal(steamData.ContentFolder, newSteamData.ContentFolder);
        Assert.Equal(steamData.Id, newSteamData.Id);
        Assert.Equivalent(steamData.Tags, newSteamData.Tags);
        Assert.Equal(steamData.Title, newSteamData.Title);
        Assert.Equal(steamData.Visibility, newSteamData.Visibility);
        Assert.Equal(steamData.Description, newSteamData.Description);
        Assert.Equal(steamData.Metadata, newSteamData.Metadata);
        Assert.Equal(steamData.PreviewFile, newSteamData.PreviewFile);
    }

    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new SteamData(null!));
    }
}