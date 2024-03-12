using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EawModinfo;

internal static class ThrowHelper
{
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
    public static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
            ThrowNullOrEmptyException(argument, paramName);
    }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

    [DoesNotReturn]
    private static void ThrowNullOrWhiteSpaceException(string? argument, string? paramName = null)
    {
        if (argument is null)
            throw new ArgumentNullException(paramName);
        throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.", paramName);
    }

    [DoesNotReturn]
    private static void ThrowNullOrEmptyException(string? argument, string? paramName)
    {
        if (argument is null)
            throw new ArgumentNullException(paramName);
        throw new ArgumentException("The value cannot be an empty string.", paramName);
    }
}