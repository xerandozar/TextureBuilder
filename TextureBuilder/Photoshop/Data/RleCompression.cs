using System;
using System.IO;

namespace TextureBuilder.Photoshop.Data
{    
    public class RleCompression
    {
        private Stream m_Stream;

        public RleCompression(Stream stream)
        {
            m_Stream = stream;
        }

        unsafe public int Decode(byte[] buffer, int offset, int count)
        {            
            fixed (byte* bufferPointer = &buffer[0])
            {
                int bytesLeft = count;
                int bufferIdx = offset;

                while (bytesLeft > 0)
                {                                        
                    sbyte flagCounter = unchecked((sbyte)m_Stream.ReadByte());

                    // Raw packet
                    if (flagCounter > 0)
                    {
                        int readLength = flagCounter + 1;
                        m_Stream.Read(buffer, bufferIdx, readLength);

                        bufferIdx += readLength;
                        bytesLeft -= readLength;
                    }
                    // RLE packet
                    else if (flagCounter > -128)
                    {
                        int runLength = 1 - flagCounter;
                        byte byteValue = (byte)m_Stream.ReadByte();
                        
                        byte* pointer = bufferPointer + bufferIdx;
                        byte* pointerEnd = pointer + runLength;

                        while (pointer < pointerEnd)
                        {
                            *pointer = byteValue;
                            pointer++;
                        }

                        bufferIdx += runLength;
                        bytesLeft -= runLength;
                    }                    
                }
                
                return count - bytesLeft;
            }
        }
    }
}