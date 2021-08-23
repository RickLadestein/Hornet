using System;
using System.Text;

namespace HornetEditor.Util
{
    /// <summary>
    /// Structure containing multi purpose color representation
    /// </summary>
    public struct HornetColor
    {
        /// <summary>
        /// Red color value
        /// </summary>
        public float r;

        /// <summary>
        /// Green color value
        /// </summary>
        public float g;

        /// <summary>
        /// Blue color value
        /// </summary>
        public float b;

        /// <summary>
        /// Alpha color value
        /// </summary>
        public float a;

        /// <summary>
        /// Constructs a new instance of HornetColor with given parameters
        /// </summary>
        /// <param name="r">Red color value</param>
        /// <param name="g">Green color value</param>
        /// <param name="b">Blue color value</param>
        /// <param name="a">Alpha color value</param>
        public HornetColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        /// <summary>
        /// Constructs a HornetColor struct from color components
        /// </summary>
        /// <param name="r_byte">Byte representing red color value</param>
        /// <param name="g_byte">Byte representing green color value</param>
        /// <param name="b_byte">Byte representing blue color value</param>
        /// <param name="a_byte">Byte representing alpha color value</param>
        /// <returns></returns>
        public static HornetColor FromRgb(byte r_byte, byte g_byte, byte b_byte, byte a_byte)
        {
            return new HornetColor()
            {
                r = HornetColor.ConvertbyteToFloat(r_byte),
                g = HornetColor.ConvertbyteToFloat(g_byte),
                b = HornetColor.ConvertbyteToFloat(b_byte),
                a = HornetColor.ConvertbyteToFloat(a_byte)
            };
        }

        /// <summary>
        /// Converts System.Windows.Media.Color to HornetColor struct
        /// </summary>
        /// <param name="color">The System.Windows.Media.Color structure</param>
        /// <returns>HornetColor representing the System.Windows.Media.Color</returns>
        public static HornetColor FromColor(System.Windows.Media.Color color)
        {
            return new HornetColor()
            {
                r = color.R,
                g = color.G,
                b = color.B,
                a = color.A
            };
        }

        /// <summary>
        /// Converts System.Drawing.Color to HornetColor struct
        /// </summary>
        /// <param name="color">The System.Drawing.Color structure</param>
        /// <returns>HornetColor representing the System.Drawing.Color</returns>
        public static HornetColor FromColor(System.Drawing.Color color)
        {
            return new HornetColor()
            {
                r = color.R,
                g = color.G,
                b = color.B,
                a = color.A
            };
        }

        /// <summary>
        /// Gets the brush that can be used by wpf
        /// </summary>
        /// <returns>The brush representing this color</returns>
        public System.Windows.Media.Brush GetBrush()
        {
            return new System.Windows.Media.SolidColorBrush(this);
        }

        /// <summary>
        /// Converts float color space to byte color space
        /// </summary>
        /// <param name="_floatval">The floating point color value</param>
        /// <returns>Byte color representation of the float color value</returns>
        public static byte ConvertFloatTobyte(float _floatval)
        {
            if (_floatval > 1.0f)
            {
                return 255;
            }
            else if (_floatval < 0.0f)
            {
                return 0;
            }
            int number = (int)Math.Round(_floatval * 255.0f);
            return (byte)number;
        }


        /// <summary>
        /// Converts byte color space to float color space
        /// </summary>
        /// <param name="_byteval">The byte color value</param>
        /// <returns>Byte color representation of the float color value</returns>
        public static float ConvertbyteToFloat(byte _byteval)
        {
            int number = (int)_byteval;
            return ((float)number) / 255;
        }


        /// <summary>
        /// Converts System.Windows.Media.Color to HornetColor 
        /// </summary>
        /// <param name="color">System.Windows.Media.Color struct</param>
        public static implicit operator HornetColor(System.Windows.Media.Color color)
        {
            return HornetColor.FromColor(color);
        }

        /// <summary>
        /// Converts System.Drawing.Color to HornetColor 
        /// </summary>
        /// <param name="color">System.Drawing.Color struct</param>
        public static implicit operator HornetColor(System.Drawing.Color color)
        {
            return HornetColor.FromColor(color);
        }

        /// <summary>
        /// Converts HornetColor to System.Windows.Media.Color
        /// </summary>
        /// <param name="color">HornetColor struct</param>
        public static implicit operator System.Windows.Media.Color(HornetColor color)
        {
            return System.Windows.Media.Color.FromArgb(
                HornetColor.ConvertFloatTobyte(color.a),
                HornetColor.ConvertFloatTobyte(color.r),
                HornetColor.ConvertFloatTobyte(color.g),
                HornetColor.ConvertFloatTobyte(color.b)
            );
        }

        /// <summary>
        /// Converts HornetColor to System.Drawing.Color
        /// </summary>
        /// <param name="color">HornetColor struct</param>
        public static implicit operator System.Drawing.Color(HornetColor color)
        {
            return System.Drawing.Color.FromArgb(
                HornetColor.ConvertFloatTobyte(color.a),
                HornetColor.ConvertFloatTobyte(color.r),
                HornetColor.ConvertFloatTobyte(color.g),
                HornetColor.ConvertFloatTobyte(color.b)
            );
        }

        
    }

    public static class HornetColors
    {
        public static HornetColor AppDarkBackground { get { return new HornetColor(0.1f, 0.1f, 0.1f, 1.0f); } }
        public static HornetColor ControlLightGrayBackground { get { return new HornetColor(0.40f, 0.40f, 0.40f, 1.0f); } }
        public static HornetColor ControlMediumGrayBackground { get { return new HornetColor(0.35f, 0.35f, 0.35f, 1.0f); } }
        public static HornetColor ControlDarkGrayBackground { get { return new HornetColor(0.30f, 0.30f, 0.30f, 1.0f); } }
        public static HornetColor SelectDarkBackground { get { return new HornetColor(0.375f, 0.375f, 0.375f, 1.0f); } }
    }
}
