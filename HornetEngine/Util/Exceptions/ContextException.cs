using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Util.Exceptions
{
    class ContextInitException : Exception
    {
        /// <summary>
        /// A ContextInitException exception class
        /// </summary>
        public ContextInitException(object context, string except) :
            base($"Context {context} initialisation failed: {except}") {}
    }

    /// <summary>
    /// A ContextException exception class
    /// </summary>
    class ContextException : Exception
    {
        public ContextException(object context, string except) :
            base($"Context {context} encountered an exception: {except}") {}
    }
}
