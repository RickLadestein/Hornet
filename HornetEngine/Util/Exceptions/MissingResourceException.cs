using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Util.Exceptions
{
    public class MissingResourceException : Exception
    {
        /// <summary>
        /// A MissingResourceException exception class
        /// </summary>
        public MissingResourceException(String resource_id) 
            : base($"The resource with id {resource_id} was not found in the ResourceManager") { }
    }
}
