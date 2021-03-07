using System;

namespace EawModinfo
{
    /// <summary>
    /// Exceptions which gets thrown when parsing modinfo data fails, based on the available specification.
    /// </summary>
    public sealed class ModinfoParseException : ModinfoException
    {
        /// <inheritdoc/>
        public ModinfoParseException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public ModinfoParseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}