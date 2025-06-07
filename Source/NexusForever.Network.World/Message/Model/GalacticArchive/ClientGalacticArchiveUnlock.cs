using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Sent when following GalacticArchiveLink or interacting with creatures that have archiveArticleIdInteractUnlock
    [Message(GameMessageOpcode.ClientGalacticArchiveUnlock)]
    public class ClientGalacticArchiveUnlock : IReadable
    {
        public uint ArchiveArticleId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ArchiveArticleId = reader.ReadUInt();
        }
    }
}
