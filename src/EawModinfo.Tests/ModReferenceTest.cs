using System.Collections.Generic;
using System.Text.Json.Nodes;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
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
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModReference);
        var modReference = ModReference.Parse(data);
        Assert.Equal(range, modReference.VersionRange);
    }

    [Fact]
    public void Test_Equals_GetHashCode()
    {
        var a = new ModReference { Type = ModType.Workshops, Identifier = "123" };
        var b = new ModReference { Type = ModType.Workshops, Identifier = "123" };
        var c = new ModReference { Type = ModType.Default, Identifier = "123" };
        IModReference d = new JsonModReference { Type = ModType.Default, Identifier = "123" };
        IModReference d2 = new JsonModReference { Type = ModType.Default, Identifier = "123" };
        IModReference e = new JsonModReference { Type = ModType.Workshops, Identifier = "123" };
        var f = new ModReference { Type = ModType.Workshops, Identifier = "789" };

        EqualityTestHelpers.AssertDefaultEquals(false, false, a, (ModReference?)null);
        EqualityTestHelpers.AssertDefaultEquals<IModReference>(false, false, a, null);
        EqualityTestHelpers.AssertWithComparer(false, ModReferenceEqualityComparer.Default, a, null);

        EqualityTestHelpers.AssertDefaultEquals(true, true, a, a);
        EqualityTestHelpers.AssertDefaultEquals<IModReference>(true, true, a, a);
        EqualityTestHelpers.AssertWithComparer(true, ModReferenceEqualityComparer.Default, a, a);

        EqualityTestHelpers.AssertDefaultEquals(true, true, a, b);
        EqualityTestHelpers.AssertDefaultEquals<IModReference>(true, true, a, b);
        EqualityTestHelpers.AssertWithComparer(true, ModReferenceEqualityComparer.Default, a, b);

        EqualityTestHelpers.AssertDefaultEquals(false, false, a, c);
        EqualityTestHelpers.AssertDefaultEquals<IModReference>(false, false, a, c);
        EqualityTestHelpers.AssertWithComparer(false, ModReferenceEqualityComparer.Default, a, c);

        Assert.Equal(false, a.Equals(d));
        Assert.Equal(false, d.Equals(a));
        EqualityTestHelpers.AssertDefaultEquals(false, false, a, d);
        EqualityTestHelpers.AssertWithComparer(false, ModReferenceEqualityComparer.Default, a, d);

        Assert.Equal(true, a.Equals(e));
        Assert.Equal(true, e.Equals(a));
        EqualityTestHelpers.AssertDefaultEquals(false, true, a, e);
        EqualityTestHelpers.AssertWithComparer(true, ModReferenceEqualityComparer.Default, a, e);

        EqualityTestHelpers.AssertDefaultEquals(false, false, a, f);
        EqualityTestHelpers.AssertDefaultEquals<IModReference>(false, false, a, f);
        EqualityTestHelpers.AssertWithComparer(false, ModReferenceEqualityComparer.Default, a, f);

        EqualityTestHelpers.AssertDefaultEquals(true, true, d, d);
        EqualityTestHelpers.AssertWithComparer(true, ModReferenceEqualityComparer.Default, d, d);

        EqualityTestHelpers.AssertDefaultEquals(true, true, d, d2);
        EqualityTestHelpers.AssertWithComparer(true, ModReferenceEqualityComparer.Default, d, d2);

        EqualityTestHelpers.AssertDefaultEquals(false, false, d, e);
        EqualityTestHelpers.AssertWithComparer(false, ModReferenceEqualityComparer.Default, d, e);

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
            "123123", ModType.Workshops, false
        ];

        yield return
        [
            @"
{
    ""identifier"":""123123""
}",
            null!, null!, true
        ];

        yield return
        [
            @"
{
    ""modtype"":1
}",
            null!, null!, true
        ];

        yield return
        [
            @"
{
    ""modtype"":-1
}",
            null!, null!, true
        ];

        yield return
        [
            @"
{
    ""modtype"":50
}",
            null!, null!, true
        ];

        yield return
        [
            @"
{
    ""identifier"":"""",
    ""modtype"":1
}",
            null!, null!, true
        ];

    }

    [Theory]
    [MemberData(nameof(GetData))]
    public void Test_Parse(string data, string? expectedId, ModType? expectedType, bool throws)
    {
        if (throws)
        {
            Assert.Throws<ModinfoParseException>(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModReference));
            Assert.Throws<ModinfoParseException>(() => ModReference.Parse(data));
        }
        else
        {
            ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModReference);
            var modReference = ModReference.Parse(data);
            Assert.Equal(expectedId, modReference.Identifier);
            Assert.Equal(expectedType, modReference.Type);
        }
    }

    public static IEnumerable<object[]> GetReferences()
    {
        yield return [new ModReference("123", ModType.Default), "{\"identifier\":\"123\"}"];
    }
}