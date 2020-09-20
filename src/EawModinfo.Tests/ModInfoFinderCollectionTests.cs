using System.IO;
using System.Linq;
using EawModinfo.Spec;
using Xunit;

namespace EawModinfo.Tests
{
    public class ModInfoFinderCollectionTests
    {
        [Fact]
        public void Test1()
        {
            var mainFile = ModinfoFileTests.GetMain();
            var collection = new ModInfoFinderCollection(new DirectoryInfo(Directory.GetCurrentDirectory()), mainFile);
            Assert.Single(collection);
            Assert.NotNull(collection.MainModInfo);
            Assert.Empty(collection.Variants);
        }

        [Fact]
        public void Test2()
        {
            var variant = ModinfoFileTests.GetVariantStandalone();
            var collection = new ModInfoFinderCollection(new DirectoryInfo(Directory.GetCurrentDirectory()), new []{variant});
            Assert.Single(collection);
            Assert.Single(collection.Variants);
            Assert.Null(collection.MainModInfo);
        }

        [Fact]
        public void Test3()
        {
            var variant = ModinfoFileTests.GetVariantStandalone();
            var main = ModinfoFileTests.GetMain();
            var collection = new ModInfoFinderCollection(new DirectoryInfo(Directory.GetCurrentDirectory()), main, new[] { variant });
            Assert.Equal(2, collection.Count());
            Assert.Single(collection.Variants);
            Assert.NotNull(collection.MainModInfo);
        }

        [Fact]
        public void Test4()
        {
            var variant = ModinfoFileTests.GetVariantStandalone();
            var variantM = ModinfoFileTests.GetVariantMerged();
            var main = ModinfoFileTests.GetMain();
            var collection = new ModInfoFinderCollection(new DirectoryInfo(Directory.GetCurrentDirectory()), main, new[] { variant, variantM });
            Assert.Equal(3, collection.Count());
            Assert.Equal(2, collection.Variants.Count);
            Assert.NotNull(collection.MainModInfo);
        }

        [Fact]
        public void Test5()
        {
            var variant = ModinfoFileTests.GetVariantStandalone();
            var variantM = ModinfoFileTests.GetVariantMerged();
            var main = ModinfoFileTests.GetMain();
            Assert.Throws<ModinfoException>(() =>
                new ModInfoFinderCollection(new DirectoryInfo(Directory.GetCurrentDirectory()), variant));
        }

        [Fact]
        public void Test6()
        {
            var main = ModinfoFileTests.GetMain();
            Assert.Throws<ModinfoException>(() =>
                new ModInfoFinderCollection(new DirectoryInfo(Directory.GetCurrentDirectory()), new []{main}));
        }

        [Fact]
        public void Test7()
        {
            var variant = ModinfoFileTests.GetVariantStandalone();
            var main = ModinfoFileTests.GetMain();
            Assert.Throws<ModinfoException>(() =>
                new ModInfoFinderCollection(new DirectoryInfo(Directory.GetCurrentDirectory()), new[] { variant, main }));
        }
    }
}