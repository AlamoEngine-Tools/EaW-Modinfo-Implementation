using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests;

public abstract class ModinfoFileTestsBase
{
    protected readonly MockFileSystem FileSystem = new();

    protected abstract ModinfoFileKind FileKind { get; }

    protected abstract string GetFileName();

    protected abstract IModinfoFile CreateFile(string path, bool isInvalidFileContent = false);

    [Theory]
    [MemberData(nameof(TestUtilities.InvalidModinfoFileNamesTestData), MemberType = typeof(TestUtilities))]
    public async void ValidateFile_InvalidName(string name)
    {
        var file = CreateFile(FileSystem.Path.Combine("mods", "myMod", name));
        Assert.True(file.File.Exists);
        Assert.False(file.IsFileValid(out _));

        Assert.Throws<ModinfoException>(() => file.GetModinfo());
        await Assert.ThrowsAsync<ModinfoException>(() => file.GetModinfoAsync());
        Assert.False(file.TryGetModinfo(out var info));
        Assert.Null(info);
    }

    [Fact]
    public void GetModinfo_ValidFile()
    {
        var file = CreateFile(FileSystem.Path.Combine("mods", "myMod", GetFileName()));
        Assert.Equal(FileKind, file.FileKind);
        
        Assert.True(file.IsFileValid(out _));
        var modInfo = file.GetModinfo();
        Assert.NotNull(modInfo);

        // Call again
        modInfo = file.GetModinfo();
        Assert.NotNull(modInfo);
    }

    [Fact]
    public async Task GetModinfoAsync_ValidFile()
    {
        var file = CreateFile(FileSystem.Path.Combine("mods", "myMod", GetFileName()));
        Assert.Equal(FileKind, file.FileKind);

        Assert.True(file.IsFileValid(out _));
        var modInfo = await file.GetModinfoAsync();
        Assert.NotNull(modInfo);

        // Call again
        modInfo = await file.GetModinfoAsync();
        Assert.NotNull(modInfo);
    }

    [Fact]
    public void TryGetModinfo_ValidFile()
    {
        var file = CreateFile(FileSystem.Path.Combine("mods", "myMod", GetFileName()));
        Assert.Equal(FileKind, file.FileKind);

        Assert.True(file.IsFileValid(out _));
        Assert.True(file.TryGetModinfo(out var modinfo));
        Assert.NotNull(modinfo);
    }

    [Fact]
    public async Task InvalidFileContent_Throws()
    {
        var file = CreateFile(FileSystem.Path.Combine("mods", "myMod", GetFileName()), true);
        Assert.Equal(FileKind, file.FileKind);

        await Task.Run(() =>
        {
            Assert.True(file.IsFileValid(out _));
            Assert.False(file.TryGetModinfo(out var modinfo));
            Assert.Null(modinfo);
            Assert.Throws<ModinfoParseException>(() => file.GetModinfo());
        });
        await Assert.ThrowsAsync<ModinfoParseException>(() => file.GetModinfoAsync());
    }

    [Fact]
    public void Test_ValidateFile_FileNotFound_ThenFileIsFound()
    {
        var file = CreateFile(FileSystem.Path.Combine("mods", "myMod", GetFileName()));

        file.File.Delete();

        Assert.False(file.IsFileValid(out _));

        FileSystem.File.Create(file.File.FullName);

        Assert.True(file.IsFileValid(out _));
    }
}