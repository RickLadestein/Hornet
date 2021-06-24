using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    public class RadialLightComponent : Component
    {
        public GlmSharp.vec3 Albedo { get; set; }
        public float Range { get; set; }
        public float Intensity { get; set; }


        /// <summary>
        /// Sets the Albedo color from color temperature
        /// </summary>
        /// <param name="color_temp">Color temperature in Kelvin</param>
        public void SetAlbedoFromTemperature(uint color_temp)
        {
            float norm_temp = color_temp / 100;
            float r_val, g_val, b_val;

            //Calculate Red value
            if(norm_temp <= 66)
            {
                r_val = 1.0f;
            } else
            {
                float norm_red = norm_temp - 60;
                r_val = 329.698727446f * MathF.Pow(norm_red, -0.1332047592f);
                r_val = OpenTK.Mathematics.MathHelper.Clamp(r_val, 0, 255);
            }

            //Calculate Green value
            if(norm_temp <= 66)
            {
                g_val = 99.4708025861f * MathF.Log10(norm_temp) - 161.1195681661f;
                
            } else
            {
                float norm_green = norm_temp - 60;
                g_val = 288.1221695283f * MathF.Pow(norm_green, -0.0755148492f);
            }
            OpenTK.Mathematics.MathHelper.Clamp(g_val, 0, 255);

            //Calculate Blue value
            if(norm_temp >= 66)
            {
                b_val = 255;
            } else
            {
                float norm_blue = norm_temp - 10;
                b_val = 138.5177312231f * MathF.Log10(norm_blue) - 305.0447927307f;
            }
            OpenTK.Mathematics.MathHelper.Clamp(b_val, 0, 255);

            this.Albedo = new GlmSharp.vec3()
            {
                x = r_val * (1.0f / 255.0f),
                y = g_val * (1.0f / 255.0f),
                z = b_val * (1.0f / 255.0f)
            };
        }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
