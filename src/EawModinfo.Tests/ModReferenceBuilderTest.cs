using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using EawModinfo.File;
using EawModinfo.Model;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using Xunit;

namespace EawModinfo.Tests;

public class ModReferenceBuilderTest
{
    private readonly IFileSystem _fileSystem = new MockFileSystem();

    [Fact]
    public void CreateVirtualModIdentifier_ReturnsCorrectModReference()
    {
        var modinfo = ModinfoData.Parse(TestUtilities.MainModinfoData);
        var result = ModReferenceBuilder.CreateVirtualModIdentifier(modinfo);

        Assert.Equal(modinfo.ToJson(), result.Identifier);
        Assert.Equal(ModType.Virtual, result.Type);
    }

    [Fact]
    public void CreateVirtualModIdentifier_ThrowsArgumentNullException_WhenModinfoIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => ModReferenceBuilder.CreateVirtualModIdentifier(null!));
    }

    [Theory]
    [InlineData(ModLocationKind.GameModsDirectory)]
    [InlineData(ModLocationKind.SteamWorkshops)]
    [InlineData(ModLocationKind.External)]
    public void ThrowsArgumentNullException_WhenInputIsNull(ModLocationKind modLocation)
    {
        Assert.Throws<ArgumentNullException>(() =>
            ModReferenceBuilder.CreateIdentifiers(null!, modLocation));
    }

    [Fact]
    public void CreateIdentifiers_ThrowsModinfoException_WhenModIsSteamWorkshopButNotDirectory()
    {
        var modDirectoryPath = "Game/Mods/NoWorkshopsId";
        var modDirectory = _fileSystem.DirectoryInfo.New(modDirectoryPath);

        var modinfoFinderCollection = new ModinfoFinderCollection(modDirectory, null, []);

        Assert.Throws<ModinfoException>(() =>
            ModReferenceBuilder.CreateIdentifiers(modinfoFinderCollection, ModLocationKind.SteamWorkshops));
    }

    [Theory]
    [MemberData(nameof(ModDirectoryTestData))]
    public void CreateIdentifiers_NoModinfoFiles_ReturnsCorrectIdentifier(
        ModLocationKind locationKind, 
        string modDirectoryPath,
        string expectedIdentifier)
    {
        var modDirectory = _fileSystem.DirectoryInfo.New(modDirectoryPath);

        var modinfoFinderCollection = new ModinfoFinderCollection(modDirectory, null, new List<ModinfoVariantFile>());

        var modReferences = ModReferenceBuilder.CreateIdentifiers(modinfoFinderCollection, locationKind)
            .ToList();

        var modRef = Assert.Single(modReferences);
        if (locationKind is ModLocationKind.External)
            expectedIdentifier = _fileSystem.Path.GetFullPath(expectedIdentifier);
        Assert.Equal(expectedIdentifier, modRef.ModReference.Identifier);
        Assert.Equal(locationKind is ModLocationKind.SteamWorkshops ? ModType.Workshops : ModType.Default, modRef.ModReference.Type);
        Assert.Null(modRef.Modinfo);
    }

    [Theory]
    [MemberData(nameof(GetCombinedModinfoFilesTestData))]
    public void CreateIdentifiers_TestScenario(
      string modPath,
      ModLocationKind locationKind,
      bool? hasValidMainModinfo,
      ICollection<string> validVariants,
      ICollection<string> malformedVariants,
      string expectedBaseIdentifier)
    {
        var modDir = _fileSystem.DirectoryInfo.New(modPath);

        // Simulate the creation of a main modinfo file
        var mainModinfo = hasValidMainModinfo switch
        {
            true => CreateValidMainFile(modDir, "ValidMain.json"),
            false => CreateInvalidMainModinfoFile(modDir),
            _ => null
        };

        // Simulate the creation of variant files
        var variantFiles = validVariants.Select(x => CreateValidVariantFile(modDir, x))
            .Concat(malformedVariants.Select(x => CreateInvalidVariantFile(modDir)))
            .ToList();

        var modinfoFinderResult = new ModinfoFinderCollection(modDir, mainModinfo, variantFiles);

        var result = ModReferenceBuilder.CreateIdentifiers(modinfoFinderResult, locationKind).ToList();

        // Adjust expected identifier if it's external
        if (locationKind is ModLocationKind.External) 
            expectedBaseIdentifier = _fileSystem.Path.GetFullPath(expectedBaseIdentifier);

        var expectedCount = 1 + validVariants.Count;
        Assert.Equal(expectedCount, result.Count);

        var currentIndex = 0;

        // Validate main modinfo if it exists
        if (hasValidMainModinfo == true)
        {
            Assert.Equal(expectedBaseIdentifier, result[currentIndex].ModReference.Identifier);
            Assert.Equal(locationKind is ModLocationKind.SteamWorkshops ? ModType.Workshops : ModType.Default, result[currentIndex].ModReference.Type);
            Assert.NotNull(result[currentIndex].Modinfo);
            Assert.Equal("ValidMain.json", result[currentIndex].Modinfo.Name);
            currentIndex++;
        }

        // Validate valid variants
        var validVariantsList = validVariants.ToList(); // Ensure we can index into validVariants
        for (int i = 0; i < validVariantsList.Count; i++)
        {
            var expectedVariant = validVariantsList[i];
            var modReference = result[currentIndex];
            Assert.Equal($"{expectedBaseIdentifier}:{expectedVariant}", modReference.ModReference.Identifier);
            Assert.Equal(locationKind is ModLocationKind.SteamWorkshops ? ModType.Workshops : ModType.Default, modReference.ModReference.Type);
            Assert.NotNull(modReference.Modinfo);
            Assert.Equal(expectedVariant, modReference.Modinfo.Name);
            currentIndex++;
        }
    }
    
    public static IEnumerable<object[]> GetCombinedModinfoFilesTestData()
    {
        foreach (var pathData in ModDirectoryTestData())
        {
            var locationKind = (ModLocationKind)pathData[0];
            var modPath = (string)pathData[1];
            var expectedIdentifier = (string)pathData[2];

            yield return
            [
                modPath,
                locationKind,
                null!, // No main modinfo
                Array.Empty<string>(), // No main modinfo
                Array.Empty<string>(),
                expectedIdentifier
            ];

            yield return
            [
                modPath,
                locationKind,
                false, // Invalid main file
                Array.Empty<string>(), // No main modinfo
                Array.Empty<string>(),
                expectedIdentifier
            ];

            //yield return
            //[
            //    modPath,
            //    isSteamWorkshopMod,
            //    true, // Valid main modinfo
            //    new[] { "ValidVariant2" },
            //    Enumerable.Empty<string>(),
            //    expectedIdentifier,
            //    isExternalMod
            //];

            //yield return
            //[
            //    modPath,
            //    isSteamWorkshopMod,
            //    false, // Malformed main modinfo
            //    Enumerable.Empty<string>(),
            //    new[] { "MalformedVariant2" },
            //    expectedIdentifier,
            //    isExternalMod
            //];
        }
    }
    
    public static IEnumerable<object[]> ModDirectoryTestData()
    {
        yield return [ModLocationKind.SteamWorkshops, "Game/Mods/1234567890", "1234567890"];
        yield return [ModLocationKind.GameModsDirectory, "Game/Mods/DefaultMod", "DefaultMod"];
        yield return [ModLocationKind.External, "path/ExternalMod", "ExternalMod"];
    }

    private MainModinfoFile? CreateValidMainFile(IDirectoryInfo dir, string mainmodinfoJson)
    {
        throw new NotImplementedException();
    }

    private MainModinfoFile CreateInvalidMainModinfoFile(IDirectoryInfo dir)
    {
        dir.Create();
        var file = _fileSystem.FileInfo.New(_fileSystem.Path.Combine(dir.FullName, "modinfo.json"));
        using var sw = file.CreateText();
        sw.Write("This is not a valid modinfo file");
        file.Refresh();
        return new MainModinfoFile(file);
    }

    private ModinfoVariantFile CreateInvalidVariantFile(IDirectoryInfo dir)
    {
        dir.Create();
        var file = _fileSystem.FileInfo.New(_fileSystem.Path.Combine(dir.FullName, "modinfo.json"));
        using var sw = file.CreateText();
        sw.Write("This is not a valid modinfo file");
        file.Refresh();
        return new ModinfoVariantFile(file);
    }

    private ModinfoVariantFile CreateValidVariantFile(IDirectoryInfo dir, string name)
    {
        throw new NotImplementedException();
    }
}
