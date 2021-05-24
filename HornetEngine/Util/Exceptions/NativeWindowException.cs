using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Util.Exceptions
{
    class NativeWindowException : Exception
    {
        public NativeWindowException(string message) : 
            base($"Encountered an exception in NativeWindow: {message}") { }
    }

    class NativeWindowCreationException : Exception
    {
        public NativeWindowCreationException(string message) :
            base($"Failed to create native window: {message}") { }
    }
}
