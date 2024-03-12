using System.Collections.Generic;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests;

public class LanguageInfoTests
{
    [Fact]
    public void Test_Equal()
    {
        ILanguageInfo a = new LanguageInfo {Code = "en", Support = LanguageSupportLevel.FullLocalized};
        ILanguageInfo b = new JsonLanguageInfo(new LanguageInfo { Code = "en", Support = LanguageSupportLevel.SFX });
        ILanguageInfo c = new LanguageInfo { Code = "de", Support = LanguageSupportLevel.FullLocalized};
        var d = LanguageInfo.Default;

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.Equal(a, d);
        Assert.Equal(LanguageSupportLevel.FullLocalized, d.Support);
    }

    public static IEnumerable<object[]> GetData()
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
    [MemberData(nameof(GetData))]
    public void Test_Parse(string data, string expectedCode, LanguageSupportLevel expectedLevel)
    {
        var languageInfo = LanguageInfo.Parse(data);
        Assert.Equal(expectedCode, languageInfo.Code);
        Assert.Equal(expectedLevel, languageInfo.Support);
        Assert.NotNull(languageInfo.Culture);
    }
}