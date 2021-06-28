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
        /// <summary>
        /// An image
        /// </summary>
        public Image<Rgba32> image;

        /// <summary>
        /// The width and height of an image
        /// </summary>
        public uint width, height;

        /// <summary>
        /// The constructor of an image
        /// </summary>
        /// <param name="im">The given image</param>
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

        /// <summary>
        /// A function which loads an image
        /// </summary>
        /// <param name="path">The path of the image</param>
        /// <param name="flip">A boolean which contains whether the image should be flipped</param>
        /// <returns></returns>
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

        /// <summary>
        /// A function which diposes of an image
        /// </summary>
        public void Dispose()
        {
            if(image != null)
            {
                image.Dispose();
            }
        }
    }
}
