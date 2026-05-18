using System.Text;

namespace NexusForever.GameTable
{
    public class StringTable : IDisposable
    {
        private readonly MemoryStream stream;
        private readonly BinaryReader reader;

        public StringTable(byte[] buffer)
        {
            stream = new MemoryStream(buffer);
            reader = new BinaryReader(stream, Encoding.Unicode);
        }

        public void Dispose()
        {
            reader.Dispose();
            stream.Dispose();
        }

        public string GetEntry(uint offset)
        {
            stream.Position = offset;
            return reader.ReadWideString();
        }
    }
}
