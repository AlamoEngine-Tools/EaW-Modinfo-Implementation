using System.Collections.Generic;
using EawModinfo.Utilities;
using Semver;
using Xunit;
using Xunit.Abstractions;

namespace EawModinfo.Tests;

public class SemVerHelperTests(ITestOutputHelper output)
{
    public static IEnumerable<object?[]> GetTestData()
    {
        yield return [null, null];
        yield return ["1", new SemVersion(1, 0, 0)];
        yield return ["1.0", new SemVersion(1, 0, 0)];
        yield return ["1.0.0", new SemVersion(1, 0, 0)];
        yield return ["1.0.0.0", new SemVersion(1, 0, 0)];
        yield return ["1.0.0.0.1-pre1", SemVersion.ParsedFrom(1, 0, 0, "pre1")];

        yield return ["1.2.3.4", new SemVersion(1, 2, 3)];
        yield return ["1.2.3.4-pre1", SemVersion.ParsedFrom(1,2,3, "pre1")];

        yield return ["1.2.3.4+2", SemVersion.ParsedFrom(1, 2, 3, string.Empty, "2")];

        yield return ["1-pre1", SemVersion.ParsedFrom(1, 0, 0, "pre1")];
        yield return ["1-pre1+1", SemVersion.ParsedFrom(1, 0, 0, "pre1", "1")];
    }



    [Theory]
    [MemberData(nameof(GetTestData))]
    public void CreateSanitizedVersion(string? inputData, SemVersion? semanticVersion)
    {
        var newVersion = SemVerHelper.CreateSanitizedVersion(inputData);
        if (semanticVersion is not null)
            output.WriteLine(semanticVersion.ToString());
        Assert.Equal(semanticVersion, newVersion);
    }
}