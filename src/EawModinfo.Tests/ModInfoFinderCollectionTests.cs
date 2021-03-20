using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using EawModinfo.File;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests
{
    public class ModinfoFinderCollectionTests
    {
        [Fact]
        public void Test1()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>());

            var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(fs, "mods/A"));
            var collection = new ModinfoFinderCollection(fs.DirectoryInfo.FromDirectoryName("mods/A"), mainFile);
            Assert.Single(collection);
            Assert.NotNull(collection.MainModinfo);
            Assert.Empty(collection.Variants);
        }

        [Fact]
        public void Test2()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>());

            var variant = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(fs, "mods/A"));
            var collection = new ModinfoFinderCollection(
                fs.DirectoryInfo.FromDirectoryName("mods/A"), new[] { variant });
            Assert.Single(collection);
            Assert.Single(collection.Variants);
            Assert.Null(collection.MainModinfo);
        }

        [Fact]
        public void Test3()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>());

            var variant = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(fs, "mods/A"));
            var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(fs, "mods/A"));
            var collection = new ModinfoFinderCollection(
                fs.DirectoryInfo.FromDirectoryName("mods/A"), mainFile, new[] { variant });
            Assert.Equal(2, collection.Count());
            Assert.Single(collection.Variants);
            Assert.NotNull(collection.MainModinfo);
        }

        [Fact]
        public void Test4()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>());

            var variantFileInfo = ModinfoDataUtils.CreateVariantMainFile(fs, "mods/A");
            var variant = new ModinfoVariantFile(variantFileInfo);
            var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(fs, "mods/A"));
            var variantM = new ModinfoVariantFile(variantFileInfo, mainFile);
            var collection = new ModinfoFinderCollection(
                fs.DirectoryInfo.FromDirectoryName("mods/A"), mainFile, new[] { variant, variantM });
            Assert.Equal(3, collection.Count());
            Assert.Equal(2, collection.Variants.Count);
            Assert.NotNull(collection.MainModinfo);
        }

        [Fact]
        public void Test5()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>());

            var variant = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(fs, "mods/A"));

            Assert.Throws<ModinfoException>(() =>
                new ModinfoFinderCollection(fs.DirectoryInfo.FromDirectoryName("mods/A"), variant));
        }

        [Fact]
        public void Test6()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>());
            var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(fs, "mods/A"));
            Assert.Throws<ModinfoException>(() => new ModinfoFinderCollection(
                fs.DirectoryInfo.FromDirectoryName("mods/A"), new[] { mainFile }));
        }

        [Fact]
        public void Test7()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>());

            var variant = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(fs, "mods/A"));
            var mainFile = new MainModinfoFile(ModinfoDataUtils.CreateModifnoFile(fs, "mods/A"));

            Assert.Throws<ModinfoException>(() => new ModinfoFinderCollection(
                fs.DirectoryInfo.FromDirectoryName("mods/A"), new IModinfoFile[] {variant, mainFile}));
        }
    }
}