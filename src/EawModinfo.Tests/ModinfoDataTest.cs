using System.Collections.Generic;
using System.Linq;
using EawModinfo.Model;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using Xunit;

namespace EawModinfo.Tests
{
    public class ModinfoDataTest
    {
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
            Assert.Equal(new SemanticVersion(1,1,1, "BETA"), modinfo.Version);
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
            Assert.Single(modinfo.InternalLanguages);
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
            Assert.Equal(2, modinfo.InternalLanguages.Count());
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
            Assert.Single(modinfo.InternalLanguages);
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
            Assert.Single(modinfo.InternalLanguages);
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
            Assert.Single(modinfo.InternalLanguages);
            Assert.Equal("en",modinfo.Languages.ElementAt(0).Code);
            Assert.Equal("en",modinfo.InternalLanguages.ElementAt(0).Code);
            Assert.Equal(LanguageSupportLevel.FullLocalized,modinfo.Languages.ElementAt(0).Support);
            Assert.Equal(LanguageSupportLevel.FullLocalized,modinfo.InternalLanguages.ElementAt(0).Support);
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
        'tags':[
            'foc', 'eaw'
        ]
    }
}";
            var modinfo = ModinfoData.Parse(data);
            Assert.Equal("My Mod Name", modinfo.Name);
            Assert.Equal("123", modinfo.SteamData.Id);
            Assert.Equal("path", modinfo.SteamData.ContentFolder);
            Assert.Equal(SteamWorkshopVisibility.Public, modinfo.SteamData.Visibility);
            Assert.Null(modinfo.SteamData.Metadata);
            Assert.Equal(2, modinfo.SteamData.Tags.Count());
            
        }

        public static IEnumerable<object[]> GetInvalidData()
        {
            yield return new[]
            {
                string.Empty
            };
            yield return new[]
            {
                @"
{
}"
            };
        }

        [Theory]
        [MemberData(nameof(GetInvalidData))]
        public void FailingParseTest(string data)
        {
            Assert.Throws<ModinfoParseException>(() => ModinfoData.Parse(data));
        }


        [Fact]
        public void ModIdentityEqualCheck()
        {
            IModIdentity i1 = new ModinfoData{Name = "A"};
            IModIdentity i2 = new ModinfoData{Name = "A"};

            Assert.Equal(i1, i2);

            IModIdentity i3 = new ModinfoData { Name = "A", Version = new SemanticVersion(1, 1, 1) };
            IModIdentity i4 = new ModinfoData { Name = "A", Version = new SemanticVersion(1, 1, 1) };

            Assert.Equal(i3, i4);
            Assert.NotEqual(i3, i1);

            IModIdentity i5 = new ModinfoData { Name = "B" };
            Assert.NotEqual(i1, i5);

            var d1 = new ModReference { Type = ModType.Default, Identifier = "A" };
            var d2 = new ModReference { Type = ModType.Default, Identifier = "A" };
            var d3 = new ModReference { Type = ModType.Default, Identifier = "B" };

            Assert.Equal(d1, d2);

            IModIdentity i6 = new ModinfoData { Name = "A", Dependencies = new List<IModReference>(new[] { d1, d3 }) };
            IModIdentity i7 = new ModinfoData { Name = "A", Dependencies = new List<IModReference>(new[] { d2, d3 }) };
            IModIdentity i8 = new ModinfoData { Name = "A", Dependencies = new List<IModReference>(new[] { d2 }) };
            IModIdentity i9 = new ModinfoData { Name = "A", Dependencies = new List<IModReference>(new[] { d3, d1 }) };
            IModIdentity i10 = new ModinfoData { Name = "A", Dependencies = new List<IModReference>(new[] { d1 }) };

            Assert.Equal(i6, i7);
            Assert.NotEqual(i6, i8);
            Assert.NotEqual(i6, i9);
            Assert.Equal(i8, i10);
        }


        [Fact]
        public void WriterTest()
        {
            var modinfo = new ModinfoData();
            modinfo.Name = "Test";
            modinfo.Version = new SemanticVersion(1,1,1, "BETA");
            var data = modinfo.ToJson(false);
            Assert.Contains(@"""version"": ""1.1.1-BETA""", data);
            Assert.Contains(@"""code"": ""en"",", data);
            Assert.DoesNotContain(@"""custom"":", data);
        }
    }
}