using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using Xunit;
using EawModinfo.Spec.Equality;
using EawModinfo.Model.Json.Schema;
using System.Text.Json.Nodes;
using System.Linq;

namespace EawModinfo.Tests;

public class DependencyListTest
{
    [Fact]
    public void Equal_GetHashCode()
    {
        EqualityTestHelpers.AssertWithComparer(true, ModDependencyListEqualityComparer.Default, null, null);
        EqualityTestHelpers.AssertWithComparer(false, ModDependencyListEqualityComparer.Default, EqualityTestHelpers.List, null);
        EqualityTestHelpers.AssertWithComparer(false, ModDependencyListEqualityComparer.Default, null, EqualityTestHelpers.List);

        EqualityTestHelpers.AssertWithComparer(true, ModDependencyListEqualityComparer.Default, EqualityTestHelpers.List, EqualityTestHelpers.List);
        EqualityTestHelpers.AssertWithComparer(true, ModDependencyListEqualityComparer.Default, EqualityTestHelpers.List, EqualityTestHelpers.SameishList);

        EqualityTestHelpers.AssertWithComparer(false, ModDependencyListEqualityComparer.Default, EqualityTestHelpers.List, EqualityTestHelpers.DifferentLayout);
        EqualityTestHelpers.AssertWithComparer(false, ModDependencyListEqualityComparer.Default, EqualityTestHelpers.List, EqualityTestHelpers.DifferentRef);
        EqualityTestHelpers.AssertWithComparer(false, ModDependencyListEqualityComparer.Default, EqualityTestHelpers.List, EqualityTestHelpers.AllDifferent);

        EqualityTestHelpers.AssertDefaultEquals(false, false, EqualityTestHelpers.List, null);

        EqualityTestHelpers.AssertDefaultEquals(false, EqualityTestHelpers.List, new JsonDependencyList(EqualityTestHelpers.List));
        EqualityTestHelpers.AssertDefaultEquals<IModDependencyList>(false, true, EqualityTestHelpers.List, new JsonDependencyList(EqualityTestHelpers.List));

        EqualityTestHelpers.AssertDefaultEquals(true, true, EqualityTestHelpers.List, EqualityTestHelpers.List);
        EqualityTestHelpers.AssertDefaultEquals<IModDependencyList>(true, true, EqualityTestHelpers.List, EqualityTestHelpers.List);

        EqualityTestHelpers.AssertDefaultEquals(true, true, EqualityTestHelpers.List, EqualityTestHelpers.List);
        EqualityTestHelpers.AssertDefaultEquals<IModDependencyList>(true, true, EqualityTestHelpers.List, EqualityTestHelpers.List);

        EqualityTestHelpers.AssertDefaultEquals(false, false, EqualityTestHelpers.List, EqualityTestHelpers.DifferentLayout);
        EqualityTestHelpers.AssertDefaultEquals<IModDependencyList>(false, false, EqualityTestHelpers.List, EqualityTestHelpers.DifferentLayout);
       
        EqualityTestHelpers.AssertDefaultEquals(false, false, EqualityTestHelpers.List, EqualityTestHelpers.DifferentRef);
        EqualityTestHelpers.AssertDefaultEquals<IModDependencyList>(false, false, EqualityTestHelpers.List, EqualityTestHelpers.DifferentRef);

        EqualityTestHelpers.AssertDefaultEquals(false, false, EqualityTestHelpers.List, EqualityTestHelpers.AllDifferent);
        EqualityTestHelpers.AssertDefaultEquals<IModDependencyList>(false, false, EqualityTestHelpers.List, EqualityTestHelpers.AllDifferent);

    }

    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new DependencyList(null!, DependencyResolveLayout.FullResolved));
        Assert.Throws<ArgumentNullException>(() => new DependencyList(null!));
    }


    public static IEnumerable<object[]> GetInvalidJsonData()
    {
        yield return ["{}", new[] { "oneOf", "type" }];
        yield return ["[]", new[] {"oneOf", "contains" }];
        yield return [@"[""FullResolved""]", new[] { "contains", "type" }];
        yield return
        [
            @"[""NoValidResolve"", {""identifier"":""123"", ""modtype"":1}]",new[]{ "oneOf", "enum", "type" }
        ];
        yield return
        [
            @"[{""identifier"":""123"" }]", new[] { "oneOf", "enum", "contains", "required" }
        ];
        yield return
        [
            @"[""FullResolved"", {""identifier"":""123"" }]", new[]{ "oneOf", "type", "contains", "required" }
        ];
        yield return
        [
            @"[null, {""identifier"":""123"", ""modtype"":1 }]", new[]{ "oneOf", "enum", "type" }
        ];
    }

    [Theory]
    [MemberData(nameof(GetInvalidJsonData))]
    public void Parse_Invalid(string data, IList<string> expectedErrorKeys)
    {
        Assert.False(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModDependencyList, out var errors));
        Assert.Equivalent(expectedErrorKeys, errors.Select(x => x.Key).Distinct(), true);
        Assert.Throws<ModinfoParseException>(() => TestUtilities.Evaluate(data, EvaluationType.ModDependencyList));
        Assert.Throws<ModinfoParseException>(() => DependencyList.Parse(data));
        Assert.Throws<ModinfoParseException>(() => DependencyList.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data))));
    }


    public static IEnumerable<object[]> GetJsonData()
    {
        yield return
        [
            @"[""FullResolved"", {""identifier"":""123"", ""modtype"":1}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops) },
            DependencyResolveLayout.FullResolved
        ];
        yield return
        [
            @"[""ResolveLastItem"", {""identifier"":""123"", ""modtype"":1}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops) },
            DependencyResolveLayout.ResolveLastItem
        ];
        yield return
        [
            @"[{""identifier"":""123"", ""modtype"":1}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops) },
            DependencyResolveLayout.ResolveRecursive
        ];
        yield return
        [
            @"[{""identifier"":""123"", ""modtype"":1}, {""identifier"":""321"", ""modtype"":0}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops), new ModReference("321", ModType.Default) },
            DependencyResolveLayout.ResolveRecursive
        ];
        yield return
        [
            @"[""FullResolved"", {""identifier"":""123"", ""modtype"":1}, {""identifier"":""321"", ""modtype"":0}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops), new ModReference("321", ModType.Default) },
            DependencyResolveLayout.FullResolved
        ];
    }

    [Theory]
    [MemberData(nameof(GetJsonData))]
    public void Parse(string data, IList<IModReference>? refs, DependencyResolveLayout? resolveLayout)
    {
        Assert.True(ModInfoJsonSchema.IsValid(JsonNode.Parse(data), EvaluationType.ModDependencyList, out _));
        TestUtilities.Evaluate(data, EvaluationType.ModDependencyList);
        var depList = DependencyList.Parse(data);
        Assert.Equal(refs, depList);
        Assert.Equal(resolveLayout, depList.ResolveLayout);

        depList = DependencyList.Parse(new MemoryStream(Encoding.UTF8.GetBytes(data)));
        Assert.Equal(refs, depList);
        Assert.Equal(resolveLayout, depList.ResolveLayout);
    }

    [Fact]
    public void Parse_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DependencyList.Parse((string)null!));
        Assert.Throws<ArgumentNullException>(() => DependencyList.Parse((Stream)null!));
    }

    public static IEnumerable<object[]> GetListInstances()
    {
        yield return [DependencyList.EmptyDependencyList, string.Empty];
        yield return
        [
            new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default) },
                DependencyResolveLayout.ResolveRecursive),
            @"[
  {
    ""identifier"": ""123"",
    ""modtype"": 0
  }
]"
        ];
        yield return
        [
            new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default) },
                DependencyResolveLayout.FullResolved),
            @"[
  ""FullResolved"",
  {
    ""identifier"": ""123"",
    ""modtype"": 0
  }
]"
        ];
        yield return
        [
            new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default) },
                DependencyResolveLayout.FullResolved),
            @"[
  ""FullResolved"",
  {
    ""identifier"": ""123"",
    ""modtype"": 0
  }
]"
        ];
    }

    [Theory]
    [MemberData(nameof(GetListInstances))]
    public static void ToJson(DependencyList list, string expected)
    {
        var json = list.ToJson();
        Assert.Equal(expected, json);

        var ms = new MemoryStream();
        list.ToJson(ms);
        Assert.Equal(expected, Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public static void ToJson_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DependencyList.EmptyDependencyList.ToJson(null!));
    }
}