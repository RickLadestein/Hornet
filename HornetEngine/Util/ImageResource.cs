using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using HornetEngine.Graphics;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using Silk.NET.OpenGL;

namespace HornetEngine.Util
{
    public class ImageResource : IDisposable
    {
        public Image<Rgba32> image;
        public uint width, height;

        public ImageResource(SixLabors.ImageSharp.Image<Rgba32> im)
        {
            if(im == null)
            {
                throw new ArgumentException("Image cannot be null");
            }

            this.image = im;
            width = (uint)im.Width;
            height = (uint)im.Height;
        }

        public static ImageResource Load(string path, bool flip)
        {
            try
            {
                SixLabors.ImageSharp.Image<Rgba32> im = (SixLabors.ImageSharp.Image<Rgba32>)SixLabors.ImageSharp.Image.Load(path);
                if(flip)
                {
                    im.Mutate(x => x.Flip(FlipMode.Vertical));
                }
                return new ImageResource(im);
            } catch(Exception ex)
            {
                return null;
            }
        }

        public void Dispose()
        {
            if(image != null)
            {
                image.Dispose();
            }
        }
    }
}
