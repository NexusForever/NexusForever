using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.GalacticArchive
{
    // Sent when SetViewed function is called on an archiveArticle that is unlocked and is unviewed or has an unlocked archiveEntry that is unviewed
    // The client has already marked the archiveArticle and every unlocked archiveEntry as viewed, relies on bit flags sent in ServerGalacticArchiveUpdate
    [Message(GameMessageOpcode.ClientGalacticArchiveViewed)]
    public class ClientGalacticArchiveViewed : IReadable
    {
        public ushort ArchiveArticleId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ArchiveArticleId = reader.ReadUShort(14u);
        }
    }
}
