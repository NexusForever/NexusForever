using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Empties the set of galacticArchive state. Must send again with ServerGalacticArchiveUpdate.
    [Message(GameMessageOpcode.ServerGalacticArchiveRefresh)]
    public class ServerGalacticArchiveRefresh : IWritable
    {
        public void Write(GamePacketWriter writer)
        {
            // Zero byte message
        }
    }
}
