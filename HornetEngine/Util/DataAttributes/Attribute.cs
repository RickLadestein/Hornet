using HornetEngine.Util.Exceptions;
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

        /// <summary>
        /// The name of the Attribute
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The type of the attribute
        /// </summary>
        public AttributeType Base_type { get; protected set; }

        /// <summary>
        /// The components of the attribute
        /// </summary>
        public uint Components { get; protected set; }

        /// <summary>
        /// The size of the base type
        /// </summary>
        public uint Base_type_size { get; protected set; }

        protected Attribute()
        {
            this.byte_data = new List<byte>();
            this.Name = "";
            this.Base_type = AttributeType.FLOAT;
            this.Components = 4;
            this.Base_type_size = sizeof(float);
        }

        /// <summary>
        /// Gets the amount of datapoints stored in the internal data buffer.
        /// </summary>
        /// <returns>The amount of datapoints stored in the internal data buffer</returns>
        public int GetDatapointCount()
        {
            if(!this.ValidateDataIntegrity())
            {
                throw new AttributeDatapointException();
            }
            return byte_data.Count / ((int)Base_type_size * (int)Components);
        }

        /// <summary>
        /// Checks if the amount of data in the buffer 
        /// </summary>
        /// <returns>True if all data is valid, False if there was no data found or data does not match component count</returns>
        public bool ValidateDataIntegrity()
        {
            if(this.byte_data.Count == 0)
            {
                return false;
            }

            int mod = (byte_data.Count / (int)Base_type_size) % (int)Components;
            if (mod > 0)
            {
                return false;
            } else
            {
                return true;
            }
        }

        /// <summary>
        /// A function which clears the attribute data
        /// </summary>
        public void ClearData()
        {
            this.byte_data.Clear();
        }
    }
}
