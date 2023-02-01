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
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> System text json (#134)
=======
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd
        yield return new object[] {"1", new SemVersion(1, 0, 0)};
        yield return new object[] {"1.0", new SemVersion(1, 0, 0)};
        yield return new object[] {"1.0.0", new SemVersion(1, 0, 0)};
        yield return new object[] {"1.0.0.0", new SemVersion(1, 0, 0)};
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd

        yield return new object[] {"1.0.0.1", new SemVersion(1, 0, 0)};
        yield return new object[] {"1.0.0.1-pre1", new SemVersion(1, 0, 0, "pre1")};

        yield return new object[] {"1.0.0.1+2", new SemVersion(1, 0, 0, null, "2")};

        yield return new object[] { "1-pre1", new SemVersion(1, 0, 0, "pre1") };
        yield return new object[] { "1-pre1+1", new SemVersion(1, 0, 0, "pre1", "1") };
<<<<<<< HEAD
=======
        yield return new object[] {"1", new Version(1, 0, 0)};
        yield return new object[] {"1.0", new Version(1, 0, 0)};
        yield return new object[] {"1.0.0", new Version(1, 0, 0)};
        yield return new object[] {"1.0.0.0", new Version(1, 0, 0)};
=======
>>>>>>> System text json (#134)

        yield return new object[] {"1.0.0.1", new SemVersion(1, 0, 0)};
        yield return new object[] {"1.0.0.1-pre1", new SemVersion(1, 0, 0, "pre1")};

        yield return new object[] {"1.0.0.1+2", new SemVersion(1, 0, 0, null, "2")};

<<<<<<< HEAD
        yield return new object[] { "1-pre1", new Version(1, 0, 0, "pre1") };
        yield return new object[] { "1-pre1+1", new Version(1, 0, 0, "pre1", "1") };
>>>>>>> to c# 10 namespaces
=======
        yield return new object[] { "1-pre1", new SemVersion(1, 0, 0, "pre1") };
        yield return new object[] { "1-pre1+1", new SemVersion(1, 0, 0, "pre1", "1") };
>>>>>>> System text json (#134)
=======
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd
    }



    [Theory]
    [MemberData(nameof(GetTestData))]
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
    public void TestSanitized(string? inputData, SemVersion? semanticVersion)
=======
    public void TestSanitized(string? inputData, Version? semanticVersion)
>>>>>>> to c# 10 namespaces
=======
    public void TestSanitized(string? inputData, SemVersion? semanticVersion)
>>>>>>> System text json (#134)
=======
    public void TestSanitized(string? inputData, SemVersion? semanticVersion)
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd
    {
        var newVersion = SemVerHelper.CreateSanitizedVersion(inputData);
        if (semanticVersion is not null)
            _output.WriteLine(semanticVersion.ToString());
        Assert.Equal(semanticVersion, newVersion);
    }
}