using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EawModinfo.Tests")]


#if !NET
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
#pragma warning disable CS1591
    public class IsExternalInit { }
#pragma warning restore CS1591
}
#endif