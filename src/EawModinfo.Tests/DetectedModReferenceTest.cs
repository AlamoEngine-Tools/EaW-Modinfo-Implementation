using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using EawModinfo.Model;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using Xunit;

namespace EawModinfo.Tests;

public class DetectedModReferenceTest
{
    private readonly IFileSystem _fileSystem = new MockFileSystem();
    
    [Fact]
    public void Constructor_ThrowsArgumentNullException()
    {
        var modReference = new ModReference("TestMod", ModType.Default);
        var directory = _fileSystem.DirectoryInfo.New("TestDirectory");
        Assert.Throws<ArgumentNullException>(() => new DetectedModReference(modReference, null!, null));
        Assert.Throws<ArgumentNullException>(() => new DetectedModReference(null!, directory, null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("string")]
    public void Constructor_InitializesPropertiesCorrectly_WhenValidArgumentsArePassed(string? modName)
    {
        var directory = _fileSystem.DirectoryInfo.New("TestDirectory");
        var modReference = new ModReference("TestMod", ModType.Default);
        IModinfo? modinfo = modName is null ? null : new ModinfoData(modName);

        var detectedModReference = new DetectedModReference(modReference, directory, modinfo);

        Assert.Equal(modReference, detectedModReference.ModReference);
        Assert.Same(directory, detectedModReference.Directory);
        Assert.Same(modinfo, detectedModReference.Modinfo);
    }
}