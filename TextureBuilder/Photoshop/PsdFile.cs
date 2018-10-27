using System;
using System.IO;
using System.Collections.Generic;
using TextureBuilder.Photoshop.Data;

namespace TextureBuilder.Photoshop
{
    public enum PsdColorMode
    {
        Bitmap = 0,
        Grayscale = 1,
        Indexed = 2,
        RGB = 3,
        CMYK = 4,
        Multichannel = 7,
        Duotone = 8,
        Lab = 9
    }
        
    public class PsdFile
    {        
        public short Version;
        public short ChannelCount;
        public int Height;
        public int Width;
        public int BitDepth;
        public PsdColorMode ColorMode;

        public List<Layer> Layers;

        public PsdFile(Stream stream)          
        {
            Layers = new List<Layer>();
            Load(stream);
        }

        private void Load(Stream stream)
        {            
            using (var reader = new BigEndianReader(stream))
            {
                ReadHeader(reader);
                
                uint colorModeDataLength = reader.ReadUInt32();
                reader.Position += colorModeDataLength;

                uint imageResourcesLength = reader.ReadUInt32();
                reader.Position += imageResourcesLength;

                ReadLayerAndMaskInfo(reader);

                DecompressImages();
            }
        }
                       
        private void ReadHeader(BigEndianReader reader)
        {
            string signature = reader.ReadAsciiChars(4);
            if (signature != "8BPS")
                throw new Exception("Invalid signature in file header");

            Version = reader.ReadInt16();
            if (Version != 1)
                throw new Exception("Invalid version in file header");

            // 6 bytes reserved
            reader.Position += 6;

            ChannelCount = reader.ReadInt16();
            if (ChannelCount < 1 || ChannelCount > 56)
                throw new Exception("Invalid channel count in file header");            

            Height = reader.ReadInt32();
            Width = reader.ReadInt32();

            BitDepth = reader.ReadInt16();
            if (BitDepth != 8)
                throw new Exception("Invalid bit depth in file header");

            ColorMode = (PsdColorMode)reader.ReadInt16();
            if (ColorMode != PsdColorMode.RGB)
                throw new Exception("Invalid color mode in file header");
        }
                                              
        private void ReadLayerAndMaskInfo(BigEndianReader reader)
        {
            uint layersAndMaskInfoLength = reader.ReadUInt32();
            if (layersAndMaskInfoLength <= 0)
                return;

            long startPosition = reader.Position;
            long endPosition = startPosition + layersAndMaskInfoLength;

            ReadLayers(reader);
                                             
            reader.Position = endPosition;
        }        
        
        private void ReadLayers(BigEndianReader reader)
        {
            uint sectionLength = sectionLength = reader.ReadUInt32();            
            if (sectionLength <= 0)
                return;            

            long startPosition = reader.Position;
            short layerCount = reader.ReadInt16();
            
            if (layerCount < 0)                            
                layerCount = Math.Abs(layerCount);            

            if (layerCount == 0)
                return;

            for (int i = 0; i < layerCount; i++)
            {
                var layer = new Layer(BitDepth, reader);
                Layers.Add(layer);
            }
                        
            foreach (var layer in Layers)
            {
                foreach (var channel in layer.Channels)                
                    channel.ReadImageData(reader);                
            }
            
            if (sectionLength > 0)
            {                
                long endPosition = startPosition + sectionLength;                
                
                if (reader.Position < endPosition)
                    reader.Position = endPosition;
            }
        }
                   
        private void DecompressImages()
        {
            foreach (var layer in Layers)
            {
                foreach (var channel in layer.Channels)
                    channel.DecodeImageData();
            }
        }
    }   
}
