using System;
using EawModinfo.File;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests;

public class MainModinfoFileTest : ModinfoFileTestsBase
{
    protected override ModinfoFileKind FileKind => ModinfoFileKind.MainFile;

    protected override string GetFileName()
    {
        return "modinfo.json";
    }

    protected override IModinfoFile CreateFile(string path, bool isInvalidFileContent = false)
    {
        var fileInfo = FileSystem.FileInfo.New(path);
        fileInfo.Directory.Create();

        FileSystem.File.WriteAllText(path, !isInvalidFileContent ? ModinfoDataUtils.MainModinfoData : "{}");
        fileInfo.Refresh();
        return new MainModinfoFile(fileInfo);
    }

    [Fact]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MainModinfoFile(null!));
    }
}