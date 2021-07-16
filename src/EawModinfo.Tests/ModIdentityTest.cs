using System.Collections.Generic;
using EawModinfo.Model;
using EawModinfo.Spec;
using SemanticVersioning;
using Xunit;

namespace EawModinfo.Tests
{
    public class ModIdentityTest
    {
        [Fact]
        public void ModIdentityEqualCheck()
        {
            IModIdentity i1 = new ModinfoData("A");
            IModIdentity i2 = new ModIdentity("A");

            Assert.Equal(i1, i2);

            IModIdentity i3 = new ModinfoData("A") { Version = new Version(1, 1, 1) };
            IModIdentity i4 = new ModIdentity("A") { Version = new Version(1, 1, 1) };

            Assert.Equal(i3, i4);
            Assert.NotEqual(i3, i1);

            IModIdentity i5 = new ModinfoData("B");
            Assert.NotEqual(i1, i5);

            var d1 = new ModReference { Type = ModType.Default, Identifier = "A" };
            var d2 = new ModReference { Type = ModType.Default, Identifier = "A" };
            var d3 = new ModReference { Type = ModType.Default, Identifier = "B" };

            Assert.Equal(d1, d2);

            IModIdentity i6 = new ModinfoData("A") { Dependencies = new DependencyList(new IModReference[] { d1, d3 }, DependencyResolveLayout.FullResolved) };
            IModIdentity i7 = new ModIdentity("A") { Dependencies = new DependencyList(new IModReference[] { d2, d3 }, DependencyResolveLayout.FullResolved) };
            IModIdentity i8 = new ModIdentity("A") { Dependencies = new DependencyList(new IModReference[] { d2 }, DependencyResolveLayout.FullResolved) };
            IModIdentity i9 = new ModIdentity("A") { Dependencies = new DependencyList(new IModReference[] { d3, d1 }, DependencyResolveLayout.FullResolved) };
            IModIdentity i10 = new ModinfoData("A") { Dependencies = new DependencyList(new IModReference[] { d1 }, DependencyResolveLayout.FullResolved) };

            Assert.Equal(i6, i7);
            Assert.NotEqual(i6, i8);
            Assert.NotEqual(i6, i9);
            Assert.Equal(i8, i10);
        }
    }
}