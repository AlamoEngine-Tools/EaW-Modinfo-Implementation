using System.Collections.Generic;
using EawModinfo.Model;
using EawModinfo.Spec;
using Semver.Ranges;
using Xunit;

namespace EawModinfo.Tests;

public class ModReferenceTests
{
    public static IEnumerable<object[]> VersionRangeData()
    {
        yield return new object[]
        {
            @"
{
    ""identifier"":""123123"",
    ""modtype"":1
}",
            null
        };

        yield return new object[]
        {
            @"
{
    ""identifier"":""123123"",
    ""modtype"":1,
    ""version-range"": ""*""
}",
            SemVersionRange.Parse("*")
        };

        yield return new object[]
        {
            @"
{
    ""identifier"":""123123"",
    ""modtype"":1,
    ""version-range"": ""someInvalidRange""
}",
            null
        };
    }

    [Theory]
    [MemberData(nameof(VersionRangeData))]
    public void VersionRangeTest(string data, SemVersionRange? range)
    {
        var modReference = ModReference.Parse(data);
        Assert.Equal(range, modReference.VersionRange);
    }

    [Fact]
    public void EqualsCheck()
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
        yield return new object[]
        {
            @"
{
    ""identifier"":""123123"",
    ""modtype"":1
}",
            "123123", ModType.Workshops
        };

        yield return new object[]
        {
            @"
{
    ""identifier"":""123123"",
}",
            "123123", ModType.Workshops, true
        };

        yield return new object[]
        {
            @"
{
    ""modtype"":1,
}",
            "123123", ModType.Workshops, true
        };

        yield return new object[]
        {
            @"
{
    ""modtype"":-1,
}",
            "123123", ModType.Workshops, true
        };

        yield return new object[]
        {
            @"
{
    ""modtype"":50,
}",
            "123123", ModType.Workshops, true
        };

    }

    [Theory]
    [MemberData(nameof(GetData))]
    public void ParseTests(string data, string expectedCode, ModType expectedLevel, bool throws = false)
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