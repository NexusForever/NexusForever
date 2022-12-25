using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientItemRuneSocketUnlock)]
    public class ClientItemRuneSocketUnlock : IReadable
    {
        public ulong Guid { get; private set; }
        public bool UseServiceTokens { get; private set; }
        public RuneType RuneType { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Guid = reader.ReadULong();
            UseServiceTokens = reader.ReadBit();
            RuneType = reader.ReadEnum<RuneType>(5u);
        }
    }
}
