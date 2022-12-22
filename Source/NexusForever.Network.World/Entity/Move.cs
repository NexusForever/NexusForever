using NexusForever.Network.Message;

namespace NexusForever.Network.World.Entity
{
    public class Move : IReadable, IWritable
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Z { get; set; }
        
        public void Read(GamePacketReader reader)
        {
            X = reader.ReadUShort();
            Y = reader.ReadUShort();
            Z = reader.ReadUShort();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
    }
}
