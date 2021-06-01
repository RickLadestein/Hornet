using HornetEngine.Graphics;
using HornetEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    public class TextureComponent : Component
    {
        public MultiTexture Textures { get; private set; }

        public TextureComponent()
        {
            Textures = new MultiTexture();
        }

        public void SetTextureUnit(String tex_identifier, HTextureUnit layer)
        {
            Texture tex = TextureResourceManager.GetInstance().GetResource(tex_identifier);
            if(tex == null)
            {
                throw new Exception($"Could not find resource: {tex_identifier}");
            } else
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
            for(int i = 0; i < Textures.textures.Length; i++)
            {
                count += Textures.textures[i] == null ? 1 : 0;
            }
            return "TextureComponent {\n" +
                $"\tActive Tex Count: {count}" +
                "}";
        }
    }
}
