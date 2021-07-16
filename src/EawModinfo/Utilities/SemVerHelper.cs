﻿using System;
using Version = SemanticVersioning.Version;

namespace EawModinfo.Utilities
{
    internal static class SemVerHelper
    {
        private static readonly char[] DelimiterChars = {'-', '+'};

        public static Version? CreateSanitizedVersion(string? version)
        {
            if (string.IsNullOrEmpty(version))
                return null;

            string[] strArray = version!.Split('.');
            if (strArray.Length == 3)
                return Version.Parse(version);
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
            return Version.Parse(newSemVerString);
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
}