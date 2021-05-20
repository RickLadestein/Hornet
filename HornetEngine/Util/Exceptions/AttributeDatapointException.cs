using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Util.Exceptions
{
    public class AttributeDatapointException : Exception
    {
        public AttributeDatapointException() : base("Datapoint size and buffer size are not alligned") {}
    }
}
