using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientDash)]
    public class ClientDash : IReadable
    {
        public DashDirection Direction { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Direction = reader.ReadEnum<DashDirection>(3u);
        }
    }
}
