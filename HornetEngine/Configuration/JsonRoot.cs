using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Configuration
{
    public class JsonRoot
    {
        public JsonData data { get; set; }
        public List<JsonRule> rules { get; set; }
    }
}
