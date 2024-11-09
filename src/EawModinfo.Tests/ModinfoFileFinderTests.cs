using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using EawModinfo.File;
using Xunit;

namespace EawModinfo.Tests;

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
        CreateScenario_MainModinfoOnly();
        CreateScenario_MainModinfoOnly_WithCaseInsensitiveName();
        CreateScenario_WithNoValidModinfoFiles();
        CreateScenario_WithMainModinfoAndVariant();
        CreateScenario_WithOnlyVariants();
        CreateScenario_WithMainModinfoAndVariantAndInvalidFiles();
    }

    [Theory]
    [InlineData(1, true, 0)]
    [InlineData(2, true, 0)]
    [InlineData(3, false, 0)]
    [InlineData(4, true, 1)]
    [InlineData(5, false, 2)]
    [InlineData(6, true, 1)]
    public void FindModinfoFiles_TestAll(int scenario, bool hasMain, int numberVariants)
    {
        var scenarioPath = _fileSystem.DirectoryInfo.New(_scenarioPaths[scenario]);
        var result = ModinfoFileFinder.FindModinfoFiles(scenarioPath);

        Assert.Equal(hasMain, result.HasMainModinfoFile);
        Assert.Equal(numberVariants, result.Variants.Count);
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

    private void CreateScenario_MainModinfoOnly()
    {
        CreateScenario(1, () =>
        {
            const string path = "scenario1";
            const string fileData = @"{
	""name"": ""testmod""
}";
            const string fileName = "modinfo.json";
            var filePath = _fileSystem.Path.Combine(path, fileName);

            _fileSystem.AddFile(filePath, new MockFileData(fileData));
            return path;
        });
    }

    private void CreateScenario_MainModinfoOnly_WithCaseInsensitiveName()
    {
        CreateScenario(2, () =>
        {
            const string path = "scenario2";
            const string fileData = @"{
	""name"": ""testmod""
}";
            const string fileName = "MoDInfO.json";
            var filePath = _fileSystem.Path.Combine(path, fileName);

            _fileSystem.AddFile(filePath, new MockFileData(fileData));
            return path;
        });
    }

    private void CreateScenario_WithNoValidModinfoFiles()
    {
        CreateScenario(3, () =>
        {
            const string path = "scenario3";

            foreach (var name in TestUtilities.GetInvalidModinfoFileNames()) 
                _fileSystem.AddFile(_fileSystem.Path.Combine(path, name), new MockFileData(string.Empty));
            return path;
        });
    }

    private void CreateScenario_WithMainModinfoAndVariant()
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

            _fileSystem.AddFile(mainFilePath, new MockFileData(mainFileData));
            _fileSystem.AddFile(variantFilePath, new MockFileData(variantFileData));

            return path;
        });
    }

    private void CreateScenario_WithMainModinfoAndVariantAndInvalidFiles()
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

            _fileSystem.AddFile(mainFilePath, new MockFileData(mainFileData));
            _fileSystem.AddFile(variantFilePath, new MockFileData(variantFileData));

            foreach (var name in TestUtilities.GetInvalidModinfoFileNames())
                _fileSystem.AddFile(_fileSystem.Path.Combine(path, name), new MockFileData(string.Empty));

            return path;
        });
    }

    private void CreateScenario_WithOnlyVariants()
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

            _fileSystem.AddFile(filePath1, new MockFileData(data1));
            _fileSystem.AddFile(filePath2, new MockFileData(data2));
            return path;
        });
    }

    private void CreateScenario(int scenarioId, Func<string> initScenario)
    {
        var basePath = initScenario();
        _scenarioPaths.Add(scenarioId, basePath);
    }
}