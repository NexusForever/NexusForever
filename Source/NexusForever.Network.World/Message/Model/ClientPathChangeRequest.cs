using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathChangeRequest)]
    public class ClientPathChangeRequest : IReadable
    {
        public Path Path { get; private set; }
        public bool OnCooldown { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Path      = reader.ReadEnum<Path>(3);
            OnCooldown = reader.ReadBit();
        }
    }
}
