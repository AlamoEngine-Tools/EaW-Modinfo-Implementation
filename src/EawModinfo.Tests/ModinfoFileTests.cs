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
    [Fact]
    public async Task Test_GetModinfo_MainFile()
    {
        var fileInfo = ModinfoDataUtils.CreateModifnoFile(new MockFileSystem(), "mods/A");
        IModinfoFile modinfoFile = new MainModinfoFile(fileInfo);

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
        var fileInfo = ModinfoDataUtils.CreateVariantMainFile(new MockFileSystem(), "mods/A");
        IModinfoFile modinfoFile = new ModinfoVariantFile(fileInfo);

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
        var fs = new MockFileSystem();

        var mainFileInfo = ModinfoDataUtils.CreateModifnoFile(fs, "mods/A");
        IModinfoFile mainFile = new MainModinfoFile(mainFileInfo);

        var variantFileInfo = ModinfoDataUtils.CreateVariantFile(fs, "mods/A");
        IModinfoFile variantFile = new ModinfoVariantFile(variantFileInfo, mainFile);

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
            Dependencies = new DependencyList(new IModReference[] { new ModReference { Identifier = "123", Type = ModType.Workshops } }, DependencyResolveLayout.FullResolved)
        };

        var file = ModinfoDataUtils.CreateVariantFile(new MockFileSystem(), "mods/A");

        IModinfoFile modinfoFile = new ModinfoVariantFile(file, main);

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
        var fs = new MockFileSystem();
        var variantMain = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(fs, "mods/A"));
        var variant = ModinfoDataUtils.CreateVariantFile(fs, "mods/A");
        Assert.Throws<ModinfoException>(() => new ModinfoVariantFile(variant, variantMain));
    }

    [Fact]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MainModinfoFile(null!));
        Assert.Throws<ArgumentNullException>(() => new ModinfoVariantFile(null!));
    }
}