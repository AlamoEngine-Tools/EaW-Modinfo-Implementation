using System.Collections.Generic;
using System.Linq;
using EawModinfo.Model;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using Newtonsoft.Json.Linq;
using SemanticVersioning;
using Xunit;

namespace EawModinfo.Tests
{
    public class ModinfoDataTest
    {
        private const string InvalidJsonData = @"{
  ""version"": ""1.0.0"",
}";

        [Fact]
        public void MinimalParseTest()
        {
            var data = @"
{
    'name':'My Mod Name'
}";

            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Null(modinfo.Version);
            Assert.Null(modinfo.SteamData);
        }
        
        [Fact]
        public void VersionParseTest()
        {
            var data = @"
{
    'name':'My Mod Name',
    'version':'1.1.1-BETA'
}";

            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Equal(new Version(1,1,1, "BETA"), modinfo.Version);
        }

        [Fact]
        public void LangParseTest1()
        {
            var data = @"
{
    'name':'My Mod Name',
    'languages':[
        {
            'code':'en'
        }
    ]
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Single(modinfo.Languages);
        }

        [Fact]
        public void LangParseTest2()
        {
            var data = @"
{
    'name':'My Mod Name',
    'languages':[
        {
            'code':'en'
        },
        {
            'code':'de'
        }
    ]
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Equal(2, modinfo.Languages.Count());
        }

        [Fact]
        public void LangParseTest3()
        {
            var data = @"
{
    'name':'My Mod Name',
    'languages':[
        {
            'code':'en'
        },
        {
            'code':'en'
        }
    ]
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Single(modinfo.Languages);
        }

        [Fact]
        public void LangParseTest4()
        {
            var data = @"
{
    'name':'My Mod Name',
    'languages':[
        {
            'code':'en'
        },
        {
            'code':'en',
            'support':3
        }
    ]
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Single(modinfo.Languages);
        }

        [Fact]
        public void LangParseTest5()
        {
            var data = @"
{
    'name':'My Mod Name'
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Single(modinfo.Languages);
            Assert.Equal("en",modinfo.Languages.ElementAt(0).Code);
            Assert.Equal(LanguageSupportLevel.FullLocalized,modinfo.Languages.ElementAt(0).Support);
        }

        [Fact]
        public void ModRefParseTest1()
        {
            var data = @"
{
    'name':'My Mod Name'
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Equal(0, modinfo.Dependencies.Count);
            Assert.Equal(DependencyResolveLayout.ResolveRecursive, modinfo.Dependencies.ResolveLayout);
        }


        [Fact]
        public void ModRefParseTest2()
        {
            var data = @"
{
    'name':'My Mod Name',
    'dependencies': [
        {
            'identifier':'12313',
            'modtype':1
        },
        {
            'identifier':'654987',
            'modtype':1
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
        public void ModRefParseTestWithLayout()
        {
            var data = @"
{
    'name':'My Mod Name',
    'dependencies': [
        'FullResolved',
        {
            'identifier':'12313',
            'modtype':1
        },
        {
            'identifier':'654987',
            'modtype':1
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
        public void ModRefParseTestFailure()
        {
            var data = @"
{
    'name':'My Mod Name',
    'dependencies': [
        {
            'identifier':'12313',
            'modtype':1
        },
        'FullResolved',
        {
            'identifier':'654987',
            'modtype':1
        }
    ]
}";
            Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
        }

        [Fact]
        public void ModRefParseTestFailure2()
        {
            var data = @"
{
    'name':'My Mod Name',
    'dependencies': [
        'BlaBlub',
        {
            'identifier':'12313',
            'modtype':1
        },
        {
            'identifier':'654987',
            'modtype':1
        }
    ]
}";
            Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
        }


        [Fact]
        public void CustomParseTest()
        {
            var data = @"
{
    'name':'My Mod Name',
    'custom': {
        
    }
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Equal(0, modinfo.Custom.Count);
        }

        [Fact]
        public void CustomParseTest1()
        {
            var data = @"
{
    'name':'My Mod Name',
    'custom': {
        'test-key':{},
        'test-key2':'123',
    }
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Equal(2, modinfo.Custom.Count);
            Assert.Equal("123", modinfo.Custom["test-key2"]);
            Assert.Equal(new JObject(), modinfo.Custom["test-key"]);
        }


        [Fact]
        public void SteamDataParse()
        {
            var data = @"
{
    'name':'My Mod Name',
    'steamdata': {
        'publishedfileid':'123',
        'contentfolder':'path',
        'visibility':0,
        'title':'test',
        'tags':[
            'foc', 'eaw'
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
            Assert.Null(modinfo.SteamData.Metadata);
            Assert.Equal(2, modinfo.SteamData.Tags.Count());
            
        }

        public static IEnumerable<object[]> GetInvalidData()
        {
            yield return new object[]
            {
                string.Empty
            };
            yield return new object[]
            {
                @"
{
}"
            };
            yield return new object[]
            {
                InvalidJsonData
            };
        }

        [Theory]
        [MemberData(nameof(GetInvalidData))]
        public void FailingParseTest(string data)
        {
            Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
        }

        [Fact]
        public void WriterTest()
        {
            var modinfo = new ModinfoData("Test") { Version = new Version(1, 1, 1, "BETA")};
            var data = modinfo.ToJson(false);
            Assert.Contains(@"""version"": ""1.1.1-BETA""", data);
            Assert.DoesNotContain(@"""custom"":", data);
        }

        [Fact]
        public void WriterTestDependencyList()
        {
            var modinfo = new ModinfoData("Test")
            {
                Dependencies = new DependencyList(new List<IModReference>{new ModReference("123", ModType.Default)}, DependencyResolveLayout.FullResolved)
            };
            var data = modinfo.ToJson(false);
            Assert.Contains(@"""FullResolved"",",data);
        }

        [Fact]
        public void WriterTestModRefRange()
        {
            var modinfo = new ModinfoData("Test")
            {
                Dependencies = new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default, new Range("1.x")) }, DependencyResolveLayout.ResolveRecursive)
            };
            var data = modinfo.ToJson(false);
            Assert.Contains(@"""version-range"": ""1.x""", data);
        }

        [Fact]
        public void TolerantVersionParseTest()
        {
            var data = @"
{
    'name':'My Mod Name',
    'version': '1.0'
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Equal(new Version(1, 0, 0) , modinfo.Version);
        }
    }
}