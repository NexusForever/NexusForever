using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientZoneMapPing)]
    public class ClientZoneMapPing : IReadable
    {
        public Identity Invoker { get; private set; } = new Identity();
        public Position PingLocation { get; private set; } = new Position();

        public void Read(GamePacketReader reader)
        {
            Invoker.Read(reader);
            PingLocation.Read(reader);
        }
    }
}
