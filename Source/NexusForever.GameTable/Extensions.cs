using System.Text;

namespace NexusForever.GameTable
{
    public static class Extensions
    {
        public static string ReadWideString(this BinaryReader reader)
        {
            var bytes = new List<byte>();

            while (true)
            {
                // Read UTF-16LE code units independently of the reader's encoding.
                ushort codeUnit = reader.ReadUInt16();
                if (codeUnit == 0)
                    return Encoding.Unicode.GetString(bytes.ToArray());

                // Decode the complete string so surrogate pairs are kept together.
                bytes.Add((byte)codeUnit);
                bytes.Add((byte)(codeUnit >> 8));
            }
        }
    }
}
