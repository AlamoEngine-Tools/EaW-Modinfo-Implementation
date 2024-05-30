using EawModinfo.Model.Json;
using EawModinfo.Spec.Steam;
using Xunit;

namespace EawModinfo.Tests;

public class SteamDataWriteTests
{
    [Fact]
    public static void Test_ToJson()
    {
        var steamData = new JsonSteamData
        {
            ContentFolder = "Test",
            Id = "123",
            Tags = new[] {"FOC"},
            Title = "Title",
            Visibility = SteamWorkshopVisibility.Private,
        };


        var data = steamData.ToJson(false);
        Assert.Contains(@"""contentfolder"": ""Test""", data);
        Assert.Contains(@"""publishedfileid"": ""123""", data);
        Assert.Contains(@"""visibility"": 2", data);
        Assert.Contains(@"""metadata"": """"", data);


        var steamDat2 = new JsonSteamData
        {
            ContentFolder = "Test",
            Id = "123",
            Tags = new[] {"FOC"},
            Visibility = SteamWorkshopVisibility.Private,
            Title = "Title",
            Metadata = "test"
        };
        var data2 = steamDat2.ToJson(false);
        Assert.Contains(@"""contentfolder"": ""Test""", data2);
        Assert.Contains(@"""publishedfileid"": ""123""", data2);
        Assert.Contains(@"""metadata"": ""test""", data2);
        Assert.Contains(@"""visibility"": 2", data2);
    }

    [Fact]
    public static void Test_ToJson_DefaultData()
    {
        var steamData = new JsonSteamData
        {
            ContentFolder = "Test Folder",
            Id = "123",
            Tags = new[] { "FOC", "SinglePlayer" },
            Title = "Title",
            Metadata = "some metadata",
            Visibility = SteamWorkshopVisibility.Public,
            Description = "test123",
            PreviewFile = "preview"
        };


        var data = steamData.ToJson(false);

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
}