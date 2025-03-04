using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AET.Modinfo.File;
using AET.Modinfo.Model;
using AET.Modinfo.Spec;
using Semver;
using Xunit;

namespace AET.Modinfo.Tests;

public class VariantModinfoFileTest : ModinfoFileTestsBase
{
    protected override ModinfoFileKind FileKind => ModinfoFileKind.VariantFile;

    protected override string GetFileName()
    {
        return "variant-modinfo.json";
    }

    protected override IModinfoFile CreateFile(string path, bool isInvalidFileContent = false)
    {
        var content = isInvalidFileContent == false ? TestUtilities.VariantModifnoData : "{}";
        return CreateVariantWithBaseFile(path, null, content);
    }

    private ModinfoVariantFile CreateVariantWithBaseFile(string path, MainModinfoFile? baseFile, string content)
    {
        var fileInfo = FileSystem.FileInfo.New(path);
        fileInfo.Directory!.Create();
        FileSystem.File.WriteAllText(path, content);
        fileInfo.Refresh();
        return new ModinfoVariantFile(fileInfo, baseFile);
    }

    private ModinfoVariantFile CreateVariantWithBaseInfo(string path, IModinfo? baseInfo)
    {
        var fileInfo = FileSystem.FileInfo.New(path);
        fileInfo.Directory!.Create();
        FileSystem.File.WriteAllText(path, TestUtilities.VariantModifnoData);
        fileInfo.Refresh();
        return new ModinfoVariantFile(fileInfo, baseInfo);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ModinfoVariantFile(null!));
        Assert.Throws<ArgumentNullException>(() => new ModinfoVariantFile(null!, (MainModinfoFile?)null));
        Assert.Throws<ArgumentNullException>(() => new ModinfoVariantFile(null!, (IModinfo?)null));
    }

    [Fact]
    public async Task GetModinfoAsync_MergeDataFromMainFileIntoVariant()
    {
        var variant = CreateVariantWithMainFile();
        await AssertMergeDataFromMainIntoVariant(variant, async file => await file.GetModinfoAsync());
    }

    [Fact]
    public async Task GetModinfo_MergeDataFromMainFileIntoVariant()
    {
        var variant = CreateVariantWithMainFile();
        await AssertMergeDataFromMainIntoVariant(variant, async file => await Task.Run(file.GetModinfo));
    }

    [Fact]
    public async Task TryGetModinfo_MergeDataFromMainFileIntoVariant()
    {
        var variant = CreateVariantWithMainFile();
        await AssertMergeDataFromMainIntoVariant(variant, async file => await Task.Run(() =>
        {
            Assert.True(file.TryGetModinfo(out var data));
            return data;
        }));
    }

    private ModinfoVariantFile CreateVariantWithMainFile(bool isInvalidFileContent = false)
    {
        var mainFileInfo = TestUtilities.CreateModinfoFile(FileSystem, "mods/A");
        var mainFile = new MainModinfoFile(mainFileInfo);

        var content = isInvalidFileContent == false ? TestUtilities.VariantModifnoData : "{}";
        return CreateVariantWithBaseFile(
            FileSystem.Path.Combine("mods", "myMod", GetFileName()),
            mainFile,
            content);
    }

    [Fact]
    public async Task GetModinfoAsync_MergeDataFromMainInfoIntoVariant()
    {
        var variant = CreateVariantWithMainInfo();
        await AssertMergeDataFromMainIntoVariant(variant, async file => await file.GetModinfoAsync());
    }

    [Fact]
    public async Task GetModinfo_MergeDataFromMainInfoIntoVariant()
    {
        var variant = CreateVariantWithMainInfo();
        await AssertMergeDataFromMainIntoVariant(variant, async file => await Task.Run(file.GetModinfo));
    }

    [Fact]
    public async Task TryGetModinfo_MergeDataFromMainInfoIntoVariant()
    {
        var variant = CreateVariantWithMainInfo();
        await AssertMergeDataFromMainIntoVariant(variant, async file => await Task.Run(() =>
        {
            Assert.True(file.TryGetModinfo(out var data));
            return data;
        }));
    }

    private ModinfoVariantFile CreateVariantWithMainInfo()
    {
        var main = new ModinfoData("Main")
        {
            Version = SemVersion.Parse("1.1.1-BETA"),
            Dependencies = new DependencyList([new ModReference { Identifier = "123", Type = ModType.Workshops }],
                DependencyResolveLayout.FullResolved),
            Custom = new Dictionary<string, object>{{"key", "value"}},
            Languages = [new LanguageInfo("en", LanguageSupportLevel.FullLocalized), new LanguageInfo("de", LanguageSupportLevel.SFX)]
        };

        return CreateVariantWithBaseInfo(FileSystem.Path.Combine("mods", "myMod", GetFileName()), main);
    }

    private async Task AssertMergeDataFromMainIntoVariant(ModinfoVariantFile variant, Func<ModinfoVariantFile, Task<IModinfo>> getDataFunc)
    {
        Assert.Equal(ModinfoFileKind.VariantFile, variant.FileKind);
        Assert.True(variant.IsFileValid(out _));
        
        var data = await getDataFunc(variant);
        
        Assert.Equal(SemVersion.ParsedFrom(1, 1, 1, "BETA"), data.Version);
        Assert.Single(data.Custom);
        Assert.Equal(2, data.Languages.Count);
        Assert.True(data.LanguagesExplicitlySet);
    }

    [Fact]
    public async Task InvalidMainFileContent_Throws()
    {
        var file = CreateVariantWithMainFile(true);
        Assert.Equal(FileKind, file.FileKind);

        await Task.Run(() =>
        {
            Assert.True(file.IsFileValid(out _));
            Assert.False(file.TryGetModinfo(out var modinfo));
            Assert.Null(modinfo);
            Assert.Throws<ModinfoParseException>((Func<object?>)(() => file.GetModinfo()));
        });
        await Assert.ThrowsAsync<ModinfoParseException>(() => file.GetModinfoAsync());
    }

    [Fact]
    public void MergeMainIntoVariant_WhereVariantSetsLanguageExplicitly()
    {
        FileSystem.Directory.CreateDirectory("integration");
        FileSystem.File.WriteAllText("integration/modinfo.json", TestUtilities.MainModinfoData);
        FileSystem.File.WriteAllText("integration/variant-modinfo.json", TestUtilities.VariantModifnoDataWithExplicitLanguage);

        var mainFile = new MainModinfoFile(FileSystem.FileInfo.New("integration/modinfo.json"));
        var variantFile = new ModinfoVariantFile(FileSystem.FileInfo.New("integration/variant-modinfo.json"), mainFile);

        var main = mainFile.GetModinfo();
        Assert.Equal(2, main.Languages.Count);

        var variant = variantFile.GetModinfo();
        Assert.Single(variant.Languages);
        Assert.True(variant.LanguagesExplicitlySet);
    }
}