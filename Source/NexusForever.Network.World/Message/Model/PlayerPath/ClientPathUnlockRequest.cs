using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.PlayerPath.Path;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ClientPathUnlockRequest)]
    public class ClientPathUnlockRequest : IReadable
    {
        public Path Path { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Path = reader.ReadEnum<Path>(3);
        }
    }
}
