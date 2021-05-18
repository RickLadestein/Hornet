using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HornetEngine.Graphics
{
    public abstract class Attribute
    {
        /// <summary>
        /// Attribute data that is stored within is Attribute
        /// </summary>
        public List<byte> byte_data { get; protected set; }
        public string Name { get; protected set; }
        public AttributeType Base_type { get; protected set; }
        public uint Components { get; protected set; }
        public uint Base_type_size { get; protected set; }
        public uint Datapoints { get; protected set; }

        protected Attribute()
        {
            this.byte_data = new List<byte>();
            this.Name = "";
            this.Base_type = AttributeType.FLOAT;
            this.Components = 4;
            this.Base_type_size = sizeof(float);
            this.Datapoints = 0;
        }

        public void ClearData()
        {
            this.byte_data.Clear();
        }

    }
}
