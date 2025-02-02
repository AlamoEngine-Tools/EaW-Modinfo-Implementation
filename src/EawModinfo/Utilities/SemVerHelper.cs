using System;
using Semver;

namespace AET.Modinfo.Utilities;

internal static class SemVerHelper
{
    private static readonly char[] DelimiterChars = ['-', '+'];

    public static SemVersion? CreateSanitizedVersion(string? version)
    {
        if (string.IsNullOrEmpty(version))
            return null;

        var strArray = version!.Split('.');
        if (strArray.Length == 3)
            return SemVersion.Parse(version, SemVersionStyles.Any);

        ExtractReleaseAndBuildString(ref strArray[^1], out var releaseAndBuild);

        var versionDigits = new string[3];
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

        var newSemVerString = string.Join(".", versionDigits) + releaseAndBuild;
        return SemVersion.Parse(newSemVerString, SemVersionStyles.Any);
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