using System.Collections.Generic;
using System.Linq;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Steam;
using EawModinfo.Utilities;
using Semver;
using Xunit;

namespace EawModinfo.Tests;

public class ModinfoValidatorTests
{
    public static IEnumerable<object[]> GetModinfo()
    {
        yield return new object[]
        {
            new ModinfoData("ModName")
            {
                Version = new SemVersion(1, 1, 1, "BETA"), Icon = "path/icon.ico",
                Summary = "Who reads this?"
            }
        };
        yield return new object[] {new ModinfoData("ModName") };
        yield return new object[]
        {
            new ModinfoData("ModName")
            {
                Languages = new[]
                {
                    (ILanguageInfo) GetLanguageInfos().ElementAt(0)[0],
                    (ILanguageInfo) GetLanguageInfos().ElementAt(1)[0]
                }
            }
        };
        yield return new object[]
        {
            new ModinfoData("ModName")
            {
                SteamData = (ISteamData) GetSteamData().ElementAt(0)[0]
            }
        };
        yield return new object[]
        {
            new ModinfoData("ModName")
            {
                Dependencies = new DependencyList(GetModReferences().Select(x => x[0]).OfType<IModReference>().ToList(), DependencyResolveLayout.FullResolved)
            }
        };
    }
        
    public static IEnumerable<object[]> GetInvalidModinfo()
    {
        yield return new object[]
        {
            new ModinfoData("ModName")
            {
                SteamData = (ISteamData) GetInvalidSteamData().ElementAt(0)[0]
            }
        };
        yield return new object[]
        {
            new ModinfoData("ModName")
            {
                Languages = new []{ (ILanguageInfo) GetInvalidLanguageInfos().ElementAt(0)[0]}
            }
        };
        yield return new object[]
        {
            new ModinfoData("ModName")
            {
                Dependencies = new DependencyList(new []{ (IModReference) GetInvalidModReferences().ElementAt(0)[0]}, DependencyResolveLayout.FullResolved)
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetModinfo))]
    public void ModinfoValidate(IModinfo modinfo)
    {
        Assert.Null(Record.Exception(modinfo.Validate));
    }

    [Theory]
    [MemberData(nameof(GetInvalidModinfo))]
    public void ModinfoValidateInvalid(IModinfo modinfo)
    {
        Assert.Throws<ModinfoException>(modinfo.Validate);
    }


    public static IEnumerable<object[]> GetSteamData()
    {
        yield return new object[]
        {
            new JsonSteamData
            {
                Id = "1234312", Tags = new[] {"EAW"}, Metadata = "bla", ContentFolder = "testFolder",
                Visibility = SteamWorkshopVisibility.FriendsOnly, Title = "MyTitle"
            },
            false
        };
        yield return new object[]
        {
            new JsonSteamData
            {
                Id = "1234312", Tags = new[] {"EAW"}, Metadata = "bla", ContentFolder = "testFolder",
                Title = "MyTitle"
            },
            false
        };
        yield return new object[]
        {
            new JsonSteamData {Id = "1234312", Tags = new[] {"EAW"}, ContentFolder = "testFolder", Title = "MyTitle"},
            false
        };
        yield return new object[]
        {
            new JsonSteamData
            {
                Id = "1234312", Tags = new[] {"EAW"}, ContentFolder = "testFolder",
                Description = "Some description", Title = "MyTitle"
            },
            false
        };
    }

    public static IEnumerable<object[]> GetInvalidSteamData()
    {
        yield return new object[] {new JsonSteamData(), true};
        yield return new object[]
            {new JsonSteamData {Id = "asd", Tags = new[] {"EAW"}, ContentFolder = "testFolder"}, true};
        yield return new object[]
            {new JsonSteamData {Id = "0", Tags = new[] {"EAW"}, ContentFolder = "testFolder"}, true};
        yield return new object[]
            {new JsonSteamData {Id = "-123", Tags = new[] {"EAW"}, ContentFolder = "testFolder"}, true};
        yield return new object[]
        {
            new JsonSteamData {Id = "129381209812430981329048", Tags = new[] {"EAW"}, ContentFolder = "testFolder"},
            true
        };
        yield return new object[]
            {new JsonSteamData {Id = "1234", Tags = new string[0], ContentFolder = "testFolder"}, true};
        yield return new object[] {new JsonSteamData {Id = "1234", Tags = null!, ContentFolder = "testFolder"}, true};
        yield return new object[] {new JsonSteamData {Id = "1234", Tags = new[] { "EAW" }, ContentFolder = ""}, true};
        yield return new object[] {new JsonSteamData {Id = "1234", Tags = new[] { "EAW" }, ContentFolder = null!}, true};
        yield return new object[] {new JsonSteamData {Id = "1234312", Tags = new[] {"test"}, Metadata = "bla", ContentFolder = "testFolder"}, true};
    }

