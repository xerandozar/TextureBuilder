using System;
using System.IO;
using TextureBuilder.Photoshop.Data;

namespace TextureBuilder.Photoshop
{
    public enum ImageCompression
    {
        Raw = 0,
        Rle = 1,
        Zip = 2,
        ZipPrediction = 3
    }

    public class Channel
    {
        public bool IsRGBA => ID >= -1 && ID <= 2;

        public short ID;
        public int Length;
        public Layer Layer;        
               
        public byte[] ImageDataRaw;
        public byte[] ImageData;

        public ImageCompression ImageCompression;        
               
        public Channel(BigEndianReader reader, Layer layer)
        {
            ID = reader.ReadInt16();
            Length = reader.ReadInt32();
            Layer = layer;            
        }

        public void ReadImageData(BigEndianReader reader)
        {            
            long endPosition = reader.Position + Length;
            if (!IsRGBA)
            {
                reader.Position = endPosition;
                return;
            }            

            ImageCompression = (ImageCompression)reader.ReadInt16();            
            
            switch (ImageCompression)
            {
                case ImageCompression.Raw:
                    int dataLength = Length - 2;
                    ImageDataRaw = reader.ReadBytes(dataLength);
                    break;
                case ImageCompression.Rle:
                    for (int i = 0; i < Layer.Area.Height; i++)
                    {
                        // Skip rle width
                        reader.ReadUInt16();
                    }

                    int rleDataLength = (int)(endPosition - reader.Position);                    
                    ImageDataRaw = reader.ReadBytes(rleDataLength);
                    break;
                case ImageCompression.Zip:
                case ImageCompression.ZipPrediction:
                    // Not implemented
                    break;
            }

            reader.Position = endPosition;
        }
        
        public void DecodeImageData()
        {            
            if (!IsRGBA)
                return;
                     
            switch (ImageCompression)
            {
                case ImageCompression.Raw:
                    ImageData = ImageDataRaw;
                    break;
                case ImageCompression.Rle:                    
                    using (var stream = new MemoryStream(ImageDataRaw))
                    {
                        var rleCompression = new RleCompression(stream);

                        int bytesPerRow = Layer.Area.Width;
                        int bytesTotal = Layer.Area.Height * bytesPerRow;

                        ImageData = new byte[bytesTotal];                        
                        
                        for (int i = 0; i < Layer.Area.Height; i++)
                        {
                            int rowIndex = i * bytesPerRow;
                            rleCompression.Decode(ImageData, rowIndex, bytesPerRow);
                        }
                    }
                    break;
                case ImageCompression.Zip:
                case ImageCompression.ZipPrediction:
                    // Not implemented
                    break;
            }                                   
        }               
    }
}