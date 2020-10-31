using EawModinfo.Model.Steam;
using EawModinfo.Spec.Steam;
using Xunit;

namespace EawModinfo.Tests
{
    public class SteamDataWriteTests
    {
        [Fact]
        public static void WriteTest()
        {
            var steamData = new SteamData
            {
                ContentFolder = "Test",
                Id = "123",
                Tags = new[] {"FOC"},
                Visibility = SteamWorkshopVisibility.Private,
            };


            var data = steamData.ToJson(false);
            Assert.Contains(@"""contentfolder"": ""Test""", data);
            Assert.Contains(@"""publishedfileid"": ""123""", data);
            Assert.Contains(@"""visibility"": 2", data);
            Assert.DoesNotContain(@"""metadata"":", data);


            var steamDat2 = new SteamData
            {
                ContentFolder = "Test",
                Id = "123",
                Tags = new[] {"FOC"},
                Visibility = SteamWorkshopVisibility.Private,
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
