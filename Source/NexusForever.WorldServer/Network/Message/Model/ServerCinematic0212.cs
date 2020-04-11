using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
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
