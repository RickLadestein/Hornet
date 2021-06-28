using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Graphics.Buffers
{
    public class FrameBuffer : IDisposable
    {
        /// <summary>
        /// The identifier that refers to the OpenGL FrameBuffer resource
        /// </summary>
        public uint Handle { get; private set; }

        /// <summary>
        /// The identifier that refers to the FrameBuffer's RenderBuffer
        /// </summary>
        public uint RB_Handle { get; private set; }

        public MultiTexture Textures { get; private set; }

        private List<Texture> render_targets;
        private List<GLEnum> color_attachments;
        private uint width;
        private uint height;
        private uint current_attachment;
        private bool has_depth_buffer;


        /// <summary>
        /// Instantiates a new instance of FrameBuffer with specified screen width and height settings
        /// </summary>
        /// <param name="buffer_width">The buffer width</param>
        /// <param name="buffer_height">The buffer height</param>
        public FrameBuffer(uint buffer_width, uint buffer_height)
        {
            this.width = buffer_width;
            this.height = buffer_height;
            this.current_attachment = 0;
            this.Handle = NativeWindow.GL.GenFramebuffer();
            this.Textures = new MultiTexture();
            render_targets = new List<Texture>();
            color_attachments = new List<GLEnum>();
        }

        /// <summary>
        /// Clears the Color and Depth Buffers inside the currently bound FrameBuffer
        /// </summary>
        public void ClearBuffers()
        {
            NativeWindow.GL.Clear((int)GLEnum.ColorBufferBit);
            if(has_depth_buffer)
            {
                NativeWindow.GL.Clear((int)GLEnum.DepthBufferBit);
            }
        }

        /// <summary>
        /// Binds the current FrameBuffer as RenderTarget
        /// </summary>
        public void Bind()
        {
            NativeWindow.GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
            if(this.has_depth_buffer)
            {
                NativeWindow.GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RB_Handle);
            }
        }

        /// <summary>
        /// Unbinds the current bound FrameBuffer and binds the default Front and Back buffer
        /// </summary>
        public static void Unbind()
        {
            NativeWindow.GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            NativeWindow.GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        /// <summary>
        /// Attaches a color buffer rendertarget to the current framebuffer
        /// </summary>
        /// <param name="bits_per_channel">The amount of bits per channel enum</param>
        /// <param name="channels">The amount of channels per pixel</param>
        /// <param name="pixel_type">The type of value color values are stored into (byte, int, float, etc...)</param>
        public void AttachColorRenderTarget(InternalFormat bits_per_channel, PixelFormat channels, PixelType pixel_type)
        {
            NativeWindow.GL.BindFramebuffer(GLEnum.Framebuffer,this.Handle);
            Texture tex = new Texture(this.width, this.height, bits_per_channel, channels, pixel_type);
            tex.Bind();
            tex.SetFilterMode(MinMagSetting.NEAREST);
            tex.SetWrapMode(TextureWrapSetting.BORDER_CLAMP);
            this.render_targets.Add(tex);
            GLEnum use = (GLEnum)(((int)FramebufferAttachment.ColorAttachment0) + current_attachment);
            NativeWindow.GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, use, GLEnum.Texture2D, tex.Handle, 0);

            //Tell opengl that we have (current_attachment + 1) color buffers
            color_attachments.Add(use);
            GLEnum[] comps = this.color_attachments.ToArray();
            unsafe
            {
                fixed (GLEnum* first = &comps[0])
                {
                    NativeWindow.GL.DrawBuffers(current_attachment + 1, first);
                }
            }

            //this.color_attachments.Add(use);
            HTextureUnit unit = HTextureUnit.Unit_0 + current_attachment;
            this.Textures.SetTextureUnit(tex, unit);
            this.current_attachment += 1;

            //Unbind the current framebuffer
            FrameBuffer.Unbind();
        }

        /// <summary>
        /// Attaches a depth buffer rendertarget to the current framebuffer
        /// </summary>
        public void AttachDepthBufferTarget()
        {
            if (!this.has_depth_buffer)
            {
                this.RB_Handle = NativeWindow.GL.GenRenderbuffer();
                this.Bind();
                NativeWindow.GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent, this.width, this.height);
                NativeWindow.GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, this.RB_Handle);

                FrameBuffer.Unbind();
                this.has_depth_buffer = true;
            } else
            {
                throw new Exception("Cannot add DepthBuffer target to framebuffer: framebuffer already has DepthBuffer target");
            }
        }

        /// <summary>
        /// Retrieves the current framebuffer status
        /// </summary>
        /// <returns>empty if valid, a status if the framebuffer was invalid</returns>
        public String GetFrameBufferStatus()
        {
            this.Bind();
            string output = string.Empty;
            GLEnum status = NativeWindow.GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if(status != GLEnum.FramebufferComplete)
            {
                output = $"Framebuffer is not complete: {status}";
            }
            FrameBuffer.Unbind();
            return output;
        }


        /// <summary>
        /// Releases unmanaged GPU and OpenGL resources 
        /// </summary>
        public void Dispose()
        {
            if (RB_Handle != 0)
            {
                NativeWindow.GL.DeleteRenderbuffer(this.RB_Handle);
            }

            if (Handle != 0)
            {
                NativeWindow.GL.DeleteFramebuffer(this.Handle);
            }

            foreach(Texture tex in render_targets)
            {
                tex.Dispose();
            }
            this.render_targets.Clear();
        }
    }
}
