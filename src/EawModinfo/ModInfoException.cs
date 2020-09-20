using System;

namespace EawModinfo
{
    /// <summary>
    /// Base exception which gets thrown on high-level errors while working with modinfo data.
    /// </summary>
    public class ModinfoException : Exception
    {
        public ModinfoException()
        {
        }

        public ModinfoException(string message) : base(message)
        {
        }

        internal ModinfoException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}