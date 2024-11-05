using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using EawModinfo.Model;
using EawModinfo.Model.Json.Schema;
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
            Assert.Throws<ModinfoParseException>(() => SteamData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data))));
        }
        else
        {
            Assert.NotNull(expected);
            var steamData = SteamData.Parse(data);

            AssertSteamDataEquals(expected, steamData);

            steamData = SteamData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
            AssertSteamDataEquals(expected, steamData);
        }
    }

    [Fact]
    public void Parse_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => SteamData.Parse((string)null!));
        Assert.Throws<ArgumentNullException>(() => SteamData.Parse((Stream)null!));
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
    public static void Test_TagsAreUnique()
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