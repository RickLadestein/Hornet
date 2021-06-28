using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Util.Exceptions
{
    class NativeWindowException : Exception
    {
        /// <summary>
        /// A NativeWindowException exception class
        /// </summary>
        public NativeWindowException(string message) : 
            base($"Encountered an exception in NativeWindow: {message}") { }
    }

    class NativeWindowCreationException : Exception
    {
        /// <summary>
        /// A NativeWindowCreationException exception class
        /// </summary>
        public NativeWindowCreationException(string message) :
            base($"Failed to create native window: {message}") { }
    }
}
