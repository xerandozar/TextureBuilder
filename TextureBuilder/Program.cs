using System;
using System.IO;
using TextureBuilder.Photoshop;
using TextureBuilder.Util;

namespace TextureBuilder
{
    class Program
    {
        public const string PSD_PATH = "./Debug.psd";
        public const string EXPORT_PATH = "./Export";

        static void Main(string[] args)
        {
            if (!Directory.Exists(EXPORT_PATH))
                Directory.CreateDirectory(EXPORT_PATH);

            Console.WriteLine($"Reading => {PSD_PATH}");
            try
            {
                using (var stream = new FileStream(PSD_PATH, FileMode.Open))
                {
                    var psdFile = new PsdFile(stream);

                    foreach (var layer in psdFile.Layers)
                    {
                        if (!layer.IsRGBA)
                            continue;

                        int width = layer.Area.Width;
                        int height = layer.Area.Height;

                        int colorCount = width * height;
                        byte[] colors = new byte[colorCount * 4];

                        var red = layer.Channels[0];
                        var green = layer.Channels[1];
                        var blue = layer.Channels[2];
                        var alpha = layer.Channels[3];

                        int writeIndex = 0;
                        for (int i = 0; i < colorCount; i++)
                        {
                            colors[writeIndex + 0] = alpha.ImageData[i];
                            colors[writeIndex + 1] = blue.ImageData[i];
                            colors[writeIndex + 2] = green.ImageData[i];
                            colors[writeIndex + 3] = red.ImageData[i];
                            writeIndex += 4;
                        }

                        string path = $"{EXPORT_PATH}/{layer.Name}.png";
                        ImageWriter.Write(path, width, height, colors);
                        Console.WriteLine($"Created => {path}");
                    }
                }

                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
                        
            Console.ReadKey();
        }        
    }       
}
