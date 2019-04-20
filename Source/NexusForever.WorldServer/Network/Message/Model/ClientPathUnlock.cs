using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPathUnlock)]
    public class ClientPathUnlock : IReadable
    {
        public Path Path { get; set; }

        public void Read(GamePacketReader reader)
        {
            Path = reader.ReadEnum<Path>(3);
        }
    }
}
