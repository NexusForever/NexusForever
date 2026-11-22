using System.Numerics;
using System.Text;

namespace NexusForever.IO
{
    public static class BinaryReaderExtensions
    {
        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static string ReadWCString(this BinaryReader reader)
        {
            using var sreader = new BinaryReader(reader.BaseStream, Encoding.Unicode, true);

            var sb = new StringBuilder();
            while (true)
            {
                var c = sreader.ReadChar();
                if (c == '\0')
                    break;

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
