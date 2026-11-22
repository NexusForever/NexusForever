namespace NexusForever.IO.Format.WildStar.Area
{
    public class CellChunk : IBinaryReadable
    {
        public List<Cell> Cells { get; } = [];

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
                    
                    var cell = new Cell
                    {
                        X = x,
                        Y = y
                    };
                    cell.Read(cellReader);
                    Cells.Add(cell);
                }
            }
        }
    }
}
