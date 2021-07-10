using System;
using System.IO.Abstractions.TestingHelpers;
using EawModinfo.File;
using EawModinfo.Model;
using EawModinfo.Spec;
using Xunit;
using Version = SemanticVersioning.Version;

namespace EawModinfo.Tests
{
    public class ModinfoFileTests
    {
        [Fact]
        public void TestMainFile()
        {
            var fileInfo = ModinfoDataUtils.CreateModifnoFile(new MockFileSystem(), "mods/A");
            IModinfoFile modinfoFile = new MainModinfoFile(fileInfo);

            Assert.Equal(ModinfoFileKind.MainFile,modinfoFile.FileKind);

            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
            Assert.Null(Record.ExceptionAsync(modinfoFile.GetModinfoAsync).Result);
        }

        [Fact]
        public void TestVariantFile1()
        {
            var fileInfo = ModinfoDataUtils.CreateVariantMainFile(new MockFileSystem(), "mods/A");
            IModinfoFile modinfoFile = new ModinfoVariantFile(fileInfo);

            Assert.Equal(ModinfoFileKind.VariantFile, modinfoFile.FileKind);

            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
            Assert.Null(Record.ExceptionAsync(modinfoFile.GetModinfoAsync).Result);
        }

        [Fact]
        public void TestVariantFile2()
        {
            var fs = new MockFileSystem();

            var mainFileInfo = ModinfoDataUtils.CreateModifnoFile(fs, "mods/A");
            IModinfoFile mainFile = new MainModinfoFile(mainFileInfo);

            var variantFileInfo = ModinfoDataUtils.CreateVariantFile(fs, "mods/A");
            IModinfoFile variantFile = new ModinfoVariantFile(variantFileInfo, mainFile);
            
            Assert.Equal(ModinfoFileKind.VariantFile, variantFile.FileKind);

            Assert.Null(Record.Exception(variantFile.ValidateFile));
            Assert.Null(Record.Exception(variantFile.GetModinfo));
            Assert.Null(Record.ExceptionAsync(variantFile.GetModinfoAsync).Result);

            var data = variantFile.GetModinfo();

            Assert.Equal(new Version(1, 1, 1, "BETA"), data.Version);
            Assert.Single(data.Custom);
            Assert.Single(data.Languages);
        }

        [Fact]
        public void TestVariantFile3()
        {
            var main = new ModinfoData("Main")
            {
                Version = new Version(1, 1, 1),
                Dependencies = new IModReference[] { new ModReference { Identifier = "123", Type = ModType.Workshops } }
            };

            var file = ModinfoDataUtils.CreateVariantFile(new MockFileSystem(), "mods/A");

            IModinfoFile modinfoFile = new ModinfoVariantFile(file, main);

            Assert.Equal(ModinfoFileKind.VariantFile, modinfoFile.FileKind);

            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
            Assert.Null(Record.ExceptionAsync(modinfoFile.GetModinfoAsync).Result);

            var data = modinfoFile.GetModinfo();

            Assert.Equal(new Version(1, 1, 1), data.Version);
            Assert.Single(data.Dependencies);
        }

        [Fact]
        public void TestVariantFile4()
        {
            var fs = new MockFileSystem();
            var variantMain = new ModinfoVariantFile(ModinfoDataUtils.CreateVariantMainFile(fs, "mods/A"));
            var variant = ModinfoDataUtils.CreateVariantFile(fs, "mods/A");
            Assert.Throws<ModinfoException>(() => new ModinfoVariantFile(variant, variantMain));
        }

        [Fact]
        public void TestCtorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MainModinfoFile(null!));
            Assert.Throws<ArgumentNullException>(() => new ModinfoVariantFile(null!));
        }
    }
}