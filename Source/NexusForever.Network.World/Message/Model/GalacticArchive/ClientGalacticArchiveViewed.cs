using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Sent when SetViewed function is called on an archiveEntry that is unlocked but unviewed
    // Relies on bit flags sent in ServerGalacticArchiveUpdate to determine which archiveEntry was viewed for the given ArchiveArticleId
    [Message(GameMessageOpcode.ClientGalacticArchiveViewed)]
    public class ClientGalacticArchiveViewed : IReadable
    {
        public uint ArchiveArticleId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ArchiveArticleId = reader.ReadUInt();
        }
    }
}
