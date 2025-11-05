using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.GenericUnlocks
{
    [Message(GameMessageOpcode.ServerGenericUnlockAccountList)]
    public class ServerGenericUnlockAccountList : IWritable
    {
        public List<uint> GenericUnlockEntryIds { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GenericUnlockEntryIds.Count);
            GenericUnlockEntryIds.ForEach(id => writer.Write(id));
        }
    }
}
