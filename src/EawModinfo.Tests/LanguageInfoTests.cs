using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Model.Json.Schema;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using Xunit;

namespace EawModinfo.Tests;

public class LanguageInfoTests
{
    [Fact]
    public void Equal_GetHashCode()
    {
        var a = new LanguageInfo("en", LanguageSupportLevel.FullLocalized);
        ILanguageInfo b = new JsonLanguageInfo(new LanguageInfo("en", LanguageSupportLevel.SFX));
        var c = new LanguageInfo("de", LanguageSupportLevel.FullLocalized);
        var d = LanguageInfo.Default;
        var f = new LanguageInfo("EN", LanguageSupportLevel.FullLocalized);

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

    [Fact]
    public void Ctor()
    {
        var info = new LanguageInfo("en", LanguageSupportLevel.FullLocalized);
        Assert.Equal("en", info.Code);
        Assert.Equal(LanguageSupportLevel.FullLocalized, info.Support);
    }

    [Fact]
    public void Ctor_DefaultSupportCoercesToFullLocalized()
    {
        var info = new LanguageInfo("de", default);
        Assert.Equal("de", info.Code);
        Assert.Equal(LanguageSupportLevel.FullLocalized, info.Support);

        var info2 = new LanguageInfo(new JsonLanguageInfo("en", default));
        Assert.Equal("en", info2.Code);
        Assert.Equal(LanguageSupportLevel.FullLocalized, info2.Support);
    }

    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LanguageInfo(null!, LanguageSupportLevel.Default));
        Assert.Throws<ArgumentNullException>(() => new LanguageInfo(null!));
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
    public void Parse(string data, string expectedCode, LanguageSupportLevel expectedLevel)
    {
        TestUtilities.Evaluate(data, EvaluationType.ModLanguageInfo);
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModLanguageInfo, out _));
        var languageInfo = LanguageInfo.Parse(data);
        Assert.Equal(expectedCode, languageInfo.Code);
        Assert.Equal(expectedLevel, languageInfo.Support);

        var ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
        languageInfo = LanguageInfo.Parse(ms);
        Assert.Equal(expectedCode, languageInfo.Code);
        Assert.Equal(expectedLevel, languageInfo.Support);
    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        yield return
        [
            @"
{
    ""code"":""abc""
}", new[]{ "maxLength" }
        ];
        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": -1
}", new[]{ "minimum" }
        ];

        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": 0
}", new[]{"minimum"}
        ];

        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": 8
}", new[]{"maximum"}
        ];

        yield return
        [
            @"
{
    ""code"":""en"",
    ""support"": 1,
    ""other"":""value""
}", new[]{""}
        ];
    }

    [Theory]
    [MemberData(nameof(GetInvalidData))]
    public void Parse_InvalidDataThrows(string data, IList<string> expectedErrorKeys)
    {
        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModLanguageInfo));
        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModLanguageInfo, out var errors));
        Assert.Equivalent(expectedErrorKeys, errors.Select(x => x.Key), true); 
        Assert.Throws<ModinfoParseException>(() => LanguageInfo.Parse(data));
        Assert.Throws<ModinfoParseException>(() => LanguageInfo.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data))));
    }

    [Fact]
    public void Parse_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => LanguageInfo.Parse((string)null!));
        Assert.Throws<ArgumentNullException>(() => LanguageInfo.Parse((Stream)null!));
    }

    [Fact]
    public void ToJson_Languages_Default()
    {
        var languageInfo = LanguageInfo.Default;
        var json = languageInfo.ToJson();
        Assert.Contains(@"""code"": ""en""", json);
        Assert.DoesNotContain(@"""support"": 7", json);

        var ms = new MemoryStream();
        languageInfo.ToJson(ms);
        json = Encoding.UTF8.GetString(ms.ToArray());
        Assert.Contains(@"""code"": ""en""", json);
        Assert.DoesNotContain(@"""support"": 7", json);
    }

    [Fact]
    public void ToJson_Languages_CustomDefault()
    {
        var languageInfo = new LanguageInfo("en", 0);
        var json = languageInfo.ToJson();
        Assert.Contains(@"""code"": ""en""", json);
        Assert.DoesNotContain(@"""support""", json);

        var ms = new MemoryStream();
        languageInfo.ToJson(ms);
        json = Encoding.UTF8.GetString(ms.ToArray());
        Assert.Contains(@"""code"": ""en""", json);
        Assert.DoesNotContain(@"""support""", json);
    }

    [Fact]
    public void ToJson_Languages_Full()
    {
        var languageInfo = new LanguageInfo("en", LanguageSupportLevel.FullLocalized);
        var json = languageInfo.ToJson();
        Assert.Contains(@"""code"": ""en""", json);
        Assert.DoesNotContain(@"""support"": 7", json);

        var ms = new MemoryStream();
        languageInfo.ToJson(ms);
        json = Encoding.UTF8.GetString(ms.ToArray());
        Assert.Contains(@"""code"": ""en""", json);
        Assert.DoesNotContain(@"""support""", json);
    }

    [Fact]
    public void ToJson_Languages_NonEnglish()
    {
        var languageInfo = new LanguageInfo("de", LanguageSupportLevel.FullLocalized);
        var json = languageInfo.ToJson();

        Assert.Contains(@"""code"": ""de""", json);
        Assert.DoesNotContain(@"""support"": 7", json);

        var ms = new MemoryStream();
        languageInfo.ToJson(ms);
        json = Encoding.UTF8.GetString(ms.ToArray());
        Assert.Contains(@"""code"": ""de""", json);
        Assert.DoesNotContain(@"""support"": 7", json);
    }

    [Fact]
    public void ToJson_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LanguageInfo("en", LanguageSupportLevel.FullLocalized).ToJson(null!));
    }

    [Fact]
    public void GetCulture_Valid()
    {
        var info = new LanguageInfo("de", LanguageSupportLevel.Text);
        Assert.NotNull(info.GetCulture());
        Assert.NotNull(info.GetCulture());
    }

    [Fact]
    public void GetCulture_Invalid()
    {
        var info = new LanguageInfo("d3", LanguageSupportLevel.Text);
        Assert.Throws<CultureNotFoundException>(() => info.GetCulture());
        Assert.Throws<CultureNotFoundException>(() => info.GetCulture());
    }
}