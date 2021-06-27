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
        public MultiTexture Textures { get; private set; }
        public ShaderProgram Shader { get; private set; }

        public MaterialDescriptor Material { get; set; }

        public MaterialComponent()
        {
            this.Textures = new MultiTexture();
            this.Shader = null;
        }

        public MaterialComponent(ShaderProgram prg)
        {
            if(prg == null)
            {
                throw new ArgumentNullException("ShaderProgram");
            }
            this.Textures = new MultiTexture();
            this.Shader = prg;
        }

        public void SetShaderFromId(String matId)
        {
            ShaderProgram result = ShaderResourceManager.Instance.GetResource(matId);
            if(result == null)
            {
                throw new MissingResourceException(matId);
            }
            this.Shader = result;
        }

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
