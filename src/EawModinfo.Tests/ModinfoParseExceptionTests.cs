using System;
using Xunit;

namespace EawModinfo.Tests;

public static class ModinfoParseExceptionTests
{
    [Fact]
    public static void Ctor_String()
    {
        var message = "modinfo error";
        var exception = new ModinfoParseException(message);
        ModinfoExceptionTests.AssertException(exception, message: message);
    }

    [Fact]
    public static void Ctor_String_Exception()
    {
        var message = "modinfo error";
        var innerException = new Exception("Inner exception");
        var exception = new ModinfoParseException(message, innerException);
        ModinfoExceptionTests.AssertException(exception, innerException: innerException, message: message);
    }
}