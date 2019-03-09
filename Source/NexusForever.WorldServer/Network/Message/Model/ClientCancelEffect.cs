using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCancelEffect, MessageDirection.Client)]
    public class ClientCancelEffect : IReadable
    {
        public uint ServerUniqueId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ServerUniqueId = reader.ReadUInt();
        }
    }
}