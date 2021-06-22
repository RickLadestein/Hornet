using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using HornetEngine.Util;
using System.Runtime.InteropServices;

namespace HornetEngine.Graphics
{
    /// <summary>
	/// Enum specifying the texture wrapping settings
	/// </summary>
	public enum TextureWrapSetting
    {
        /// <summary>
        /// Use repeat for outside mode of bounds texture mapping
        /// </summary>
        REPEAT = GLEnum.Repeat,

		/// <summary>
		/// Use mirrored repeat mode for outside of bounds texture mapping
		/// </summary>
		MIRRORED_REPEAT = GLEnum.MirroredRepeat,

		/// <summary>
		/// Use edge clamp mode for outside of bounds texture mapping
		/// </summary>
		EDGE_CLAMP = GLEnum.ClampToEdge,

		/// <summary>
		/// Use border clamp mode for outside of bounds texture mapping
		/// </summary>
		BORDER_CLAMP = GLEnum.ClampToBorder
    };

    /// <summary>
    /// Enum specifying the mini/magnification setting
    /// </summary>
    public enum MinMagSetting
    {
        /// <summary>
        /// Use nearest neighbour pixel filtering for mini/magnification 
        /// </summary>
        NEAREST = GLEnum.Nearest,

		/// <summary>
		/// Use linear pixel filtering for mini/magnification 
		/// </summary>
		LINEAR = GLEnum.Linear
    };

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

    public enum TextureStatus
    {
        /// <summary>
        /// Texture is unitialised and cannot be used yet
        /// </summary>
        UNINITIALISED,

        /// <summary>
        /// Texture is aquiring an OpenGL texture handle
        /// </summary>
        AQUIRING_HANDLE,

        /// <summary>
        /// Texture is importing image
        /// </summary>
        IMPORTING_IMAGE,

        /// <summary>
        /// Texture is loading image in the GPU
        /// </summary>
        LOADING_IMAGE,

        /// <summary>
        /// Texture is ready
        /// </summary>
        READY
    }
    public class Texture : IDisposable
    {
        public uint Handle { get; private set; }
        public TextureStatus Status { get; private set; }
        public String Error { get; private set; }

        public TextureWrapSetting Wrap { get; private set; }

        public MinMagSetting Filter { get; private set; }


        public Texture(String dir_id, String imfile, bool mipmap)
        {
            this.Status = TextureStatus.UNINITIALISED;
            this.InitDefaults();

            this.Status = TextureStatus.AQUIRING_HANDLE;
            this.Handle = NativeWindow.GL.GenTexture();
            if(this.Handle == 0)
            {
                Error = "OpenGL could not create Texture handle";
                return;
            }

            this.Status = TextureStatus.IMPORTING_IMAGE;
            String str = DirectoryManager.GetResourceDir(dir_id);
            String path = DirectoryManager.ConcatDirFile(str, imfile);
            ImageResource im = ImageResource.Load(path, true); 
            if(im == null)
            {
                Error = "Image loading failed";
                return;
            }

            this.Status = TextureStatus.LOADING_IMAGE;
            NativeWindow.GL.ActiveTexture(GLEnum.Texture0);
            NativeWindow.GL.BindTexture(GLEnum.Texture2D, this.Handle);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapS, (uint)Wrap);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapT, (uint)Wrap);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMinFilter, (uint)Filter);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMagFilter, (uint)Filter);
            LoadImageIntoTexture(im);
            if (mipmap)
            {
                NativeWindow.GL.GenerateMipmap(GLEnum.Texture2D);
            }
            this.Status = TextureStatus.READY;
            NativeWindow.GL.BindTexture(GLEnum.Texture2D, 0);
        }

        public Texture(uint width, uint height)
        {
            this.Status = TextureStatus.UNINITIALISED;
            this.InitDefaults();

            this.Status = TextureStatus.AQUIRING_HANDLE;
            this.Handle = NativeWindow.GL.GenTexture();
            if (this.Handle == 0)
            {
                Error = "OpenGL could not create Texture handle";
                return;
            }

            this.Status = TextureStatus.LOADING_IMAGE;
            NativeWindow.GL.ActiveTexture(GLEnum.Texture0);
            NativeWindow.GL.BindTexture(GLEnum.Texture2D, this.Handle);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapS, (uint)Wrap);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapT, (uint)Wrap);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMinFilter, (uint)Filter);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMagFilter, (uint)Filter);
            NativeWindow.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, 0);
            this.Status = TextureStatus.READY;
        }

        public Texture(uint width, uint height, InternalFormat bits_per_channel, PixelFormat channels, PixelType pixel_type)
        {
            this.Status = TextureStatus.UNINITIALISED;
            this.InitDefaults();

            this.Status = TextureStatus.AQUIRING_HANDLE;
            this.Handle = NativeWindow.GL.GenTexture();
            if (this.Handle == 0)
            {
                Error = "OpenGL could not create Texture handle";
                return;
            }

            this.Status = TextureStatus.LOADING_IMAGE;
            NativeWindow.GL.ActiveTexture(GLEnum.Texture0);
            NativeWindow.GL.BindTexture(GLEnum.Texture2D, this.Handle);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapS, (uint)Wrap);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapT, (uint)Wrap);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMinFilter, (uint)Filter);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMagFilter, (uint)Filter);
            NativeWindow.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)bits_per_channel, width, height, 0, channels, pixel_type, 0);
            this.Status = TextureStatus.READY;
        }

        private void InitDefaults()
        {
            this.Wrap = TextureWrapSetting.REPEAT;
            this.Filter = MinMagSetting.NEAREST;
        }

        private unsafe void LoadImageIntoTexture(ImageResource im)
        {
            fixed (void* data = &MemoryMarshal.GetReference(im.image.GetPixelRowSpan(0)))
            {
                NativeWindow.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, im.width, im.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            }
        }

        public void SetFilterMode(MinMagSetting filter)
        {
            this.Bind();
            this.Filter = filter;
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMinFilter, (uint)filter);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMagFilter, (uint)filter);
            this.Unbind();
        }

        public void SetWrapMode(TextureWrapSetting wrap)
        {
            this.Bind();
            this.Wrap = wrap;
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapS, (uint)wrap);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapT, (uint)wrap);
            this.Unbind();
        }

        public void Bind()
        {
            NativeWindow.GL.BindTexture(GLEnum.Texture2D, this.Handle);
        }

        public void Unbind()
        {
            NativeWindow.GL.BindTexture(GLEnum.Texture2D, 0);
        }

        public void Dispose()
        {
            if(this.Handle > 0)
            {
                NativeWindow.GL.DeleteTexture(this.Handle);
            }
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
                    this.textures[tex_id].Bind();
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
            NativeWindow.GL.ActiveTexture(GLEnum.Texture0);
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
