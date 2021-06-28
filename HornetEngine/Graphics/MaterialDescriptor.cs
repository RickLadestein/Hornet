using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Graphics
{
    public struct MaterialDescriptor
    {
        /// <summary>
        /// The ambient color
        /// </summary>
        public GlmSharp.vec3 Color_ambient;

        /// <summary>
        /// The diffuse color
        /// </summary>
        public GlmSharp.vec3 Color_diffuse;

        /// <summary>
        /// The specular color
        /// </summary>
        public GlmSharp.vec3 Color_specular;

        /// <summary>
        /// The opacity
        /// </summary>
        public float Opacity;

        public string Ambient_map;
        public string Normal_map;
        public string Specular_map;
        public static MaterialDescriptor ParseFrom(Assimp.Material material)
        {
            MaterialDescriptor output = new MaterialDescriptor()
            {
                Color_ambient = new GlmSharp.vec3(material.ColorAmbient.R, material.ColorAmbient.G, material.ColorAmbient.B),
                Color_diffuse = new GlmSharp.vec3(material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B),
                Color_specular = new GlmSharp.vec3(material.ColorSpecular.R, material.ColorSpecular.G, material.ColorSpecular.B),
                Opacity = material.Opacity,

                Ambient_map = CutTexFilepath(material.TextureAmbient.FilePath),
                Normal_map = CutTexFilepath(material.TextureDisplacement.FilePath),
                Specular_map = CutTexFilepath(material.TextureSpecular.FilePath)
            };
            return output;
        }

        /// <summary>
        /// A function which can be used to modify string paths
        /// </summary>
        /// <param name="filepath">The given file path</param>
        /// <returns>The modified file path</returns>
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
