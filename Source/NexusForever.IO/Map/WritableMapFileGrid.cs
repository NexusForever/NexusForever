namespace NexusForever.IO.Map
{
    public class WritableMapFileGrid : MapFileGrid, IWritable
    {
        public WritableMapFileGrid(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);

            writer.Write(cells.Count);
            foreach (WritableMapFileCell cell in cells.Values)
                cell.Write(writer);
        }

        public void AddCell(WritableMapFileCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException();

            cells.Add((cell.X << 16) | cell.Y, cell);
        }
    }
}
