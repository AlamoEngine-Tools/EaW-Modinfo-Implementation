using System;
using System.IO.Abstractions.TestingHelpers;
using EawModinfo.File;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests;

public class ModinfoFinderCollectionTests
{
    private readonly MockFileSystem _fileSystem = new();

    [Fact]
    public void Enumerate_Main()
    { 
        var mainFile = new MainModinfoFile(TestUtilities.CreateModinfoFile(_fileSystem, "mods/A"));
        var collection = new ModinfoFinderCollection(_fileSystem.DirectoryInfo.New("mods/A"), mainFile, []);
        Assert.True(collection.HasMainModinfoFile);
        Assert.NotNull(collection.MainModinfo);
        Assert.Empty(collection.Variants);
        Assert.False(collection.HasVariantModinfoFiles);
        Assert.Equal(_fileSystem.DirectoryInfo.New("mods/A").FullName, collection.Directory.FullName);
    }

    [Fact]
    public void Enumerate_OnlyVariants()
    {
        var variant = new ModinfoVariantFile(TestUtilities.CreateVariantMainFile(_fileSystem, "mods/A"));
        var collection = new ModinfoFinderCollection(_fileSystem.DirectoryInfo.New("mods/A"), null, [variant]);
        Assert.False(collection.HasMainModinfoFile);
        Assert.Null(collection.MainModinfo);
        Assert.Single(collection.Variants);
        Assert.True(collection.HasVariantModinfoFiles);
    }

    [Fact]
    public void Enumerate_MainPlusSingleVariants()
    { 
        var variant = new ModinfoVariantFile(TestUtilities.CreateVariantMainFile(_fileSystem, "mods/A"));
        var mainFile = new MainModinfoFile(TestUtilities.CreateModinfoFile(_fileSystem, "mods/A"));
        var collection = new ModinfoFinderCollection(_fileSystem.DirectoryInfo.New("mods/A"), mainFile, [variant]);
        Assert.True(collection.HasMainModinfoFile);
        Assert.NotNull(collection.MainModinfo);
        Assert.Single(collection.Variants);
        Assert.True(collection.HasVariantModinfoFiles);
    }

    [Fact]
    public void Enumerate_MainPlusMultipleVariants()
    {
        var variantFileInfo = TestUtilities.CreateVariantMainFile(_fileSystem, "mods/A");
        var variant = new ModinfoVariantFile(variantFileInfo);
        var mainFile = new MainModinfoFile(TestUtilities.CreateModinfoFile(_fileSystem, "mods/A"));
        var variantM = new ModinfoVariantFile(variantFileInfo, mainFile);
        var collection = new ModinfoFinderCollection(
            _fileSystem.DirectoryInfo.New("mods/A"), mainFile, [variant, variantM]);
        Assert.True(collection.HasMainModinfoFile);
        Assert.NotNull(collection.MainModinfo);
        Assert.Equal(2, collection.Variants.Count);
        Assert.True(collection.HasVariantModinfoFiles);
    }

    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ModinfoFinderCollection(null!, null, []));
        Assert.Throws<ArgumentNullException>(() => new ModinfoFinderCollection(_fileSystem.DirectoryInfo.New("path"), null, null!));
    }
}