using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Model.Json.Schema;
using EawModinfo.Spec;
using EawModinfo.Spec.Equality;
using Semver;
using Xunit;

namespace EawModinfo.Tests;

public class ModReferenceTests
{
    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ModReference(null!, ModType.Default));
        Assert.Throws<ArgumentException>(() => new ModReference("", ModType.Default));
        Assert.Throws<ArgumentNullException>(() => new ModReference(null!));
    }

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
    public void Parse_VersionRange(string data, SemVersionRange? range)
    {
        TestUtilities.Evaluate(data, EvaluationType.ModReference);
        var modReference = ModReference.Parse(data);
        Assert.Equal(range, modReference.VersionRange);
    }

    [Fact]
    public void Equals_GetHashCode()
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

#pragma warning disable xUnit2004
        Assert.Equal(false, a.Equals(d));
        Assert.Equal(false, d.Equals(a));
#pragma warning restore xUnit2004

        EqualityTestHelpers.AssertDefaultEquals(false, false, a, d);
        EqualityTestHelpers.AssertWithComparer(false, ModReferenceEqualityComparer.Default, a, d);

#pragma warning disable xUnit2004
        Assert.Equal(true, a.Equals(e));
        Assert.Equal(true, e.Equals(a));
#pragma warning restore xUnit2004

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

    public static IEnumerable<object[]> GetInvalidJsonData()
    {
        yield return
        [
            @"
{
    ""identifier"":""123123""
}",
            new[]{"required"}];

        yield return
        [
            @"
{
    ""modtype"":1
}",
            new[]{"required"}];

        yield return
        [
            @"
{
    ""modtype"":-1
}",
            new[]{"required", "minimum"}];

        yield return
        [
            @"
{
    ""modtype"":50
}",
            new[]{"required", "maximum"}];

        yield return
        [
            @"
{
    ""identifier"":"""",
    ""modtype"":1
}", 
            new[]{"minLength"}];
    }

    [Theory]
    [MemberData(nameof(GetInvalidJsonData))]
    public void Parse_Throws(string data, IList<string> expectedErrorKeys)
    {
        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModReference, out var errors));
        Assert.Equivalent(expectedErrorKeys, errors.Select(x => x.Key), true);

        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModReference));
        Assert.Throws<ModinfoParseException>(() => ModReference.Parse(data));
        Assert.Throws<ModinfoParseException>(() => SteamData.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data))));
    }

    public static IEnumerable<object[]> GetJsonData()
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
    }

    [Theory]
    [MemberData(nameof(GetJsonData))]
    public void Parse(string data, string? expectedId, ModType? expectedType)
    {
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModReference, out _));
        TestUtilities.Evaluate(data, EvaluationType.ModReference);
        var modReference = ModReference.Parse(data);
        Assert.Equal(expectedId, modReference.Identifier);
        Assert.Equal(expectedType, modReference.Type);
    }

    [Fact]
    public void Parse_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ModReference.Parse(null!));
    }

    [Fact]
    public void ToJson_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ModReference().ToJson(null!));
        ModReference modReference = default;
        Assert.Throws<ModinfoException>(() => modReference.ToJson());
        Assert.Throws<ModinfoException>(() => modReference.ToJson(new MemoryStream()));
    }

    [Fact]
    public static void ToJson()
    {
        var expected = @"{
  ""identifier"": ""name"",
  ""modtype"": 0
}";
        var modReference = new ModReference("name", ModType.Default);
        var data = modReference.ToJson();
        Assert.Equal(expected, data);

        var ms = new MemoryStream();
        modReference.ToJson(ms);
        Assert.Equal(expected, Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public static void ToJson_WithRange()
    {
        var expected = @"{
  ""identifier"": ""name"",
  ""modtype"": 0,
  ""version-range"": ""*""
}";
        var modReference = new ModReference("name", ModType.Default, SemVersionRange.Parse("*"));
        var data = modReference.ToJson();
        Assert.Equal(expected, data);

        var ms = new MemoryStream();
        modReference.ToJson(ms);
        Assert.Equal(expected, Encoding.UTF8.GetString(ms.ToArray()));
    }
}