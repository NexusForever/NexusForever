using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerPathExplorerCastSearching)]
    public class ClientPlayerPathExplorerCastSearching : IReadable
    {
        public uint ClientSpellCastUniqueID { get; set; }
        public byte AbilityItemIndex { get; set; }
        public ushort Unknown {  get; set; }

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueID = reader.ReadUInt();
            AbilityItemIndex = reader.ReadByte(2);
            Unknown = reader.ReadUShort(14);
        }
    }
}
