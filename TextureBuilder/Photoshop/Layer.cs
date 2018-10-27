using System;
using System.Linq;
using System.Collections.Generic;
using TextureBuilder.Geometry;
using TextureBuilder.Photoshop.Data;

namespace TextureBuilder.Photoshop
{    
    public class Layer
    {
        public bool IsRGBA
        {
            get
            {                
                if (Area.IsEmpty)
                    return false;

                if (Channels.Count != 4 || Channels.Any(item => !item.IsRGBA))
                    return false;

                if (Channels.Sum(item => item.ImageData.Length) != Area.Width * Area.Height * 4)
                    return false;

                return true;
            }
        }

        public int BitDepth;
        public Rectangle Area;

        public List<Channel> Channels;

        public string BlendMode;
        public byte Opacity;
        public bool Clipping;
        public string Name;

        public Layer(int bitDepth, BigEndianReader reader)          
        {            
            BitDepth = bitDepth;
            Area = reader.ReadRectangle();            

            Channels = new List<Channel>();

            int channelCount = reader.ReadUInt16();
            for (int i = 0; i < channelCount; i++)
            {
                var channel = new Channel(reader, this);
                Channels.Add(channel);                
            }            

            string signature = reader.ReadAsciiChars(4);
            if (signature != "8BIM")
                throw (new Exception("Invalid signature in layer header"));

            BlendMode = reader.ReadAsciiChars(4);
            Opacity = reader.ReadByte();
            Clipping = reader.ReadBoolean();

            // Flags
            reader.ReadByte();            

            // Padding
            reader.ReadByte();
            
            uint extraDataLength = reader.ReadUInt32();

            long startPosition = reader.Position;
            long endPosition = startPosition + extraDataLength;
                        
            uint maskLength = reader.ReadUInt32();
            reader.Position += maskLength;
                 
            int blendingRangesLength = reader.ReadInt32();
            reader.Position += blendingRangesLength;                    

            Name = reader.ReadString(4);
                        
            reader.Position = endPosition;            
        }
    }
}
