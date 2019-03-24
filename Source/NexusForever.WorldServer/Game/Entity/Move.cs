using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Move : IReadable, IWritable
    {
        ushort X { get; set; }
        ushort Y { get; set; }
        ushort Z { get; set; }
        
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
