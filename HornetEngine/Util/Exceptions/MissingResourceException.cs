using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Util.Exceptions
{
    public class MissingResourceException : Exception
    {
        public MissingResourceException(String resource_id) 
            : base($"The resource with id {resource_id} was not found in the ResourceManager") { }
    }
}
