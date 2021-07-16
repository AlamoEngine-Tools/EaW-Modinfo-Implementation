using System;

namespace EawModinfo
{
    /// <summary>
    /// Base exception which gets thrown on high-level errors while working with modinfo data.
    /// </summary>
    public class ModinfoException : Exception
    {
        /// <inheritdoc/>
        public ModinfoException()
        {
        }

        /// <inheritdoc/>
        public ModinfoException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        internal ModinfoException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}