using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using EawModinfo.File;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests;

public class ModinfoFileFinderTests
{
    private readonly Dictionary<int, string> _scenarioPaths = new();
    private readonly MockFileSystem _fileSystem = new();

    public ModinfoFileFinderTests()
    {
        CreateScenario1();
        CreateScenario2();
        CreateScenario3();
        CreateScenario4();
        CreateScenario5();
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 0)]
    [InlineData(4, 1)]
    [InlineData(5, 0)]
    public void TestMain(int scenario, int expectedFilesFound)
    {
        var scenarioPath = _fileSystem.DirectoryInfo.New(_scenarioPaths[scenario]);
        var finder = new ModinfoFileFinder(scenarioPath);
        Assert.Equal(scenarioPath, finder.Directory);

        var result = finder.Find(FindOptions.FindMain);

        Assert.Equal(expectedFilesFound, result.Count());

    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 1)]
    [InlineData(5, 2)]
    public void TestVariants(int scenario, int expectedFilesFound)
    {
        var scenarioPath = _fileSystem.DirectoryInfo.New(_scenarioPaths[scenario]);
        var finder = new ModinfoFileFinder(scenarioPath);
        Assert.Equal(scenarioPath, finder.Directory);

        var result = finder.Find(FindOptions.FindVariants);

        Assert.Equal(expectedFilesFound, result.Count());

    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 0)]
    [InlineData(4, 2)]
    [InlineData(5, 2)]
    public void TestAll(int scenario, int expectedFilesFound)
    {
        var scenarioPath = _fileSystem.DirectoryInfo.New(_scenarioPaths[scenario]);
        var finder = new ModinfoFileFinder(scenarioPath);
        Assert.Equal(scenarioPath, finder.Directory);

        var result = finder.Find(FindOptions.FindAny);
        Assert.Equal(expectedFilesFound, result.Count());
    }

    [Fact]
    public void TestMerge()
    {
        var scenarioPath = _fileSystem.DirectoryInfo.New(_scenarioPaths[4]);
        var finder = new ModinfoFileFinder(scenarioPath);
        Assert.Equal(scenarioPath, finder.Directory);

        var vars = finder.Find(FindOptions.FindVariants);
        var all = finder.Find(FindOptions.FindAny);

        Assert.NotNull(all.Variants.ElementAt(0).GetModinfo().Version);
        Assert.NotNull(vars.Variants.ElementAt(0).GetModinfo().Version);
    }
    
    private void CreateScenario1()
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

    private void CreateScenario2()
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

    private void CreateScenario3()
    {
        CreateScenario(3, () =>
        {
            const string path = "scenario3";
            const string fileName = "empty.txt";
            var filePath = _fileSystem.Path.Combine(path, fileName);

            _fileSystem.AddFile(filePath, new MockFileData(string.Empty));
            return path;
        });
    }

    private void CreateScenario4()
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
    
    private void CreateScenario5()
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