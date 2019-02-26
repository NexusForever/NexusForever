using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCostumeSet, MessageDirection.Client)]
    public class ClientCostumeSet : IReadable
    {
        public int Index { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Index = reader.ReadInt();
        }
    }
}
