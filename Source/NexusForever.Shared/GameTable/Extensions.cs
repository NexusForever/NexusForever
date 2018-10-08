using System;
using System.IO;
using System.Text;

namespace NexusForever.Shared.GameTable
{
    public static class Extensions
    {
        public static string ReadWideString(this BinaryReader reader)
        {
            uint position = 0u;
            byte[] buffer = new byte[64];

            while (true)
            {
                if (position == buffer.Length)
                {
                    byte[] content = buffer;
                    buffer = new byte[buffer.Length + 64];
                    Buffer.BlockCopy(content, 0, buffer, 0, content.Length);
                }

                ushort character = reader.ReadUInt16();
                if (character == 0)
                    return Encoding.Unicode.GetString(buffer, 0, (int)position);

                buffer[position++] = (byte)(character & 0x00FF);
                buffer[position++] = (byte)(character >> 8);
            }
        }
    }
}
