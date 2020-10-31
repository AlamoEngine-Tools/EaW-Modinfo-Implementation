using System.Collections.Generic;
using System.Linq;
using EawModinfo.Model;
using EawModinfo.Model.Steam;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using NuGet.Versioning;
using Xunit;

namespace EawModinfo.Tests
{
    public class MergeTests
    {
        [Fact]
        public void Merge()
        {
            var mainData = new ModinfoData
            {
                Name = "Mod",
                Icon = "icon.ico",
                Languages = new[] {new LanguageInfo {Code = "en"}},
                Dependencies = new List<IModReference>(new[] {new ModReference {Identifier = "bla"}}),
                Custom = new Dictionary<string, object>(new[]
                {
                    new KeyValuePair<string, object>("testKey1", "value"),
                })
            };

            var variantData = new ModinfoData
            {
                Name = "Variant of Mod",
                Languages = new[] {new LanguageInfo {Code = "en"}, new LanguageInfo {Code = "de"}},
                SteamData = new SteamData {Id = "123", Tags = new[] {"FOC"}, ContentFolder = "bla"},
                Custom =
                    new Dictionary<string, object>(new[] {new KeyValuePair<string, object>("testKey2", "value")}),
                Dependencies = new List<IModReference>(new[]
                {
                    new ModReference {Identifier = "bla"}, new ModReference {Identifier = "blub"}
                }),
                Version = SemanticVersion.Parse("1.2.2")
            };

            var newData = variantData.MergeInto(mainData);
            
            Assert.Equal(variantData.Name, newData.Name);
            Assert.Equal(mainData.Icon, newData.Icon);
            Assert.Equal(2, newData.Languages.Count());
            Assert.Equal(2, newData.Dependencies.Count);
            Assert.Equal("bla", newData.Dependencies[0].Identifier);
            Assert.NotSame(mainData.Dependencies[0], newData.Dependencies[0]);
            Assert.NotNull(newData.SteamData);
            Assert.Equal(variantData.SteamData!.Id, newData.SteamData.Id);
            Assert.Equal(2, newData.Custom.Count);
            Assert.Equal(new SemanticVersion(1,2,2), newData.Version);
            Assert.NotSame(variantData.Version, newData.Version);


            var invalid = new ModinfoData();
            Assert.Throws<ModinfoException>(() => invalid.MergeInto(newData));
            Assert.Throws<ModinfoException>(() => newData.MergeInto(invalid));
        }
    }
}