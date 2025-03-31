using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerPathExplorerCastSearching)]
    public class ClientPlayerPathExplorerCastSearching : IReadable
    {
        public uint ClientSpellCastUniqueID { get; set; }
        public byte SearchRadiusBand { get; set; } // casts 1 of 4 Searching spells depending on band
        public ushort PathExplorerScavengerClueId {  get; set; } // Relates to nearest ScavengerHunt WorldLocation

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueID = reader.ReadUInt();
            SearchRadiusBand = reader.ReadByte(2);
            PathExplorerScavengerClueId = reader.ReadUShort(14);
        }
    }
}
