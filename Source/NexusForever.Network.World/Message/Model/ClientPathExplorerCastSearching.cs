using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathExplorerCastSearching)]
    public class ClientPathExplorerCastSearching : IReadable
    {
        public uint ClientSpellCastUniqueID { get; private set; }
        public byte SearchRadiusBand { get; private set; } // casts 1 of 4 Searching spells depending on band
        public ushort PathExplorerScavengerClueId {  get; private set; } // Relates to nearest ScavengerHunt WorldLocation

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueID = reader.ReadUInt();
            SearchRadiusBand = reader.ReadByte(2);
            PathExplorerScavengerClueId = reader.ReadUShort(14);
        }
    }
}
