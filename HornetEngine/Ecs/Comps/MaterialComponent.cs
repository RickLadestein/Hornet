using System;
using System.Collections.Generic;
using System.Text;
using HornetEngine.Graphics;
using HornetEngine.Util;
using HornetEngine.Util.Exceptions;

namespace HornetEngine.Ecs
{
    public class MaterialComponent : Component
    {
        /// <summary>
        /// The Textures of the object
        /// </summary>
        public MultiTexture Textures { get; private set; }
        
        /// <summary>
        /// The shader of the object
        /// </summary>
        public ShaderProgram Shader { get; private set; }

        /// <summary>
        /// The physical properties of a material
        /// </summary>
        public MaterialDescriptor Material { get; set; }

        /// <summary>
        /// The constructor of a MaterialComponent
        /// </summary>
        public MaterialComponent()
        {
            this.Textures = new MultiTexture();
            this.Shader = null;
        }

        /// <summary>
        /// The constructor of a MaterialComponent
        /// </summary>
        /// <param name="prg">The shaderprogram used for this component</param>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException</exception>
        public MaterialComponent(ShaderProgram prg)
        {
            if(prg == null)
            {
                throw new ArgumentNullException("ShaderProgram");
            }
            this.Textures = new MultiTexture();
            this.Shader = prg;
        }

        /// <summary>
        /// A function which can be used to set a shader
        /// </summary>
        /// <param name="matId">A string containing the ID of the shader</param>
        /// <exception cref="MissingResourceException">Throws a MissingResourceException</exception>
        public void SetShaderFromId(String matId)
        {
            ShaderProgram result = ShaderResourceManager.Instance.GetResource(matId);
            if(result == null)
            {
                throw new MissingResourceException(matId);
            }
            this.Shader = result;
        }

        /// <summary>
        /// A function which can be used to set a texture unit
        /// </summary>
        /// <param name="tex_identifier">A string containing the ID of the texture</param>
        /// <param name="layer">The allocated texture unit within the GPU</param>
        /// <exception cref="Exception">Throws an Exception</exception>
        public void SetTextureUnit(String tex_identifier, HTextureUnit layer)
        {
            Texture tex = TextureResourceManager.Instance.GetResource(tex_identifier);
            if (tex == null)
            {
                throw new Exception($"Could not find resource: {tex_identifier}");
            }
            else
            {
                Textures.SetTextureUnit(tex, layer);
            }
        }

        /// <summary>
        /// A function which can be used to clear the given texture unit
        /// </summary>
        /// <param name="layer">The texture unit which should be cleared</param>
        public void ClearTextureUnit(HTextureUnit layer)
        {
            Textures.ClearTextureUnit(layer);
        }

        public override string ToString()
        {
            int count = 0;
            for (int i = 0; i < Textures.textures.Length; i++)
            {
                count += Textures.textures[i] == null ? 1 : 0;
            }
            return "MaterialComponent {\n" +
                $"\tShaderProgram: {Shader.Handle}" +
                $"\tActive Tex Count: {count}" +
                "}";
        }
    }
}
