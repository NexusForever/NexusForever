using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.GalacticArchive
{
    [Message(GameMessageOpcode.ServerGalacticArchiveUpdate)]
    public class ServerGalacticArchiveUpdate : IWritable
    {
        public ushort ArchiveArticleId { get; set; }
        public uint UnlockedFlags { get; set; } // Bits 0-15 correspond to the archiveEntry index not archiveEntryId, see archiveArticle tbl. Bit 30 is article viewed, bit 31 is article unlocked
        public uint ViewedFlags { get; set; } // Bits 0-15 correspond to the archiveEntry index not archiveEntryId, see archiveArticle tbl

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ArchiveArticleId, 14u);
            writer.Write(UnlockedFlags);
            writer.Write(ViewedFlags);
        }
    }
}
