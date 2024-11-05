using System;
using System.Collections.Generic;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using System.Text.Json.Nodes;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;
using EawModinfo.Spec.Equality;
using EawModinfo.Model.Json.Schema;

namespace EawModinfo.Tests;

public class DependencyListTest
{
    [Fact]
    public void ModDependencyList_Equal_GetHashCode()
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
    public void CtorThrows()
    {
        Assert.Throws<ArgumentNullException>(() => new DependencyList(null!, DependencyResolveLayout.FullResolved));
        Assert.Throws<ArgumentNullException>(() => new DependencyList(null!));
    }

    public static IEnumerable<object[]> GetJsonData()
    {
        yield return ["{}", null!, null!, true];
        yield return ["[]", null!, null!, true];
        yield return [@"[""FullResolved""]", null!, null!, true];
        yield return
        [
            @"[""FullResolved"", {""identifier"":""123"", ""modtype"":1}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops) },
            DependencyResolveLayout.FullResolved, false
        ];
        yield return
        [
            @"[""ResolveLastItem"", {""identifier"":""123"", ""modtype"":1}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops) },
            DependencyResolveLayout.ResolveLastItem, false
        ];
        yield return
        [
            @"[{""identifier"":""123"", ""modtype"":1}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops) },
            DependencyResolveLayout.ResolveRecursive, false
        ];
        yield return
        [
            @"[""NoValidResolve"", {""identifier"":""123"", ""modtype"":1}]", null!, null!, true
        ];
        yield return
        [
            @"[{""identifier"":""123"", ""modtype"":1}, {""identifier"":""321"", ""modtype"":0}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops), new ModReference("321", ModType.Default) },
            DependencyResolveLayout.ResolveRecursive, false
        ];
        yield return
        [
            @"[""FullResolved"", {""identifier"":""123"", ""modtype"":1}, {""identifier"":""321"", ""modtype"":0}]",
            new List<IModReference> { new ModReference("123", ModType.Workshops), new ModReference("321", ModType.Default) },
            DependencyResolveLayout.FullResolved, false
        ];
        yield return
        [
            @"[{""identifier"":""123"" }]", null!, null!, true
        ];
        yield return
        [
            @"[""FullResolved"", {""identifier"":""123"" }]", null!, null!, true
        ];
        yield return
        [
            @"[null, {""identifier"":""123"", ""modtype"":1 }]", null!, null!, true
        ];
    }

    [Theory]
    [MemberData(nameof(GetJsonData))]
    public void Parse(string data, IList<IModReference>? refs, DependencyResolveLayout? resolveLayout, bool throws)
    {
        if (throws)
        {
            Assert.Throws<ModinfoParseException>(() => ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModDependencyList));
            Assert.Throws<ModinfoParseException>(() => DependencyList.Parse(data));
        }
        else
        {
            ModInfoJsonSchema.Evaluate(JsonNode.Parse(data), EvaluationType.ModDependencyList);
            var depList = DependencyList.Parse(data);
            Assert.Equal(refs, depList);
            Assert.Equal(resolveLayout, depList.ResolveLayout);
        }
    }

    [Fact]
    public void ParseNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DependencyList.Parse(null!));
    }

    public static IEnumerable<object[]> GetListInstances()
    {
        yield return [DependencyList.EmptyDependencyList, string.Empty];
        yield return
        [
            new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default) },
                DependencyResolveLayout.ResolveRecursive),
            "[\r\n  {\r\n    \"identifier\": \"123\",\r\n    \"modtype\": 0\r\n  }\r\n]"
        ];
        yield return
        [
            new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default) },
                DependencyResolveLayout.FullResolved),
            "[\r\n  \"FullResolved\",\r\n  {\r\n    \"identifier\": \"123\",\r\n    \"modtype\": 0\r\n  }\r\n]"
        ];
        yield return
        [
            new DependencyList(new List<IModReference> { new ModReference("123", ModType.Default) },
                DependencyResolveLayout.FullResolved),
            "[\r\n  \"FullResolved\",\r\n  {\r\n    \"identifier\": \"123\",\r\n    \"modtype\": 0\r\n  }\r\n]"
        ];
    }

    [Theory]
    [MemberData(nameof(GetListInstances))]
    public static void Serialize(DependencyList list, string expected)
    {
        var json = JsonSerializer.Serialize(new JsonDependencyList(list), ParseUtility.SerializerOptions);
        Assert.Equal(expected, json);
    }
}