using System;

namespace EawModinfo;

/// <summary>
/// Exceptions which gets thrown when parsing modinfo data fails, based on the available specification.
/// </summary>
public sealed class ModinfoParseException : ModinfoException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModinfoException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ModinfoParseException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModinfoException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">
    /// The exception that is the cause of the current exception.
    /// If the innerException parameter is not a null reference,
    /// the current exception is raised in a catch block that handles the inner exception.
    /// </param>
    public ModinfoParseException(string message, Exception inner) : base(message, inner)
    {
    }
}