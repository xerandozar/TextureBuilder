using System;
using System.IO;
using System.Text;
using TextureBuilder.Geometry;

namespace TextureBuilder.Photoshop.Data
{
    public class BigEndianReader : IDisposable
    {        
        public long Position
        {
            get { return m_Stream.Position; }
            set { m_Stream.Position = value; }
        }

        private Stream m_Stream;
        private BinaryReader m_Reader;        

        public BigEndianReader(Stream stream)
        {
            m_Stream = stream;
            m_Reader = new BinaryReader(stream, Encoding.ASCII);            
        }
        
        public byte ReadByte()
        {            
            return m_Reader.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {            
            return m_Reader.ReadBytes(count);
        }

        public bool ReadBoolean()
        {
            return m_Reader.ReadBoolean();
        }

        public short ReadInt16()
        {            
            short value = m_Reader.ReadInt16();
            return (short)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);            
        }

        public int ReadInt32()
        {
            int value = m_Reader.ReadInt32();
            return (int)((value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                         (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24);            
        }

        public ushort ReadUInt16()
        {
            ushort value = m_Reader.ReadUInt16();
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);            
        }

        public uint ReadUInt32()
        {
            uint value = m_Reader.ReadUInt32();
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }       

        public Rectangle ReadRectangle()
        {            
            int y = ReadInt32();
            int x = ReadInt32();
            int height = ReadInt32() - y;
            int width = ReadInt32() - x;
            return new Rectangle(x, y, width, height);
        }

        public string ReadAsciiChars(int count)
        {
            var bytes = m_Reader.ReadBytes(count);
            return Encoding.ASCII.GetString(bytes);            
        }

        public string ReadString(int padMultiplier)
        {                        
            long startPosition = Position;
            byte stringLength = ReadByte();
            var bytes = ReadBytes(stringLength);
            ReadPadding(startPosition, padMultiplier);
            return Encoding.UTF8.GetString(bytes);            
        }

        private void ReadPadding(long startPosition, int padMultiplier)
        {
            long totalLength = Position - startPosition;

            int remainder = (int)totalLength % padMultiplier;
            if (remainder == 0)
                return;
            
            ReadBytes(padMultiplier - remainder);
        }        

        public void Dispose()
        {
            m_Reader.Dispose();            
        }
    }
}
