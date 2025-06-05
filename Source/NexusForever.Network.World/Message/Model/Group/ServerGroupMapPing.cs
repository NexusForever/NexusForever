using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerZoneMapPing)]
    public class ServerZoneMapPing : IWritable
    {
        public Identity Invoker { get; set; }
        public Position PingLocation { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Invoker.Write(writer);
            PingLocation.Write(writer);
        }
    }
}
