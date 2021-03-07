using System;
using System.IO;
using EawModinfo.File;
using EawModinfo.Model;
using EawModinfo.Spec;
using NuGet.Versioning;
using Xunit;

namespace EawModinfo.Tests
{
    public class ModinfoFileTests
    {
        internal static IModinfoFile GetMain()
        {
            var solutionPath = GetSolutionPath();
            var file = new FileInfo(Path.Combine(solutionPath, "test/Files/modinfo.json"));
            return new MainModinfoFile(file);
        }

        internal static IModinfoFile GetVariantStandalone()
        {
            var solutionPath = GetSolutionPath();
            var file = new FileInfo(Path.Combine(solutionPath, "test/Files/variantMain-modinfo.json"));
            return new ModinfoVariantFile(file);
        }

        internal static IModinfoFile GetVariantMerged()
        {
            var main = GetMain();
            var solutionPath = GetSolutionPath();
            var file = new FileInfo(Path.Combine(solutionPath, "test/Files/variant-modinfo.json"));
            return new ModinfoVariantFile(file, main);
        }

        [Fact]
        public void TestMainFile()
        { 
            var solutionPath = GetSolutionPath();
            var file = new FileInfo(Path.Combine(solutionPath, "test/Files/modinfo.json"));
            IModinfoFile modinfoFile = new MainModinfoFile(file);

            Assert.Equal(ModinfoFileKind.MainFile,modinfoFile.FileKind);

            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
            Assert.Null(Record.ExceptionAsync(modinfoFile.GetModinfoAsync).Result);
        }

        [Fact]
        public void TestVariantFile1()
        {
            var solutionPath = GetSolutionPath();
            var file = new FileInfo(Path.Combine(solutionPath, "test/Files/variantMain-modinfo.json"));
            IModinfoFile modinfoFile = new ModinfoVariantFile(file);

            Assert.Equal(ModinfoFileKind.VariantFile, modinfoFile.FileKind);

            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
            Assert.Null(Record.ExceptionAsync(modinfoFile.GetModinfoAsync).Result);
        }

        [Fact]
        public void TestVariantFile2()
        {
            var solutionPath = GetSolutionPath();

            var main = GetMain();

            var file = new FileInfo(Path.Combine(solutionPath, "test/Files/variant-modinfo.json"));
            IModinfoFile modinfoFile = new ModinfoVariantFile(file, main);

            Assert.Equal(ModinfoFileKind.VariantFile, modinfoFile.FileKind);

            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
            Assert.Null(Record.ExceptionAsync(modinfoFile.GetModinfoAsync).Result);

            var data = modinfoFile.GetModinfo();

            Assert.Equal(new SemanticVersion(1,1,1, "BETA"), data.Version);
            Assert.Single(data.Custom);
            Assert.Single(data.Languages);
        }

        [Fact]
        public void TestVariantFile3()
        {
            var main = new ModinfoData { Name = "Main", Version = new SemanticVersion(1, 1, 1) };
            main.Dependencies.Add(new ModReference { Identifier = "123", Type = ModType.Workshops });

            var solutionPath = GetSolutionPath();
            var file = new FileInfo(Path.Combine(solutionPath, "test/Files/variant-modinfo.json"));
            IModinfoFile modinfoFile = new ModinfoVariantFile(file, main);

            Assert.Equal(ModinfoFileKind.VariantFile, modinfoFile.FileKind);

            Assert.Null(Record.Exception(modinfoFile.ValidateFile));
            Assert.Null(Record.Exception(modinfoFile.GetModinfo));
            Assert.Null(Record.ExceptionAsync(modinfoFile.GetModinfoAsync).Result);

            var data = modinfoFile.GetModinfo();

            Assert.Equal(new SemanticVersion(1, 1, 1), data.Version);
            Assert.Single(data.Dependencies);
        }

        [Fact]
        public void TestVariantFile4()
        {
            var variant = GetVariantStandalone();

            var solutionPath = GetSolutionPath();
            var file = new FileInfo(Path.Combine(solutionPath, "test/Files/variant-modinfo.json"));
            Assert.Throws<ModinfoException>(() => new ModinfoVariantFile(file, variant));
        }

        [Fact]
        public void TestCtorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MainModinfoFile(null!));
            Assert.Throws<ArgumentNullException>(() => new ModinfoVariantFile(null!));
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