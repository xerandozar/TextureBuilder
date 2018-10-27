using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TextureBuilder.Util
{
    public static class ImageWriter
    {
        public static void Write(string path, int width, int height, byte[] colors)
        {
            var bitmap = new Bitmap(width, height);
            var bitmapData = bitmap.LockBits
            (
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb
            );
            
            var pointer = bitmapData.Scan0;
            Marshal.Copy(colors, 0, pointer, colors.Length);
            bitmap.UnlockBits(bitmapData);

            bitmap.Save(path, ImageFormat.Png);
        }
    }
}