    [Theory]
    [MemberData(nameof(GetSteamData))]
    [MemberData(nameof(GetInvalidSteamData))]
    public void SteamDataValidate(ISteamData steamData, bool shallThrow)
    {
        if (!shallThrow)
            Assert.Null(Record.Exception(steamData.Validate));
        else
            Assert.Throws<ModinfoException>(steamData.Validate);
    }

    public static IEnumerable<object[]> GetModReferences()
    {
        yield return new object[] {new ModReference { Identifier = "en", Type = ModType.Default}, false};
        yield return new object[] {new ModReference { Identifier = "en/ba/fr", Type = ModType.Default}, false};
        yield return new object[] {new ModReference { Identifier = "en\\ba\\fr", Type = ModType.Default}, false};
        yield return new object[] {new ModReference { Identifier = "c:\\en\\ba\\fr", Type = ModType.Default}, false};
        yield return new object[] {new ModReference { Identifier = "../en/../../ba", Type = ModType.Default}, false};
        yield return new object[] {new ModReference { Identifier = "123456", Type = ModType.Default}, false};
        yield return new object[] {new ModReference { Identifier = "123456", Type = ModType.Workshops}, false};
        yield return new object[] {new ModReference { Identifier = "blabla", Type = ModType.Virtual}, false};
    }

    public static IEnumerable<object[]> GetInvalidModReferences()
    {
        yield return new object[] {new ModReference(), true};
        yield return new object[]
        {
            new ModReference
            {
                Identifier = null!, Type = ModType.Default
            },
            true
        };
        yield return new object[] {new ModReference { Identifier = null!, Type = ModType.Workshops}, true};
        yield return new object[] {new ModReference { Identifier = null!, Type = ModType.Virtual}, true};
        yield return new object[] {new ModReference { Identifier = string.Empty, Type = ModType.Default}, true};
        yield return new object[] {new ModReference { Identifier = string.Empty, Type = ModType.Workshops}, true};
        yield return new object[] {new ModReference { Identifier = string.Empty, Type = ModType.Virtual}, true};
        yield return new object[] {new ModReference { Identifier = "-1", Type = ModType.Workshops}, true};
        yield return new object[] {new ModReference { Identifier = "0", Type = ModType.Workshops}, true};
        yield return new object[]
            {new ModReference {Identifier = "12921908098098098309481234", Type = ModType.Workshops}, true};
    }



    [Theory]
    [MemberData(nameof(GetModReferences))]
    [MemberData(nameof(GetInvalidModReferences))]
    public void ModReferenceValidate(IModReference modReference, bool shallThrow)
    {
        if (!shallThrow)
            Assert.Null(Record.Exception(modReference.Validate));
        else
            Assert.Throws<ModinfoException>(modReference.Validate);
    }


    public static IEnumerable<object[]> GetLanguageInfos()
    {
        yield return new object[] {new LanguageInfo {Code = "en"}, false};
        yield return new object[] {new LanguageInfo { Code = "de"}, false};
        yield return new object[] {new LanguageInfo { Code = "es"}, false};
    }


    public static IEnumerable<object[]> GetInvalidLanguageInfos()
    {
        yield return new object[] {new LanguageInfo(), true};
        yield return new object[]
        {
            new LanguageInfo
            {
                Code = string.Empty
            },
            true
        };
        yield return new object[] {new LanguageInfo { Code = "ens"}, true};
        yield return new object[] {new LanguageInfo { Code = "de-de"}, true};
        yield return new object[] {new LanguageInfo { Code = "deu"}, true};
        yield return new object[] {new LanguageInfo { Code = "iv"}, true};
    }




    [Theory]
    [MemberData(nameof(GetLanguageInfos))]
    [MemberData(nameof(GetInvalidLanguageInfos))]
    public void LanguageValidate(ILanguageInfo info, bool shallThrow)
    {
        if (!shallThrow)
            Assert.Null(Record.Exception(info.Validate));
        else
            Assert.Throws<ModinfoException>(info.Validate);
    }
}