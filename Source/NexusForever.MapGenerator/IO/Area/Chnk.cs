using System.IO;
using NexusForever.Shared.IO;

namespace NexusForever.MapGenerator.IO.Area
{
    public class Chnk : IReadable
    {
        public ChnkCell[] Cells = new ChnkCell[16 * 16];

        public void Read(BinaryReader reader)
        {
            uint lastIndex = 0;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var cellInfo = reader.ReadUInt32();
                uint index = (cellInfo >> 24) & 0xFF;
                uint size = cellInfo & 0x00FFFFFF;

                byte[] cellData = reader.ReadBytes((int)size);

                index += lastIndex;
                lastIndex = index + 1;

                using (var cellStream = new MemoryStream(cellData))
                using (var cellReader = new BinaryReader(cellStream))
                {
                    uint x = index % 16;
                    uint y = index / 16;
                    Cells[index] = new ChnkCell(x, y);
                    Cells[index].Read(cellReader);
                }
            }
        }
    }
}
