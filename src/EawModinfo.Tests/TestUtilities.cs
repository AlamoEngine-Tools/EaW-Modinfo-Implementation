using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json;
using EawModinfo.Model.Json.Schema;
using Testably.Abstractions.Testing;

namespace EawModinfo.Tests;

internal static class TestUtilities
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

    internal static string VariantModifnoDataWithExplicitLanguage = @"{
	""name"": ""Addon"",
    ""languages"":[
        {
          ""code"": ""en""
        }
    ]
}";

    internal static IFileInfo CreateModinfoFile(MockFileSystem fs, string path)
    {
        const string name = "modinfo.json";
        return CreateFile(fs, path, name, MainModinfoData);
    }

    internal static IFileInfo CreateVariantMainFile(MockFileSystem fs, string path)
    {
        const string name = "variantMain-modinfo.json";
        return CreateFile(fs, path, name, VariantMainModinfoData);
    }

    internal static IFileInfo CreateFile(MockFileSystem fs, string path, string name, string data)
    {
        var fullPath = fs.Path.Combine(path, name);
        var dir = fs.Path.GetDirectoryName(fullPath);
        fs.Directory.CreateDirectory(dir);
        fs.File.WriteAllText(fullPath, data);
        return fs.FileInfo.New(fullPath);
    }

    public static IEnumerable<string> GetInvalidModinfoFileNames()
    {
        yield return "-modinfo.json";
        yield return "file.json";
        yield return "modinfo.txt";
        yield return "modinf.json";
        yield return "variant-modinf.json";
    }

    public static IEnumerable<object[]> InvalidModinfoFileNamesTestData()
    {
        return GetInvalidModinfoFileNames().Select(name => (object[])[name]);
    }

    public static void Evaluate(string json, EvaluationType type)
    {
        ModInfoJsonSchema.Evaluate(JsonNode.Parse(json, null, new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        })!, type);
    }

    public static bool GetRandomBool()
    {
        var random = new Random();
        return random.Next() % 2 == 0;
    }
}