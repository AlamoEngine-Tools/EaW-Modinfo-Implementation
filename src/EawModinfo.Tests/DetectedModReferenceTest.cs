using System;
using System.IO.Abstractions;
using AET.Modinfo.Model;
using AET.Modinfo.Spec;
using AET.Modinfo.Utilities;
using Testably.Abstractions.Testing;
using Xunit;

namespace AET.Modinfo.Tests;

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