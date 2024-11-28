using System;
using System.Collections.Generic;
using System.IO;
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
        yield return
        [
            new ModinfoData("ModName")
            {
                Version = SemVersion.ParsedFrom(1, 1, 1, "BETA"), Icon = "path/icon.ico",
                Summary = "Who reads this?"
            }
        ];
        yield return [new ModinfoData("ModName")];
        yield return
        [
            new ModinfoData("ModName")
            {
                Languages =
                [
                    (ILanguageInfo) GetLanguageInfos().ElementAt(0)[0],
                    (ILanguageInfo) GetLanguageInfos().ElementAt(1)[0]
                ]
            }
        ];
        yield return
        [
            new ModinfoData("ModName")
            {
                SteamData = (ISteamData) GetSteamData().ElementAt(0)[0]
            }
        ];
        yield return
        [
            new ModinfoData("ModName")
            {
                Dependencies = new DependencyList(GetModReferences().Select(x => x[0]).OfType<IModReference>().ToList(), DependencyResolveLayout.FullResolved)
            }
        ];
    }

    public static IEnumerable<object[]> GetInvalidModinfo()
    {
        yield return [new JsonModinfoData{Name = string.Empty}];
        yield return [new JsonModinfoData{Name = null!}];
        yield return
        [
            new ModinfoData("ModName")
            {
                SteamData = (ISteamData) GetInvalidSteamData().ElementAt(0)[0]
            }
        ];
        yield return
        [
            new ModinfoData("ModName")
            {
                Languages = [(ILanguageInfo) GetInvalidLanguageInfos().ElementAt(0)[0]]
            }
        ];
        yield return
        [
            new ModinfoData("ModName")
            {
                Dependencies = new DependencyList([(IModReference) GetInvalidModReferences().ElementAt(0)[0]], DependencyResolveLayout.FullResolved)
            }
        ];
        yield return
        [
            new CustomModinfo(null!)
        ];
        yield return
        [
            new CustomModinfo("")
        ];

        yield return
        [
            new CustomModinfo("name")
            {
                Languages = new List<ILanguageInfo>{LanguageInfo.Default},
                Custom = new Dictionary<string, object>(),
                Dependencies = null!
            }
        ];
        yield return
        [
            new CustomModinfo("name")
            {
                Custom = new Dictionary<string, object>(),
                Dependencies = new DependencyList([], DependencyResolveLayout.FullResolved),
                Languages = null!
            }
        ];
        yield return
        [
            new CustomModinfo("name")
            {
                Languages = new List<ILanguageInfo>{LanguageInfo.Default},
                Dependencies = new DependencyList([], DependencyResolveLayout.FullResolved),
                Custom = null!
            }
        ];
    }

    [Theory]
    [MemberData(nameof(GetModinfo))]
    public void Validate(IModinfo modinfo)
    {
        Assert.Null(Record.Exception(modinfo.Validate));
    }

    [Theory]
    [MemberData(nameof(GetInvalidModinfo))]
    public void Validate_ThrowsModinfoException(IModinfo modinfo)
    {
        Assert.Throws<ModinfoException>(modinfo.Validate);
    }


    public static IEnumerable<object[]> GetSteamData()
    {
        yield return
        [
            new JsonSteamData
            {
                Id = "1234312", Tags = ["EAW"], Metadata = "bla", ContentFolder = "testFolder",
                Visibility = SteamWorkshopVisibility.FriendsOnly, Title = "MyTitle"
            },
            false
        ];
        yield return
        [
            new JsonSteamData
            {
                Id = "1234312", Tags = ["EAW"], Metadata = "bla", ContentFolder = "testFolder",
                Title = "MyTitle"
            },
            false
        ];
        yield return
        [
            new JsonSteamData {Id = "1234312", Tags = ["EAW"], ContentFolder = "testFolder", Title = "MyTitle"},
            false
        ];
        yield return
        [
            new JsonSteamData
            {
                Id = "1234312", Tags = ["EAW"], ContentFolder = "testFolder",
                Description = "Some description", Title = "MyTitle"
            },
            false
        ];
    }

    public static IEnumerable<object[]> GetInvalidSteamData()
    {
        yield return [new JsonSteamData(), true];
        yield return [new JsonSteamData { Id = null! }, true];
        yield return [new JsonSteamData {Id = "asd", Tags = ["EAW"], ContentFolder = "testFolder"}, true]; 
        yield return [new JsonSteamData {Id = "1234", Tags = ["EAW"], ContentFolder = "testFolder"}, true];
        yield return [new JsonSteamData {Id = "0", Tags = ["EAW"], ContentFolder = "testFolder", Title = "Title"}, true];
        yield return [new JsonSteamData {Id = "-123", Tags = ["EAW"], ContentFolder = "testFolder", Title = "Title" }, true];
        yield return
        [
            new JsonSteamData {Id = "129381209812430981329048", Tags = ["EAW"], ContentFolder = "testFolder", Title = "Title"},
            true
        ];
        yield return [new JsonSteamData {Id = "1234", Tags = Array.Empty<string>(), ContentFolder = "testFolder", Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234", Tags = null!, ContentFolder = "testFolder", Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234", Tags = null!, ContentFolder = "testFolder", Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234", Tags = ["EAW"], ContentFolder = "", Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234", Tags = ["EAW"], ContentFolder = null!, Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234312", Tags = ["eaw"], Metadata = "bla", ContentFolder = "testFolder", Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234312", Tags = ["test"], Metadata = "bla", ContentFolder = "testFolder", Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234312", Tags = ["FOC", "a,c"], Metadata = "bla", ContentFolder = "testFolder", Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234312", Tags = ["FOC", "a\tc"], Metadata = "bla", ContentFolder = "testFolder", Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234312", Tags = ["FOC", "FOC"], Metadata = "bla", ContentFolder = "testFolder", Title = "Title" }, true];
        yield return [new JsonSteamData { Id = "1234312", Tags = ["FOC", new string('a', 256)], Metadata = "bla", ContentFolder = "testFolder", Title = "Title" }, true];
    }

    [Theory]
    [MemberData(nameof(GetSteamData))]
    [MemberData(nameof(GetInvalidSteamData))]
    public void Validate_SteamData(ISteamData steamData, bool shallThrow)
    {
        if (!shallThrow)
            Assert.Null(Record.Exception(steamData.Validate));
        else
            Assert.Throws<ModinfoException>(steamData.Validate);
    }

    public static IEnumerable<object[]> GetModReferences()
    {
        yield return [new ModReference { Identifier = "en", Type = ModType.Default }, false];
        yield return [new ModReference { Identifier = "en/ba/fr", Type = ModType.Default }, false];
        yield return [new ModReference { Identifier = "en\\ba\\fr", Type = ModType.Default }, false];
        yield return [new ModReference { Identifier = "c:\\en\\ba\\fr", Type = ModType.Default }, false];
        yield return [new ModReference { Identifier = "../en/../../ba", Type = ModType.Default }, false];
        yield return [new ModReference { Identifier = "123456", Type = ModType.Default }, false];
        yield return [new ModReference { Identifier = "123456", Type = ModType.Workshops }, false];
        yield return [new ModReference { Identifier = "blabla", Type = ModType.Virtual }, false];
    }

    public static IEnumerable<object[]> GetInvalidModReferences()
    {
        yield return [new ModReference(), true];
        yield return
        [
            new ModReference
            {
                Identifier = null!, Type = ModType.Default
            },
            true
        ];
        yield return [new ModReference { Identifier = null!, Type = ModType.Workshops }, true];
        yield return [new ModReference { Identifier = null!, Type = ModType.Virtual }, true];
        yield return [new ModReference { Identifier = string.Empty, Type = ModType.Default }, true];
        yield return [new ModReference { Identifier = string.Empty, Type = ModType.Workshops }, true];
        yield return [new ModReference { Identifier = string.Empty, Type = ModType.Virtual }, true];
        yield return [new ModReference { Identifier = "-1", Type = ModType.Workshops }, true];
        yield return [new ModReference { Identifier = "0", Type = ModType.Workshops }, true];
        yield return [new ModReference { Identifier = "12921908098098098309481234", Type = ModType.Workshops }, true];
        yield return [new ModReference { Identifier = "1234", Type = (ModType) 0xff }, true];
    }



    [Theory]
    [MemberData(nameof(GetModReferences))]
    [MemberData(nameof(GetInvalidModReferences))]
    public void Validate_ModReference(IModReference modReference, bool shallThrow)
    {
        if (!shallThrow)
            Assert.Null(Record.Exception(modReference.Validate));
        else
            Assert.Throws<ModinfoException>(modReference.Validate);
    }


    public static IEnumerable<object[]> GetLanguageInfos()
    {
        yield return [new LanguageInfo("en", LanguageSupportLevel.FullLocalized), false];
        yield return [new LanguageInfo("de", LanguageSupportLevel.FullLocalized), false];
        yield return [new LanguageInfo("es", LanguageSupportLevel.FullLocalized), false];
    }


    public static IEnumerable<object[]> GetInvalidLanguageInfos()
    {
        yield return [new JsonLanguageInfo(null!, LanguageSupportLevel.FullLocalized), true];
        yield return
        [
            new JsonLanguageInfo("", LanguageSupportLevel.FullLocalized),
            true
        ];
        yield return [new LanguageInfo("ens", LanguageSupportLevel.FullLocalized), true];
        yield return [new LanguageInfo("de-de", LanguageSupportLevel.FullLocalized), true];
        yield return [new LanguageInfo("deu", LanguageSupportLevel.FullLocalized), true];
        yield return [new LanguageInfo("iv", LanguageSupportLevel.FullLocalized), true];
        yield return [new LanguageInfo("..", LanguageSupportLevel.FullLocalized), true];
    }

    [Theory]
    [MemberData(nameof(GetLanguageInfos))]
    [MemberData(nameof(GetInvalidLanguageInfos))]
    public void Validate_Language(ILanguageInfo info, bool shallThrow)
    {
        if (!shallThrow)
            Assert.Null(Record.Exception(info.Validate));
        else
            Assert.Throws<ModinfoException>(info.Validate);
    }

    private class CustomModinfo(string name) : IModinfo
    {
        public string Name => name;
        public SemVersion? Version { get; }
        public IModDependencyList Dependencies { get; set; }

        public string? Summary { get; }
        public string? Icon { get; }
        public IDictionary<string, object> Custom { get; set; }
        public ISteamData? SteamData { get; }
        public IReadOnlyCollection<ILanguageInfo> Languages { get; set; }
        public bool LanguagesExplicitlySet { get; }

        public string ToJson()
        {
            throw new NotImplementedException();
        }

        public void ToJson(Stream stream)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IModIdentity? other)
        {
            throw new NotImplementedException();
        }
    }
}