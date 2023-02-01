using System.Collections.Generic;
using EawModinfo.Utilities;
using Semver;
using Xunit;
using Xunit.Abstractions;

namespace EawModinfo.Tests;

public class SemVerHelperTests
{
    private readonly ITestOutputHelper _output;

    public SemVerHelperTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public static IEnumerable<object?[]> GetTestData()
    {
        yield return new object?[] { null, null };
        yield return new object[] { "1", new SemVersion(1, 0, 0) };
        yield return new object[] { "1.0", new SemVersion(1, 0, 0) };
        yield return new object[] { "1.0.0", new SemVersion(1, 0, 0) };
        yield return new object[] { "1.0.0.0", new SemVersion(1, 0, 0) };

        yield return new object[] { "1.0.0.1", new SemVersion(1, 0, 0) };
        yield return new object[] { "1.0.0.1-pre1", new SemVersion(1, 0, 0, "pre1") };

        yield return new object[] { "1.0.0.1+2", new SemVersion(1, 0, 0, null, "2") };

        yield return new object[] { "1-pre1", new SemVersion(1, 0, 0, "pre1") };
        yield return new object[] { "1-pre1+1", new SemVersion(1, 0, 0, "pre1", "1") };
    }



    [Theory]
    [MemberData(nameof(GetTestData))]
    public void TestSanitized(string? inputData, SemVersion? semanticVersion)
    {
        var newVersion = SemVerHelper.CreateSanitizedVersion(inputData);
        if (semanticVersion is not null)
            _output.WriteLine(semanticVersion.ToString());
        Assert.Equal(semanticVersion, newVersion);
    }
}