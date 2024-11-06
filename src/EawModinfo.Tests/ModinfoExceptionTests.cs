using System;
using Xunit;

namespace EawModinfo.Tests;

public static class ModinfoExceptionTests
{
    [Fact]
    public static void Ctor_Empty()
    {
        var exception = new ModinfoException();
        AssertException(exception, validateMessage: false);
    }

    [Fact]
    public static void Ctor_String()
    {
        var message = "modinfo error";
        var exception = new ModinfoException(message);
        AssertException(exception, message: message);
    }

    [Fact]
    public static void Ctor_String_Exception()
    {
        var message = "modinfo error";
        var innerException = new Exception("Inner exception");
        var exception = new ModinfoException(message, innerException);
        AssertException(exception, innerException: innerException, message: message);
    }

    internal static void AssertException(Exception e,
        Exception? innerException = null,
        string? message = null,
        string? stackTrace = null,
        bool validateMessage = true)
    {
        Assert.Equal(innerException, e.InnerException);
        if (validateMessage)
            Assert.Equal(message, e.Message);
        else
            Assert.NotNull(e.Message);
        Assert.Equal(stackTrace, e.StackTrace);
    }
}