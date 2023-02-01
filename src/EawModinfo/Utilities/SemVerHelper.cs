using Semver;
using System;

namespace EawModinfo.Utilities;

internal static class SemVerHelper
{
    private static readonly char[] DelimiterChars = {'-', '+'};

<<<<<<< HEAD
<<<<<<< HEAD
    public static SemVersion? CreateSanitizedVersion(string? version)
=======
    public static Version? CreateSanitizedVersion(string? version)
>>>>>>> to c# 10 namespaces
=======
    public static SemVersion? CreateSanitizedVersion(string? version)
>>>>>>> System text json (#134)
    {
        if (string.IsNullOrEmpty(version))
            return null;

        string[] strArray = version!.Split('.');
        if (strArray.Length == 3)
<<<<<<< HEAD
<<<<<<< HEAD
            return SemVersion.Parse(version, SemVersionStyles.Any);
=======
            return Version.Parse(version);
>>>>>>> to c# 10 namespaces
=======
            return SemVersion.Parse(version, SemVersionStyles.Any);
>>>>>>> System text json (#134)
        if (strArray.Length >= 5)
            throw new InvalidOperationException();

        ExtractReleaseAndBuildString(ref strArray[strArray.Length - 1], out var releaseAndBuild);

        string[] versionDigits = new string[3];
        for (var i = 0; i < versionDigits.Length; i++)
        {
            try
            {
                versionDigits[i] = strArray[i];
            }
            catch (IndexOutOfRangeException)
            {
                versionDigits[i] = "0";
            }
        }

        string newSemVerString = string.Join(".", versionDigits) + releaseAndBuild;
<<<<<<< HEAD
<<<<<<< HEAD
        return SemVersion.Parse(newSemVerString, SemVersionStyles.Any);
=======
        return Version.Parse(newSemVerString);
>>>>>>> to c# 10 namespaces
=======
        return SemVersion.Parse(newSemVerString, SemVersionStyles.Any);
>>>>>>> System text json (#134)
    }

    private static void ExtractReleaseAndBuildString(ref string input, out string? releaseAndBuild)
    {
        releaseAndBuild = null;
        var releaseIndex = input.IndexOfAny(DelimiterChars);
        if (releaseIndex == -1)
            return;
        releaseAndBuild = input.Substring(releaseIndex);
        input = input.Remove(releaseIndex);
    }
}