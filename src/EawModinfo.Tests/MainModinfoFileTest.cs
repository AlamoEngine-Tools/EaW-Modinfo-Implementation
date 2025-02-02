using System;
using AET.Modinfo.File;
using AET.Modinfo.Spec;
using Xunit;

namespace AET.Modinfo.Tests;

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
        fileInfo.Directory!.Create();

        FileSystem.File.WriteAllText(path, !isInvalidFileContent ? TestUtilities.MainModinfoData : "{}");
        fileInfo.Refresh();
        return new MainModinfoFile(fileInfo);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MainModinfoFile(null!));
    }
}