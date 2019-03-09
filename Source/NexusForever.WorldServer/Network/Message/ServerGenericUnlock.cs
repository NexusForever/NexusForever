using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message
{
    [Message(GameMessageOpcode.ServerGenericUnlock, MessageDirection.Server)]
    public class ServerGenericUnlock : IWritable
    {
        public ushort GenericUnlockEntryId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GenericUnlockEntryId, 14u);
        }
    }
}
