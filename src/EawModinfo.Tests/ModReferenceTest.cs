using System.Collections.Generic;
using EawModinfo.Model;
using EawModinfo.Spec;
using Semver;
using Xunit;

namespace EawModinfo.Tests;

public class ModReferenceTests
{
    public static IEnumerable<object[]> VersionRangeData()
    {
        yield return
        [
            @"
{
    ""identifier"":""123123"",
    ""modtype"":1
}",
            null!
        ];

        yield return
        [
            @"
{
    ""identifier"":""123123"",
    ""modtype"":1,
    ""version-range"": ""*""
}",
            SemVersionRange.Parse("*")
        ];

        yield return
        [
            @"
{
    ""identifier"":""123123"",
    ""modtype"":1,
    ""version-range"": ""someInvalidRange""
}",
            null!
        ];
    }

    [Theory]
    [MemberData(nameof(VersionRangeData))]
    public void Test_Parse_VersionRange(string data, SemVersionRange? range)
    {
        var modReference = ModReference.Parse(data);
        Assert.Equal(range, modReference.VersionRange);
    }

    [Fact]
    public void Test_Equals()
    {
        IModReference a = new ModReference { Type = ModType.Workshops, Identifier = "123213" };
        IModReference b = new ModReference { Type = ModType.Workshops, Identifier = "123213" };
        IModReference c = new ModReference { Type = ModType.Default, Identifier = "123213" };
        IModReference d = new ModReference { Type = ModType.Default, Identifier = "123213" };

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.Equal(c, d);
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return
        [
            @"
{
    ""identifier"":""123123"",
    ""modtype"":1
}",
            "123123", ModType.Workshops
        ];

        yield return
        [
            @"
{
    ""identifier"":""123123"",
}",
            "123123", ModType.Workshops, true
        ];

        yield return
        [
            @"
{
    ""modtype"":1,
}",
            "123123", ModType.Workshops, true
        ];

        yield return
        [
            @"
{
    ""modtype"":-1,
}",
            "123123", ModType.Workshops, true
        ];

        yield return
        [
            @"
{
    ""modtype"":50,
}",
            "123123", ModType.Workshops, true
        ];

    }

    [Theory]
    [MemberData(nameof(GetData))]
    public void Test_Parse(string data, string expectedCode, ModType expectedLevel, bool throws = false)
    {
        if (throws)
            Assert.Throws<ModinfoParseException>(() => ModReference.Parse(data));
        else
        {
            var modReference = ModReference.Parse(data);
            Assert.Equal(expectedCode, modReference.Identifier);
            Assert.Equal(expectedLevel, modReference.Type);
        }
    }
}