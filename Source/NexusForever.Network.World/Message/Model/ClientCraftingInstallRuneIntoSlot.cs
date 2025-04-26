using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCraftingInstallRuneIntoSlot)]
    public class ClientCraftingInstallRuneIntoSlot : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public uint[] RuneSlotItem2Id { get; private set; } // Item2Id of rune, arranged in order of slot

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            uint count = reader.ReadUInt();
            RuneSlotItem2Id = new uint[count];
            for (int i = 0; i < count; i++)
            {
                RuneSlotItem2Id[i] = reader.ReadUInt();
            }
        }
    }
}
