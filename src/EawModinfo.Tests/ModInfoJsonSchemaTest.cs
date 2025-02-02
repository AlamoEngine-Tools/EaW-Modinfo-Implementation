using System;
using AET.Modinfo.Model;
using AET.Modinfo.Model.Json.Schema;
using AET.Modinfo.Spec;
using AET.Modinfo.Spec.Steam;
using Xunit;

namespace AET.Modinfo.Tests;

public class ModInfoJsonSchemaTest
{
    [Fact]
    public void RunTests()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<IModReference>("{}"));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<IModinfo>("{}"));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<IModDependencyList>("{}"));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<ISteamData>("{}"));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<ILanguageInfo>("{}"));

        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<ModReference>("{}"));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<ModinfoData>("{}"));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<DependencyList>("{}"));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<SteamData>("{}"));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<LanguageInfo>("{}"));
    }
}