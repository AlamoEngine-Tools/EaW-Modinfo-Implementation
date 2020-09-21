using System;
using System.IO;
using System.Linq;
using EawModinfo.File;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests
{
    public class ModinfoFileFinderTests
    {
        [Theory]
        [InlineData("Scenario1", 1)]
        [InlineData("Scenario2", 1)]
        [InlineData("Scenario3", 0)]
        [InlineData("Scenario4", 1)]
        [InlineData("Scenario5", 0)]
        public void TestMain(string scenarioPath, int expectedFilesFound)
        {
            var dir = new DirectoryInfo(Path.Combine(GetSolutionPath(), "test", scenarioPath));
            var finder = new ModinfoFileFinder(dir);

            var result = finder.Find(FindOptions.FindMain);

            Assert.Equal(expectedFilesFound, result.Count());

        }

        [Theory]
        [InlineData("Scenario1", 0)]
        [InlineData("Scenario2", 0)]
        [InlineData("Scenario3", 0)]
        [InlineData("Scenario4", 1)]
        [InlineData("Scenario5", 2)]
        public void TestVariants(string scenarioPath, int expectedFilesFound)
        {
            var dir = new DirectoryInfo(Path.Combine(GetSolutionPath(), "test", scenarioPath));
            var finder = new ModinfoFileFinder(dir);

            var result = finder.Find(FindOptions.FindVariants);

            Assert.Equal(expectedFilesFound, result.Count());

        }

        [Theory]
        [InlineData("Scenario1", 1)]
        [InlineData("Scenario2", 1)]
        [InlineData("Scenario3", 0)]
        [InlineData("Scenario4", 2)]
        [InlineData("Scenario5", 2)]
        public void TestAll(string scenarioPath, int expectedFilesFound)
        {
            var dir = new DirectoryInfo(Path.Combine(GetSolutionPath(), "test", scenarioPath));
            var finder = new ModinfoFileFinder(dir);
            var result = finder.Find(FindOptions.FindAny);
            Assert.Equal(expectedFilesFound, result.Count());
        }

        [Fact]
        public void TestMerge()
        {
            var dir = new DirectoryInfo(Path.Combine(GetSolutionPath(), "test", "Scenario4"));
            var finder = new ModinfoFileFinder(dir);
            var vars = finder.Find(FindOptions.FindVariants);
            var alls = finder.Find(FindOptions.FindAny);

            Assert.NotNull(alls.Variants.ElementAt(0).GetModinfo().Version);
            Assert.NotNull(vars.Variants.ElementAt(0).GetModinfo().Version);
        }

        [Fact]
        public void TestThrow()
        {
            var dir = new DirectoryInfo(Path.Combine(GetSolutionPath(), "test", "Scenario3"));
            var finder = new ModinfoFileFinder(dir);
            Assert.Throws<ModinfoException>(() => finder.FindThrow(FindOptions.FindAny));
        }


        private static string GetSolutionPath()
        {
            var current = Directory.GetCurrentDirectory();
            var i = current.IndexOf("src\\", StringComparison.InvariantCulture);
            var p = current.Substring(0, i);
            return p;
        }
    }
}
