using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Graphics
{
    public struct MaterialDescriptor
    {
        public GlmSharp.vec3 Color_ambient;
        public GlmSharp.vec3 Color_diffuse;
        public GlmSharp.vec3 Color_specular;
        public float Opacity;

        public string Diffuse_map;
        public string Ambient_map;
        public string Dispersion_map;
        public static MaterialDescriptor ParseFrom(Assimp.Material material)
        {

            MaterialDescriptor output = new MaterialDescriptor()
            {
                Color_ambient = new GlmSharp.vec3(material.ColorAmbient.R, material.ColorAmbient.G, material.ColorAmbient.B),
                Color_diffuse = new GlmSharp.vec3(material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B),
                Color_specular = new GlmSharp.vec3(material.ColorSpecular.R, material.ColorSpecular.G, material.ColorSpecular.B),
                Opacity = material.Opacity,

                Diffuse_map = CutTexFilepath(material.TextureDiffuse.FilePath),
                Ambient_map = CutTexFilepath(material.TextureAmbient.FilePath),
                Dispersion_map = CutTexFilepath(material.TextureDisplacement.FilePath)
            };
            return output;
        }

        private static string CutTexFilepath(string filepath)
        {
            if(filepath == null || filepath == string.Empty)
            {
                return string.Empty;
            }

            string[] tokens = filepath.Split("/");
            if(tokens.Length == 1)
            {
                return filepath;
            } else
            {
                return tokens[tokens.Length - 1];
            }
        }
    }
}
