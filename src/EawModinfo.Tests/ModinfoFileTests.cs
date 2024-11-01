using System;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using EawModinfo.File;
using EawModinfo.Model;
using EawModinfo.Spec;
using Semver;
using Xunit;

namespace EawModinfo.Tests;

public class ModinfoFileTests
{
    private readonly MockFileSystem _fileSystem = new();

    [Fact]
    public async Task Test_GetModinfo_MainFile()
    {
        var fileInfo = ModinfoDataUtils.CreateModifnoFile(_fileSystem, "mods/A");
        var modinfoFile = new MainModinfoFile(fileInfo);

        Assert.Equal(ModinfoFileKind.MainFile, modinfoFile.FileKind);

        await Task.Run(() =>
        {
            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
        });
        Assert.Null(await Record.ExceptionAsync(modinfoFile.GetModinfoAsync));
    }

    [Fact]
    public async Task Test_GetModinfo_VariantFile1()
    {
        var fileInfo = ModinfoDataUtils.CreateVariantMainFile(_fileSystem, "mods/A");
        var modinfoFile = new ModinfoVariantFile(fileInfo);

        Assert.Equal(ModinfoFileKind.VariantFile, modinfoFile.FileKind);

        await Task.Run(() =>
        {
            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
        });
        
        Assert.Null(await Record.ExceptionAsync(modinfoFile.GetModinfoAsync));
    }

    [Fact]
    public async Task Test_GetModinfo_VariantFile2()
    {
        var mainFileInfo = ModinfoDataUtils.CreateModifnoFile(_fileSystem, "mods/A");
        var mainFile = new MainModinfoFile(mainFileInfo);

        var variantFileInfo = ModinfoDataUtils.CreateVariantFile(_fileSystem, "mods/A");
        var variantFile = new ModinfoVariantFile(variantFileInfo, mainFile);

        Assert.Equal(ModinfoFileKind.VariantFile, variantFile.FileKind);

        await Task.Run(() =>
        {
            Assert.Null(Record.Exception(variantFile.ValidateFile));
            Assert.Null(Record.Exception(variantFile.GetModinfo));
        });
        Assert.Null(await Record.ExceptionAsync(variantFile.GetModinfoAsync));

        var data = await variantFile.GetModinfoAsync();
        Assert.Equal(SemVersion.ParsedFrom(1, 1, 1, "BETA"), data.Version);
        Assert.Single(data.Custom);
        Assert.Single(data.Languages);

        await Task.Run(() =>
        {
            var syncData = variantFile.GetModinfo();
            Assert.Equal(SemVersion.ParsedFrom(1, 1, 1, "BETA"), data.Version);
            Assert.Single(syncData.Custom);
            Assert.Single(syncData.Languages);
        });
    }

    [Fact]
    public async Task Test_GetModinfo_VariantFile3()
    {
        var main = new ModinfoData("Main")
        {
            Version = new SemVersion(1, 1, 1),
            Dependencies = new DependencyList([new ModReference { Identifier = "123", Type = ModType.Workshops }], DependencyResolveLayout.FullResolved)
        };

        var file = ModinfoDataUtils.CreateVariantFile(_fileSystem, "mods/A");

        var modinfoFile = new ModinfoVariantFile(file, main);

        Assert.Equal(ModinfoFileKind.VariantFile, modinfoFile.FileKind);


        await Task.Run(() =>
        {
            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
        });
       
        Assert.Null(await Record.ExceptionAsync(modinfoFile.GetModinfoAsync));

        var data = await modinfoFile.GetModinfoAsync();
        Assert.Equal(new SemVersion(1, 1, 1), data.Version);
        Assert.Single(data.Dependencies);

        await Task.Run(() =>
        {
            var syncData = modinfoFile.GetModinfo();
            Assert.Equal(new SemVersion(1, 1, 1), syncData.Version);
            Assert.Single(syncData.Dependencies);
        });
    }

    [Fact]
    public void Test_GetModinfo_VariantFile4()
    {
        var variantMain = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(_fileSystem, "mods/A"));
        var variant = ModinfoDataUtils.CreateVariantFile(_fileSystem, "mods/A");
        Assert.Throws<ModinfoException>(() => new ModinfoVariantFile(variant, variantMain));
    }

    [Fact]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MainModinfoFile(null!));
        Assert.Throws<ArgumentNullException>(() => new ModinfoVariantFile(null!));
    }

    [Fact]
    public void Test_ValidateFile_FileNotFound_ThenFileIsFound()
    {
        var main = new MainModinfoFile(_fileSystem.FileInfo.New("modinfo.json"));
        var variant = new ModinfoVariantFile(_fileSystem.FileInfo.New("variant-modinfo.json"));

        Assert.Throws<ModinfoException>(() => main.ValidateFile());
        Assert.Throws<ModinfoException>(() => variant.ValidateFile());

        using var _ = _fileSystem.File.Create("modinfo.json");
        using var __ = _fileSystem.File.Create("variant-modinfo.json");
        main.ValidateFile();
        variant.ValidateFile();
    }

    [Fact]
    public void Test_ValidateFile_InvalidName()
    {
        using var _ = _fileSystem.File.Create("someName.json");
        using var __ = _fileSystem.File.Create("variant.json");
        var main = new MainModinfoFile(_fileSystem.FileInfo.New("someName.json"));
        var variant = new ModinfoVariantFile(_fileSystem.FileInfo.New("variant.json"));

        Assert.True(main.File.Exists);
        Assert.True(variant.File.Exists);
        
        Assert.Throws<ModinfoException>(() => main.ValidateFile());
        Assert.Throws<ModinfoException>(() => variant.ValidateFile());
    }
}