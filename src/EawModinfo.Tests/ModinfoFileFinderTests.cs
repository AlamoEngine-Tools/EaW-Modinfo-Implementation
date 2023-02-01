using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using EawModinfo.File;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests;

public class ModinfoFileFinderTests
{
    private static readonly IDirectoryInfo[] Scenarios = new IDirectoryInfo[5];


    static ModinfoFileFinderTests()
    {
        var fs = new MockFileSystem();
        CreateScenario1(fs);
        CreateScenario2(fs);
        CreateScenario3(fs);
        CreateScenario4(fs);
        CreateScenario5(fs);
    }

    private static void CreateScenario1(MockFileSystem fs)
    {
        const string path = "scenario1";
        const string fileData = @"{
	""name"": ""testmod""
}";
        const string fileName = "modinfo.json";
        var filePath = fs.Path.Combine(path, fileName);

        fs.AddFile(filePath, new MockFileData(fileData));

        Scenarios[0] = fs.DirectoryInfo.FromDirectoryName(path);
    }

    private static void CreateScenario2(MockFileSystem fs)
    {
        const string path = "scenario2";
        const string fileData = @"{
	""name"": ""testmod""
}";
        const string fileName = "MoDInfO.json";
        var filePath = fs.Path.Combine(path, fileName);

        fs.AddFile(filePath, new MockFileData(fileData));

        Scenarios[1] = fs.DirectoryInfo.FromDirectoryName(path);
    }

    private static void CreateScenario3(MockFileSystem fs)
    {
        const string path = "scenario3";
        const string fileName = "empty.txt";
        var filePath = fs.Path.Combine(path, fileName);

        fs.AddFile(filePath, new MockFileData(string.Empty));

        Scenarios[2] = fs.DirectoryInfo.FromDirectoryName(path);
    }


    private static void CreateScenario4(MockFileSystem fs)
    {
        const string path = "scenario4";
        const string mainFileName = "modinfo.json";
        const string variantFileName = "variant-modinfo.json";
        var mainFilePath = fs.Path.Combine(path, mainFileName);
        var variantFilePath = fs.Path.Combine(path, variantFileName);

        const string mainFileData = @"{
	""name"": ""testmod"",
	""version"": ""1.0.0""
}";

        const string variantFileData = @"{
	""name"": ""Addon""
}";

        fs.AddFile(mainFilePath, new MockFileData(mainFileData));
        fs.AddFile(variantFilePath, new MockFileData(variantFileData));

        Scenarios[3] = fs.DirectoryInfo.FromDirectoryName(path);
    }


    private static void CreateScenario5(MockFileSystem fs)
    {
        const string path = "scenario5";
        const string variant1FileName = "1-modinfo.json";
        const string variant2FileName = "2-modinfo.json";
        var filePath1 = fs.Path.Combine(path, variant1FileName);
        var filePath2 = fs.Path.Combine(path, variant2FileName);

        const string data1 = @"{
	""name"": ""Addon-1""
}";

        const string data2 = @"{
	""name"": ""Addon-2""
}";

        fs.AddFile(filePath1, new MockFileData(data1));
        fs.AddFile(filePath2, new MockFileData(data2));

        Scenarios[4] = fs.DirectoryInfo.FromDirectoryName(path);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 0)]
    [InlineData(4, 1)]
    [InlineData(5, 0)]
    public void TestMain(int scenario, int expectedFilesFound)
    {
        var scenarioPath = Scenarios[scenario - 1];
        var finder = new ModinfoFileFinder(scenarioPath);

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
        var scenarioPath = Scenarios[scenario - 1];
        var finder = new ModinfoFileFinder(scenarioPath);

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
        var scenarioPath = Scenarios[scenario - 1];
        var finder = new ModinfoFileFinder(scenarioPath);
        var result = finder.Find(FindOptions.FindAny);
        Assert.Equal(expectedFilesFound, result.Count());
    }

    [Fact]
    public void TestMerge()
    {
        var finder = new ModinfoFileFinder(Scenarios[3]);
        var vars = finder.Find(FindOptions.FindVariants);
        var all = finder.Find(FindOptions.FindAny);

        Assert.NotNull(all.Variants.ElementAt(0).GetModinfo().Version);
        Assert.NotNull(vars.Variants.ElementAt(0).GetModinfo().Version);
    }

    [Fact]
    public void TestThrow()
    {
        var finder = new ModinfoFileFinder(Scenarios[2]);
        Assert.Throws<ModinfoException>(() => finder.FindThrow(FindOptions.FindAny));
    }

}