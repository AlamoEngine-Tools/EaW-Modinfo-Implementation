using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using AET.Modinfo.File;
using Testably.Abstractions.Testing;
using Xunit;

namespace AET.Modinfo.Tests;

public class ModinfoFileFinderTests
{
    private readonly Dictionary<int, string> _scenarioPaths = new();
    private readonly MockFileSystem _fileSystem = new();

    [Fact]
    public void FindModinfoFiles_InvalidArgs_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ModinfoFileFinder.FindModinfoFiles(null!));
        var scenarioPath = _fileSystem.DirectoryInfo.New("notfound");
        Assert.Throws<DirectoryNotFoundException>(() => ModinfoFileFinder.FindModinfoFiles(scenarioPath));
    }

    public ModinfoFileFinderTests()
    {
        CreateScenario1_MainModinfoOnly();
        CreateScenario2_MainModinfoOnly_WithCaseInsensitiveName();
        CreateScenario3_WithNoValidModinfoFiles();
        CreateScenario4_WithMainModinfoAndVariant();
        CreateScenario5_WithOnlyVariants();
        CreateScenario6_CreateWithMainModinfoAndVariantAndOtherFiles();
        CreateScenario7_WithInvalidMainModinfo();
        CreateScenario8_WithMainModinfoAndInvalidVariant();
        CreateScenario9_WithInvalidMainModinfoAndValidVariant();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            CreateScenario10_MultipleMainModinfoFiles_Linux();
        }
    }

    [Theory]
    [InlineData(1, true, 0)]
    [InlineData(2, true, 0)]
    [InlineData(3, false, 0)]
    [InlineData(4, true, 1)]
    [InlineData(5, false, 2)]
    [InlineData(6, true, 1)]
    [InlineData(7, true, 0)]
    [InlineData(8, true, 1)]
    [InlineData(9, true, 1)]
    public void FindModinfoFiles_TestAll(int scenario, bool hasMain, int numberVariants)
    {
        var scenarioPath = _fileSystem.DirectoryInfo.New(_scenarioPaths[scenario]);
        var result = ModinfoFileFinder.FindModinfoFiles(scenarioPath);

        Assert.Equal(hasMain, result.HasMainModinfoFile);
        Assert.Equal(numberVariants, result.Variants.Count);
    }

    [Fact]
    public void FindModinfoFiles_CreateScenario10_MultipleMainModinfoFiles_Linux()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        var scenarioPath = _fileSystem.DirectoryInfo.New(_scenarioPaths[10]);
        var result = ModinfoFileFinder.FindModinfoFiles(scenarioPath);

        Assert.True(result.HasMainModinfoFile);
        Assert.Equal((string?)"modinfo.json", (string?)result.MainModinfo.File.Name);
        Assert.Empty(result.Variants);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    public void FindModinfoFiles_TestMerge(int scenario)
    {
        var scenarioPath = _fileSystem.DirectoryInfo.New(_scenarioPaths[scenario]);
        var all = ModinfoFileFinder.FindModinfoFiles(scenarioPath);
        Assert.NotNull(all.Variants.ElementAt(0).GetModinfo().Version);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(9)]
    public void FindModinfoFiles_InvalidMain_CannotGetModinfo(int scenario)
    {
        var scenarioPath = _fileSystem.DirectoryInfo.New(_scenarioPaths[scenario]);
        var all = ModinfoFileFinder.FindModinfoFiles(scenarioPath);

        Assert.False(all.MainModinfo!.TryGetModinfo(out _));
        foreach (var modinfoVariantFile in all.Variants) 
            Assert.False(modinfoVariantFile.TryGetModinfo(out _));
    }

    private void CreateScenario1_MainModinfoOnly()
    {
        CreateScenario(1, () =>
        {
            const string path = "scenario1";
            const string fileData = @"{
	""name"": ""testmod""
}";
            const string fileName = "modinfo.json";
            var filePath = _fileSystem.Path.Combine(path, fileName);
            _fileSystem.Directory.CreateDirectory(path);
            _fileSystem.File.WriteAllText(filePath, fileData);
            return path;
        });
    }

    private void CreateScenario2_MainModinfoOnly_WithCaseInsensitiveName()
    {
        CreateScenario(2, () =>
        {
            const string path = "scenario2";
            const string fileData = @"{
	""name"": ""testmod""
}";
            const string fileName = "MoDInfO.json";
            var filePath = _fileSystem.Path.Combine(path, fileName);
            _fileSystem.Directory.CreateDirectory(path);
            _fileSystem.File.WriteAllText(filePath, fileData);
            return path;
        });
    }

    private void CreateScenario3_WithNoValidModinfoFiles()
    {
        CreateScenario(3, () =>
        {
            const string path = "scenario3";

            _fileSystem.Directory.CreateDirectory(path);

            foreach (var name in TestUtilities.GetInvalidModinfoFileNames()) 
                _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(path, name), string.Empty);
            return path;
        });
    }

    private void CreateScenario4_WithMainModinfoAndVariant()
    {
        CreateScenario(4, () =>
        {
            const string path = "scenario4";
            const string mainFileName = "modinfo.json";
            const string variantFileName = "variant-modinfo.json";
            var mainFilePath = _fileSystem.Path.Combine(path, mainFileName);
            var variantFilePath = _fileSystem.Path.Combine(path, variantFileName);

            const string mainFileData = @"{
	""name"": ""testmod"",
	""version"": ""1.0.0""
}";

            const string variantFileData = @"{
	""name"": ""Addon""
}";
            _fileSystem.Directory.CreateDirectory(path);
            _fileSystem.File.WriteAllText(mainFilePath, mainFileData);
            _fileSystem.File.WriteAllText(variantFilePath, variantFileData);

            return path;
        });
    }

    private void CreateScenario5_WithOnlyVariants()
    {
        CreateScenario(5, () =>
        {
            const string path = "scenario5";
            const string variant1FileName = "1-modinfo.json";
            const string variant2FileName = "2-modinfo.json";
            var filePath1 = _fileSystem.Path.Combine(path, variant1FileName);
            var filePath2 = _fileSystem.Path.Combine(path, variant2FileName);

            const string data1 = @"{
	""name"": ""Addon-1""
}";

            const string data2 = @"{
	""name"": ""Addon-2""
}";
            _fileSystem.Directory.CreateDirectory(path);
            _fileSystem.File.WriteAllText(filePath1, data1);
            _fileSystem.File.WriteAllText(filePath2, data2);
            return path;
        });
    }

    private void CreateScenario6_CreateWithMainModinfoAndVariantAndOtherFiles()
    {
        CreateScenario(6, () =>
        {
            const string path = "scenario6";
            const string mainFileName = "modinfo.json";
            const string variantFileName = "variant-modinfo.json";
            var mainFilePath = _fileSystem.Path.Combine(path, mainFileName);
            var variantFilePath = _fileSystem.Path.Combine(path, variantFileName);

            const string mainFileData = @"{
	""name"": ""testmod"",
	""version"": ""1.0.0""
}";

            const string variantFileData = @"{
	""name"": ""Addon""
}";
            _fileSystem.Directory.CreateDirectory(path);
            _fileSystem.File.WriteAllText(mainFilePath, mainFileData);
            _fileSystem.File.WriteAllText(variantFilePath, variantFileData);

            foreach (var name in TestUtilities.GetInvalidModinfoFileNames())
                _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(path, name), string.Empty);

            return path;
        });
    }

    private void CreateScenario7_WithInvalidMainModinfo()
    {
        CreateScenario(7, () =>
        {
            const string path = "scenario7";
            const string mainFileName = "modinfo.json";
            var mainFilePath = _fileSystem.Path.Combine(path, mainFileName);

            const string mainFileData = "\0";

            _fileSystem.Directory.CreateDirectory(path);
            _fileSystem.File.WriteAllText(mainFilePath, mainFileData);
            
            return path;
        });
    }

    private void CreateScenario8_WithMainModinfoAndInvalidVariant()
    {
        CreateScenario(8, () =>
        {
            const string path = "scenario8";
            const string mainFileName = "modinfo.json";
            const string variantFileName = "variant-modinfo.json";
            var mainFilePath = _fileSystem.Path.Combine(path, mainFileName);
            var variantFilePath = _fileSystem.Path.Combine(path, variantFileName);

            const string mainFileData = @"{
	""name"": ""testmod"",
	""version"": ""1.0.0""
}";

            const string variantFileData = "\0";

            _fileSystem.Directory.CreateDirectory(path);
            _fileSystem.File.WriteAllText(mainFilePath, mainFileData);
            _fileSystem.File.WriteAllText(variantFilePath, variantFileData);
            
            return path;
        });
    }

    private void CreateScenario9_WithInvalidMainModinfoAndValidVariant()
    {
        CreateScenario(9, () =>
        {
            const string path = "scenario9";
            const string mainFileName = "modinfo.json";
            const string variantFileName = "variant-modinfo.json";
            var mainFilePath = _fileSystem.Path.Combine(path, mainFileName);
            var variantFilePath = _fileSystem.Path.Combine(path, variantFileName);

            const string mainFileData = "\0";

            const string variantFileData = @"{
	""name"": ""Addon""
}";

            _fileSystem.Directory.CreateDirectory(path);
            _fileSystem.File.WriteAllText(mainFilePath, mainFileData);
            _fileSystem.File.WriteAllText(variantFilePath, variantFileData);

            return path;
        });
    }

    private void CreateScenario10_MultipleMainModinfoFiles_Linux()
    {
        CreateScenario(10, () =>
        {
            const string path = "scenario10";
            const string fileData = @"{
	""name"": ""testmod""
}";

            _fileSystem.Directory.CreateDirectory(path);
            _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(path, "MoDInfO.json"), fileData);
            _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(path, "modinfo.json"), fileData);
            _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(path, "modinfo.JSON"), fileData);
            _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(path, "MODINFO.json"), fileData);
            return path;
        });
    }

    private void CreateScenario(int scenarioId, Func<string> initScenario)
    {
        var basePath = initScenario();
        _scenarioPaths.Add(scenarioId, basePath);
    }
}