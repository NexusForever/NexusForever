using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NexusForever.Shared.GameTable
{
    public static class Extensions
    {
        public static string ReadWideString(this BinaryReader reader)
        {
            // We don't know if the reader is using the correct encoding. We shouldn't have to do this here,
            // but that would be alot of changes to make to fix. So instead we'll just create our own reader
            // and use the base stream with the correct encoding. This should cause no harm, since we advance
            // the underlying stream and BinaryReader doesn't actually care about position.
            using var wrappedReader = new BinaryReader(reader.BaseStream, Encoding.Unicode, true);
            var characters = new List<char>();

            while (true)
            {
                char character = wrappedReader.ReadChar();
                if (character == 0)
                    return new string(characters.ToArray());
                characters.Add(character);
            }
        }
    }
}
