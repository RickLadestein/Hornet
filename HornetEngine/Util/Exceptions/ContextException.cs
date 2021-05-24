using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Util.Exceptions
{
    class ContextInitException : Exception
    {
        public ContextInitException(object context, string except) :
            base($"Context {context} initialisation failed: {except}") {}
    }

    class ContextException : Exception
    {
        public ContextException(object context, string except) :
            base($"Context {context} encountered an exception: {except}") {}
    }
}
