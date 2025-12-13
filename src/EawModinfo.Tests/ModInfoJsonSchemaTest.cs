using System;
using System.Text.Json;
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
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<IModReference>(JsonElement.Parse("{}")));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<IModinfo>(JsonElement.Parse("{}")));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<IModDependencyList>(JsonElement.Parse("{}")));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<ISteamData>(JsonElement.Parse("{}")));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<ILanguageInfo>(JsonElement.Parse("{}")));

        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<ModReference>(JsonElement.Parse("{}")));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<ModinfoData>(JsonElement.Parse("{}")));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<DependencyList>(JsonElement.Parse("{}")));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<SteamData>(JsonElement.Parse("{}")));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModInfoJsonSchema.Evaluate<LanguageInfo>(JsonElement.Parse("{}")));
    }
}