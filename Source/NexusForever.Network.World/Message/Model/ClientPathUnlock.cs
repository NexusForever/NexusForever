using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathUnlock)]
    public class ClientPathUnlock : IReadable
    {
        public Path Path { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Path = reader.ReadEnum<Path>(3);
        }
    }
}
