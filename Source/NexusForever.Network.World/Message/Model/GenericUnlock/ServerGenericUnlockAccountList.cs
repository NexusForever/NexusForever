using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.GenericUnlock
{
    [Message(GameMessageOpcode.ServerGenericUnlockAccountList)]
    public class ServerGenericUnlockAccountList : IWritable
    {
        public List<uint> GenericUnlockEntryIds { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GenericUnlockEntryIds.Count);
            GenericUnlockEntryIds.ForEach(id => writer.Write(id));
        }
    }
}
