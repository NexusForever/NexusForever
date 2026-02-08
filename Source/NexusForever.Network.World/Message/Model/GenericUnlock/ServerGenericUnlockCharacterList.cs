using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.GenericUnlock
{
    // Sets bCharacterUnlocked state on all GenericUnlocks on the client.
    // Seems that ServerGenericUnlocksCharacterRefreshed was the preferred method
    // as that clears the bCharacterUnlocked state on everything first
    [Message(GameMessageOpcode.ServerGenericUnlockCharacterList)]
    public class ServerGenericUnlockCharacterList : IWritable
    {
        public List<uint> GenericUnlockEntryIds { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GenericUnlockEntryIds.Count);
            GenericUnlockEntryIds.ForEach(id => writer.Write(id));
        }
    }
}
