using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace EawModinfo.Tests;

internal static class ModinfoDataUtils
{
    internal static string MainModinfoData = @"{
	""name"": ""testmod"",
	""version"": ""1.1.1-BETA"",
	""languages"": [
		{
			""code"": ""en""
		},
		{
			""code"": ""de"",
			""support"": 7
		}
	],
	""custom"": {
		""key"": ""v4lu3""
	}
}";

    internal static string VariantMainModinfoData = @"{
	""name"": ""testmodVariant"",
	""version"": ""1.1.1-BETA"",
	""languages"": [
		{
			""code"": ""en""
		},
		{
			""code"": ""de"",
			""support"": 7
		}
	],
	""custom"": {
		""key"": ""v4lu3""
	}
}";

    internal static string VariantModifnoData = @"{
	""name"": ""Addon""
}";

    internal static IFileInfo CreateModifnoFile(MockFileSystem fs, string path)
    {
<<<<<<< HEAD
=======
        Requires.NotNull(fs, nameof(fs));
>>>>>>> to c# 10 namespaces
        const string name = "modinfo.json";
        return CreateFile(fs, path, name, MainModinfoData);
    }

    internal static IFileInfo CreateVariantMainFile(MockFileSystem fs, string path)
    {
        const string name = "variantMain-modinfo.json";
        return CreateFile(fs, path, name, VariantMainModinfoData);
    }

    internal static IFileInfo CreateVariantFile(MockFileSystem fs, string path)
    {
        const string name = "variant-modinfo.json";
        return CreateFile(fs, path, name, VariantModifnoData);
    }

    internal static IFileInfo CreateFile(MockFileSystem fs, string path, string name, string data)
    {
<<<<<<< HEAD
=======
        Requires.NotNull(fs, nameof(fs));
>>>>>>> to c# 10 namespaces
        var fullPath = fs.Path.Combine(path, name);
        fs.AddFile(fullPath, new MockFileData(data));
        return fs.FileInfo.FromFileName(fullPath);
    }
}