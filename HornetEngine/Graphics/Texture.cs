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
        /// <summary>
        /// The handle of the texture
        /// </summary>
        public uint Handle { get; private set; }

        /// <summary>
        /// The status of the etxture
        /// </summary>
        public TextureStatus Status { get; private set; }

        /// <summary>
        /// The error string
        /// </summary>
        public String Error { get; private set; }

        /// <summary>
        /// The wrap settings
        /// </summary>
        public TextureWrapSetting Wrap { get; private set; }

        /// <summary>
        /// The min mag filter
        /// </summary>
        public MinMagSetting Filter { get; private set; }

        /// <summary>
        /// The constructor of the texture
        /// </summary>
        /// <param name="dir_id">The folder ID</param>
        /// <param name="imfile">The name of the file</param>
        /// <param name="mipmap">A bool used for the mipmap initialization</param>
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

        /// <summary>
        /// The constructor of a texture
        /// </summary>
        /// <param name="width">The width of thhe texture</param>
        /// <param name="height">The height of the texture</param>
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

        /// <summary>
        /// The constructor of a Texture
        /// </summary>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of a texture</param>
        /// <param name="bits_per_channel">The bits per channel</param>
        /// <param name="channels">The channels of the texture</param>
        /// <param name="pixel_type">The pixeltype of the texture</param>
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
            //NativeWindow.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)bits_per_channel, width, height, 0, channels, pixel_type, IntPtr.Zero);
            unsafe
            {
                NativeWindow.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)bits_per_channel, width, height, 0, channels, pixel_type, null);
            }
            //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, 800, 600, 0, GL_RGB, GL_UNSIGNED_BYTE, NULL);
            this.Status = TextureStatus.READY;
        }

        private void InitDefaults()
        {
            this.Wrap = TextureWrapSetting.REPEAT;
            this.Filter = MinMagSetting.NEAREST;
            this.Error = string.Empty;
        }

        private unsafe void LoadImageIntoTexture(ImageResource im)
        {
            fixed (void* data = &MemoryMarshal.GetReference(im.image.GetPixelRowSpan(0)))
            {
                NativeWindow.GL.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, im.width, im.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            }
        }

        /// <summary>
        /// A function which sets a filter mode
        /// </summary>
        /// <param name="filter">The filter which should be set</param>
        public void SetFilterMode(MinMagSetting filter)
        {
            this.Bind();
            this.Filter = filter;
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMinFilter, (uint)filter);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureMagFilter, (uint)filter);
            this.Unbind();
        }

        /// <summary>
        /// A function which sets a wrap mode
        /// </summary>
        /// <param name="wrap">The wrap settings which should be set</param>
        public void SetWrapMode(TextureWrapSetting wrap)
        {
            this.Bind();
            this.Wrap = wrap;
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapS, (uint)wrap);
            NativeWindow.GL.TextureParameterI(this.Handle, GLEnum.TextureWrapT, (uint)wrap);
            this.Unbind();
        }

        /// <summary>
        /// A function which binds the texture
        /// </summary>
        public void Bind()
        {
            NativeWindow.GL.BindTexture(GLEnum.Texture2D, this.Handle);
        }

        /// <summary>
        /// A function which unbinds the texture
        /// </summary>
        public void Unbind()
        {
            NativeWindow.GL.BindTexture(GLEnum.Texture2D, 0);
        }

        /// <summary>
        /// A function that disposes the texture
        /// </summary>
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
        /// <summary>
        /// The maximum amount of texture layers
        /// </summary>
        public static readonly uint MAX_TEXTURE_LAYERS = 8;
        
        /// <summary>
        /// The currently bound textures
        /// </summary>
        public static MultiTexture current_bound;

        /// <summary>
        /// An arraylist of textures
        /// </summary>
        public Texture[] textures;

        /// <summary>
        /// The constructor of the MultiTexture
        /// </summary>
        public MultiTexture()
        {
            textures = new Texture[MAX_TEXTURE_LAYERS];
        }

        /// <summary>
        /// A function which binds the current multi textures
        /// </summary>
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

        /// <summary>
        /// A function which unbinds the current multi textures
        /// </summary>
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

        /// <summary>
        /// A function which sets a texture unit
        /// </summary>
        /// <param name="tex">The texture unit to set</param>
        /// <param name="layer">The allocated location for the GPU</param>
        public void SetTextureUnit(Texture tex, HTextureUnit layer)
        {
            uint loc = (uint)layer;
            if(loc >= MAX_TEXTURE_LAYERS)
            {
                throw new IndexOutOfRangeException("Texture unit was out of the range of the MultiTexture texture array");
            }
            this.textures[(uint)layer] = tex;
        }

        /// <summary>
        /// A function that clears the texture unit
        /// </summary>
        /// <param name="layer">The allocated space by the GPU</param>
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
