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

    #region CreateIdentifiers
    
    public static IEnumerable<object?[]> GetCombinedModinfoFilesTestData()
    {
        var mainModinfoStates = new bool?[]
        {
            // no main modinfo
            null,
            // valid main modinfo
            true, 
            // invalid main modinfo
            false
        };

        foreach (var pathData in ModDirectoryTestData())
        {
            var locationKind = (ModReferenceBuilder.ModLocationKind)pathData[0];
            var modPath = (string)pathData[1];
            var expectedIdentifier = (string)pathData[2];

            foreach (var mainModinfoState in mainModinfoStates)
            {
                yield return
                [
                    modPath,
                    locationKind,
                    mainModinfoState,
                    Array.Empty<string>(), // no variants
                    Array.Empty<string>(), // no invalid variants
                    expectedIdentifier
                ];
                yield return
                [
                    modPath,
                    locationKind,
                    mainModinfoState,
                    new[] {"variant1"},// one variants
                    Array.Empty<string>(),
                    expectedIdentifier
                ];
                yield return
                [
                    modPath,
                    locationKind,
                    mainModinfoState,
                    new[] {"variant1", "variant2"},// many variants
                    Array.Empty<string>(),
                    expectedIdentifier
                ];
                yield return
                [
                    modPath,
                    locationKind,
                    mainModinfoState,
                    new[] {"variant1", "variant2"}, // many variants
                    new[] {"variant3"}, // invalid variant
                    expectedIdentifier
                ];
                yield return
                [
                    modPath,
                    locationKind,
                    mainModinfoState,
                    new[] {"variant1", "variant2"}, // many variants
                    new[] {"variant3", "variant4"}, // invalid variants
                    expectedIdentifier
                ];
                yield return
                [
                    modPath,
                    locationKind,
                    mainModinfoState,
                    Array.Empty<string>(), // no valid variants
                    new[] {"variant3", "variant4"}, // invalid variants
                    expectedIdentifier
                ];
            }
        }
    }

    public static IEnumerable<object[]> ModDirectoryTestData()
    {
        yield return [ModReferenceBuilder.ModLocationKind.SteamWorkshops, "Game/Mods/1234567890", "1234567890"];
        yield return [ModReferenceBuilder.ModLocationKind.GameModsDirectory, "Game/Mods/DefaultMod", "DefaultMod"];
        yield return [ModReferenceBuilder.ModLocationKind.External, "path/ExternalMod", "path/ExternalMod"];
    }


    [Theory]
    [InlineData(ModReferenceBuilder.ModLocationKind.GameModsDirectory)]
    [InlineData(ModReferenceBuilder.ModLocationKind.SteamWorkshops)]
    [InlineData(ModReferenceBuilder.ModLocationKind.External)]
    public void CreateIdentifiers_ThrowsArgumentNullException_WhenInputIsNull(ModReferenceBuilder.ModLocationKind modLocation)
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
            ModReferenceBuilder.CreateIdentifiers(modinfoFinderCollection, ModReferenceBuilder.ModLocationKind.SteamWorkshops));
    }

    [Theory]
    [MemberData(nameof(ModDirectoryTestData))]
    public void CreateIdentifiers_NoModinfoFiles_ReturnsCorrectIdentifier(
        ModReferenceBuilder.ModLocationKind locationKind, 
        string modDirectoryPath,
        string expectedIdentifier)
    {
        var modDirectory = _fileSystem.DirectoryInfo.New(modDirectoryPath);

        var modinfoFinderCollection = new ModinfoFinderCollection(modDirectory, null, new List<ModinfoVariantFile>());

        var modReferences = ModReferenceBuilder.CreateIdentifiers(modinfoFinderCollection, locationKind)
            .ToList();

        var modRef = Assert.Single(modReferences);
        if (locationKind is ModReferenceBuilder.ModLocationKind.External)
            expectedIdentifier = _fileSystem.Path.GetFullPath(expectedIdentifier);
        Assert.Equal(expectedIdentifier, modRef.ModReference.Identifier);
        Assert.Equal(locationKind is ModReferenceBuilder.ModLocationKind.SteamWorkshops ? ModType.Workshops : ModType.Default, modRef.ModReference.Type);
        Assert.Null(modRef.Modinfo);
    }

    [Theory]
    [MemberData(nameof(ModDirectoryTestData))]
    public void CreateIdentifiers_OnlySingleVariant_ReturnsCorrectIdentifier(
        ModReferenceBuilder.ModLocationKind locationKind,
        string modDirectoryPath,
        string expectedIdentifier)
    {
        var modDirectory = _fileSystem.DirectoryInfo.New(modDirectoryPath);

        if (locationKind is ModReferenceBuilder.ModLocationKind.External)
            expectedIdentifier = _fileSystem.Path.GetFullPath(expectedIdentifier);

        var modinfoFinderCollection = new ModinfoFinderCollection(modDirectory, null, new List<ModinfoVariantFile>
        {
            CreateValidVariantFile(modDirectory, "VariantName")
        });

        var modReferences = ModReferenceBuilder.CreateIdentifiers(modinfoFinderCollection, locationKind)
            .ToList();

        var modRef = Assert.Single(modReferences);
        Assert.Equal($"{expectedIdentifier}:VariantName", modRef.ModReference.Identifier);
        Assert.Equal(locationKind is ModReferenceBuilder.ModLocationKind.SteamWorkshops ? ModType.Workshops : ModType.Default, modRef.ModReference.Type);
        Assert.NotNull(modRef.Modinfo);
    }

    [Theory]
    [MemberData(nameof(GetCombinedModinfoFilesTestData))]
    public void CreateIdentifiers_TestScenario(
    string modPath,
    ModReferenceBuilder.ModLocationKind locationKind,
    bool? hasValidMainModinfo,
    ICollection<string> validVariants,
    ICollection<string> malformedVariants,
    string expectedBaseIdentifier)
    {
        // Adjust expected identifier if it's external
        if (locationKind is ModReferenceBuilder.ModLocationKind.External)
            expectedBaseIdentifier = _fileSystem.Path.GetFullPath(expectedBaseIdentifier);

        var modDir = _fileSystem.DirectoryInfo.New(modPath);

        // Simulate the creation of a main modinfo file
        var mainModinfo = hasValidMainModinfo switch
        {
            true => CreateValidMainFile(modDir),
            false => CreateInvalidMainModinfoFile(modDir),
            _ => null
        };

        // Simulate the creation of variant files
        var variantFiles = validVariants.Select(x => CreateValidVariantFile(modDir, x))
            .Concat(malformedVariants.Select(x => CreateInvalidVariantFile(modDir)))
            .ToList();

        var modinfoFinderResult = new ModinfoFinderCollection(modDir, mainModinfo, variantFiles);

        var result = ModReferenceBuilder.CreateIdentifiers(modinfoFinderResult, locationKind).ToList();

        var expectedResults = new List<(string Identifier, ModType Type, string? Name)>();

        // Add the expected main modinfo entry
        if (hasValidMainModinfo == true)
        {
            expectedResults.Add((
                Identifier: expectedBaseIdentifier,
                Type: locationKind == ModReferenceBuilder.ModLocationKind.SteamWorkshops ? ModType.Workshops : ModType.Default,
                Name: "testmod"
            ));
        }
        // The main modinfo file exists but is invalid OR no valid variant modinfo files exist
        else if (hasValidMainModinfo == false || (hasValidMainModinfo is null && validVariants.Count == 0))
        {
            expectedResults.Add((
                Identifier: expectedBaseIdentifier,
                Type: locationKind == ModReferenceBuilder.ModLocationKind.SteamWorkshops ? ModType.Workshops : ModType.Default,
                Name: null
            ));
        }
        // Add the expected variant entries
        expectedResults.AddRange(validVariants.Select(variant => (
            Identifier: $"{expectedBaseIdentifier}:{variant}",
            Type: locationKind == ModReferenceBuilder.ModLocationKind.SteamWorkshops ? ModType.Workshops : ModType.Default,
            Name: variant
        )));

        Assert.Equal(expectedResults.Count, result.Count);

        foreach (var expected in expectedResults)
        {
            var actual = result.Single(r =>
                r.ModReference.Identifier == expected.Identifier &&
                r.ModReference.Type == expected.Type &&
                (expected.Name == null || r.Modinfo?.Name == expected.Name));

            if (expected.Name == null)
                Assert.Null(actual.Modinfo);
            else
                Assert.Equal(expected.Name, actual.Modinfo!.Name);

            Assert.Equal(modDir.FullName, actual.Directory.FullName);
        }
    }

    #endregion

    private MainModinfoFile CreateValidMainFile(IDirectoryInfo dir)
    {
        var file = CreateFile(dir, "modinfo.json", TestUtilities.MainModinfoData);
        return new MainModinfoFile(file);
    }

    private MainModinfoFile CreateInvalidMainModinfoFile(IDirectoryInfo dir)
    {
        var file = CreateFile(dir, "modinfo.json", "This is not a valid modinfo content");
        return new MainModinfoFile(file);
    }

    private ModinfoVariantFile CreateInvalidVariantFile(IDirectoryInfo dir)
    {
        dir.Create();
        var random = _fileSystem.Path.GetRandomFileName();
        var file = _fileSystem.FileInfo.New(_fileSystem.Path.Combine(dir.FullName, $"{random}-modinfo.json"));
        using var sw = file.CreateText();
        sw.Write("This is not a valid modinfo file");
        file.Refresh();
        return new ModinfoVariantFile(file);
    }

    private ModinfoVariantFile CreateValidVariantFile(IDirectoryInfo dir, string name)
    {
        var file = CreateFile(dir, $"{name}-modinfo.json", new ModinfoData(name).ToJson());
        return new ModinfoVariantFile(file);
    }

    private IFileInfo CreateFile(IDirectoryInfo dir, string fileName, string content)
    {
        dir.Create();
        var file = _fileSystem.FileInfo.New(_fileSystem.Path.Combine(dir.FullName, fileName));
        using var sw = file.CreateText();
        sw.Write(content);
        file.Refresh();
        return file;
    }
}