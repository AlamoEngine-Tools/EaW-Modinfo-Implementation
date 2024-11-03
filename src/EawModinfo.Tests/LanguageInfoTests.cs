using System.Collections.Generic;
using System.Text.Json.Nodes;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using Xunit;

namespace EawModinfo.Tests;

public class LanguageInfoTests
{
    [Fact]
    public void Test_Equal_GetHashCode()
    {
        var a = new LanguageInfo {Code = "en", Support = LanguageSupportLevel.FullLocalized};
        ILanguageInfo b = new JsonLanguageInfo(new LanguageInfo { Code = "en", Support = LanguageSupportLevel.SFX });
        var c = new LanguageInfo { Code = "de", Support = LanguageSupportLevel.FullLocalized};
        var d = LanguageInfo.Default;
        var f = new LanguageInfo { Code = "EN", Support = LanguageSupportLevel.FullLocalized };

        EqualityTestHelpers.AssertDefaultEquals(false, false, a, null);
        EqualityTestHelpers.AssertDefaultEquals<ILanguageInfo>(false, false, a, null);
        EqualityTestHelpers.AssertWithComparer(false, LanguageInfoEqualityComparer.Default, a, null);

        EqualityTestHelpers.AssertDefaultEquals(true, true, a, a);
        EqualityTestHelpers.AssertDefaultEquals<ILanguageInfo>(true, true, a, a);
        EqualityTestHelpers.AssertWithComparer(true, LanguageInfoEqualityComparer.Default, a, a);
        EqualityTestHelpers.AssertWithComparer(true, LanguageInfoEqualityComparer.WithSupportLevel, a, a);

        EqualityTestHelpers.AssertDefaultEquals(false, true, a, b);
        EqualityTestHelpers.AssertWithComparer(true, LanguageInfoEqualityComparer.Default, a, b);
        EqualityTestHelpers.AssertWithComparer(false, LanguageInfoEqualityComparer.WithSupportLevel, a, b);

        EqualityTestHelpers.AssertDefaultEquals(false, false, a, c);
        EqualityTestHelpers.AssertDefaultEquals<ILanguageInfo>(false, false, a, c);
        EqualityTestHelpers.AssertWithComparer(false, LanguageInfoEqualityComparer.Default, a, c);
        EqualityTestHelpers.AssertWithComparer(false, LanguageInfoEqualityComparer.WithSupportLevel, a, c);

        EqualityTestHelpers.AssertDefaultEquals(true, true, a, d);
        EqualityTestHelpers.AssertDefaultEquals<ILanguageInfo>(true, true, a, d);
        EqualityTestHelpers.AssertWithComparer(true, LanguageInfoEqualityComparer.Default, a, d);
        EqualityTestHelpers.AssertWithComparer(true, LanguageInfoEqualityComparer.WithSupportLevel, a, d);

        EqualityTestHelpers.AssertDefaultEquals(true, true, a, f);
        EqualityTestHelpers.AssertDefaultEquals<ILanguageInfo>(true, true, a, f);
        EqualityTestHelpers.AssertWithComparer(true, LanguageInfoEqualityComparer.Default, a, f);
        EqualityTestHelpers.AssertWithComparer(true, LanguageInfoEqualityComparer.WithSupportLevel, a, f);
    }

    public static IEnumerable<object[]> GetJsonData()
    {
        yield return
        [
            @"
{
    ""code"":""en""
}",
            "en", LanguageSupportLevel.FullLocalized
        ];
        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": 7
}",
            "en", LanguageSupportLevel.FullLocalized
        ];

        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": 1
}",
            "en", LanguageSupportLevel.Text
        ];

        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": 3
}",
            "en", LanguageSupportLevel.Text | LanguageSupportLevel.Speech
        ];
    }

    [Theory]
    [MemberData(nameof(GetJsonData))]
    public void Test_Parse(string data, string expectedCode, LanguageSupportLevel expectedLevel)
    {
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModLanguageInfo);
        var languageInfo = LanguageInfo.Parse(data);
        Assert.Equal(expectedCode, languageInfo.Code);
        Assert.Equal(expectedLevel, languageInfo.Support);
        Assert.NotNull(languageInfo.Culture);
    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        yield return
        [
            @"
{
    ""code"":""abc""
}"
        ];
        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": -1
}",
        ];

        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": 0
}",
        ];

        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": 8
}",
        ];

        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": 1,
    ""other"":""value""
}",
        ];
    }

    [Theory]
    [MemberData(nameof(GetInvalidData))]
    public void Parse_InvalidDataThrows(string data)
    {
        Assert.Throws<ModinfoParseException>(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModLanguageInfo));
        Assert.Throws<ModinfoParseException>(() => LanguageInfo.Parse(data));
    }

    [Fact]
    public void Test_ToJson_Languages_Default()
    {
        var languageInfo = LanguageInfo.Default;
        var json = languageInfo.ToJson();
        
        Assert.Contains(@"""code"": ""en""", json);
        Assert.Contains(@"""support"": 7", json);
    }

    [Fact]
    public void Test_ToJson_Languages_CustomDefault()
    {
        var languageInfo = new LanguageInfo("en", 0);
        var json = languageInfo.ToJson();
        Assert.Contains(@"""code"": ""en""", json);
        Assert.DoesNotContain(@"""support""", json);
    }

    [Fact]
    public void Test_ToJson_Languages_Full()
    {
        var languageInfo = new LanguageInfo("en", LanguageSupportLevel.FullLocalized);
        var json = languageInfo.ToJson();
        Assert.Contains(@"""code"": ""en""", json);
        Assert.Contains(@"""support"": 7", json);
    }

    [Fact]
    public void Test_ToJson_Languages_NonEnglish()
    {
        var languageInfo = new LanguageInfo("de", LanguageSupportLevel.FullLocalized);
        var json = languageInfo.ToJson();

        Assert.Contains(@"""code"": ""de""", json);
        Assert.Contains(@"""support"": 7", json);
    }
}