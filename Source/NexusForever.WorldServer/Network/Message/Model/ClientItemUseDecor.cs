using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientItemUseDecor, MessageDirection.Client)]
    public class ClientItemUseDecor : IReadable
    {
        public ulong ItemGuid { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
        }
    }
}
