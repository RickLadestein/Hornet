using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Graphics
{

    public enum HTextureUnit : uint
    {
        Unit_0,
        Unit_1,
        Unit_2,
        Unit_3,
        Unit_4,
        Unit_5,
        Unit_6,
        Unit_7
    }
    public class Texture
    {
        public uint Handle;


        public void Bind()
        {

        }

        public void Unbind()
        {

        }
    }

    public class MultiTexture
    {
        public static readonly uint MAX_TEXTURE_LAYERS = 8;
        public static MultiTexture current_bound;
        public Texture[] textures;

        public MultiTexture()
        {
            textures = new Texture[MAX_TEXTURE_LAYERS];
        }

        public void Bind()
        {
            if(current_bound != null)
            {
                current_bound.Unbind();
            }
            current_bound = this;
            uint min = (uint)GLEnum.Texture0;
            uint max = (uint)GLEnum.Texture0 + (MAX_TEXTURE_LAYERS - 1);
            uint tex_id = 0;
            for (uint i = min; i < max; i++)
            {
                NativeWindow.GL.ActiveTexture((GLEnum)i);
                if(this.textures[tex_id] != null)
                {
                    NativeWindow.GL.BindTexture(GLEnum.Texture2D, textures[tex_id].Handle);
                }
                tex_id += 1;
            }
            NativeWindow.GL.ActiveTexture(GLEnum.Texture0);
        }

        public void Unbind()
        {
            if(current_bound == this)
            {
                uint min = (uint)GLEnum.Texture0;
                uint max = (uint)GLEnum.Texture0 + (MAX_TEXTURE_LAYERS - 1);
                for(uint i = min; i < max; i++)
                {
                    NativeWindow.GL.ActiveTexture((GLEnum)i);
                    NativeWindow.GL.BindTexture(GLEnum.Texture2D, 0);
                }
                current_bound = null;
            }
        }

        public void SetTextureUnit(Texture tex, HTextureUnit layer)
        {
            uint loc = (uint)layer;
            if(loc >= MAX_TEXTURE_LAYERS)
            {
                throw new IndexOutOfRangeException("Texture unit was out of the range of the MultiTexture texture array");
            }
            this.textures[(uint)layer] = tex;
        }

        public void ClearTextureUnit(HTextureUnit layer)
        {
            uint loc = (uint)layer;
            if (loc >= MAX_TEXTURE_LAYERS)
            {
                throw new IndexOutOfRangeException("Texture unit was out of the range of the MultiTexture texture array");
            }
            this.textures[(uint)layer] = null;
        }
    }
}
