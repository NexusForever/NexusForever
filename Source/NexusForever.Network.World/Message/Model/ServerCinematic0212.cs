using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematic0212)]
    public class ServerCinematic0212 : IWritable
    {
        public uint Unknown0 { get; set; }
        public Position Position { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            Position.Write(writer);
        }
    }
}
