using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerPathChange)]
    public class ClientPlayerPathChange : IReadable
    {
        public Path Path { get; private set; }
        public bool UseTokens { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Path      = reader.ReadEnum<Path>(3);
            UseTokens = reader.ReadBit();
        }
    }
}
