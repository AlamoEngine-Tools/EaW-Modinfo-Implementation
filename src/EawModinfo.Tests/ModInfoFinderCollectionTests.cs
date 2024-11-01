using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using EawModinfo.File;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests;

public class ModinfoFinderCollectionTests
{
    private readonly MockFileSystem _fileSystem = new();

    [Fact]
    public void Test_Enumerate_Main()
    { 
        var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(_fileSystem, "mods/A"));
        var collection = new ModinfoFinderCollection(_fileSystem.DirectoryInfo.New("mods/A"), mainFile);
        Assert.Single(collection);
        Assert.NotNull(collection.MainModinfo);
        Assert.Empty(collection.Variants);
        Assert.Equal(_fileSystem.DirectoryInfo.New("mods/A").FullName, collection.Directory.FullName);
    }

    [Fact]
    public void Test_Enumerate_OnlyVariants()
    {
        var variant = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(_fileSystem, "mods/A"));
        var collection = new ModinfoFinderCollection(
            _fileSystem.DirectoryInfo.New("mods/A"), [variant]);
        Assert.Single(collection);
        Assert.Single(collection.Variants);
        Assert.Null(collection.MainModinfo);
    }

    [Fact]
    public void Test_Enumerate_MainPlusSingleVariants()
    { 
        var variant = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(_fileSystem, "mods/A"));
        var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(_fileSystem, "mods/A"));
        var collection = new ModinfoFinderCollection(
            _fileSystem.DirectoryInfo.New("mods/A"), mainFile, [variant]);
        Assert.Equal(2, collection.Count());
        Assert.Single(collection.Variants);
        Assert.NotNull(collection.MainModinfo);
    }

    [Fact]
    public void Test_Enumerate_MainPlusMultipleVariants()
    {
        var variantFileInfo = ModinfoDataUtils.CreateVariantMainFile(_fileSystem, "mods/A");
        var variant = new ModinfoVariantFile(variantFileInfo);
        var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(_fileSystem, "mods/A"));
        var variantM = new ModinfoVariantFile(variantFileInfo, mainFile);
        var collection = new ModinfoFinderCollection(
            _fileSystem.DirectoryInfo.New("mods/A"), mainFile, [variant, variantM]);
        Assert.Equal(3, collection.Count());
        Assert.Equal(2, collection.Variants.Count);
        Assert.NotNull(collection.MainModinfo);
    }

    [Fact]
    public void Test_Ctor_VariantAsMain_ThrowsModinfoException()
    {
        var variant = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(_fileSystem, "mods/A"));

        Assert.Throws<ModinfoException>(() =>
            new ModinfoFinderCollection(_fileSystem.DirectoryInfo.New("mods/A"), variant));
    }

    [Fact]
    public void Test_Ctor_MainAsVariant_ThrowsModinfoException()
    {
        var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(_fileSystem, "mods/A"));
        Assert.Throws<ModinfoException>(() => new ModinfoFinderCollection(
            _fileSystem.DirectoryInfo.New("mods/A"), [mainFile]));
    }

    [Fact]
    public void Test_Ctor_MainAsVariant_ThrowsModinfoException2()
    { 
        var variant = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(_fileSystem, "mods/A"));
        var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(_fileSystem, "mods/A"));

        Assert.Throws<ModinfoException>(() => new ModinfoFinderCollection(
            _fileSystem.DirectoryInfo.New("mods/A"), [variant, mainFile]));
    }

    [Fact]
    public void Test_Enumerate_Empty()
    {
        var collection = new ModinfoFinderCollection(_fileSystem.DirectoryInfo.New("mods/A"));
        Assert.Null(collection.MainModinfo);
        Assert.Empty(collection.Variants);
    }
}