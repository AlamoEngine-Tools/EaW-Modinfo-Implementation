using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec.Steam;
using Xunit;

namespace EawModinfo.Tests
{
    public class SteamDataWriteTests
    {
        [Fact]
        public static void WriteTest()
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
    }
}
