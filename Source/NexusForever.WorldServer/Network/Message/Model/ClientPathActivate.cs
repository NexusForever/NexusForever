using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPathActivate)]
    public class ClientPathActivate : IReadable
    {
        public Path Path { get; private set; }
        public bool UseTokens { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Path = (Path)reader.ReadByte(3);
            UseTokens = reader.ReadBit();
        }
    }
}
