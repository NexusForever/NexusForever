using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ClientCraftingRuneSlotClear)]
    public class ClientCraftingRuneSlotClear : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public uint RuneSlotIndex { get; private set; }
        public bool RecoverRune { get; private set; } // clear slot = 0, recover = 1
        public bool UseGroupCurrency { get; private set; } // use credits = 0, use group currency = 1

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            RuneSlotIndex = reader.ReadUInt();
            RecoverRune = reader.ReadBit();
            UseGroupCurrency = reader.ReadBit();
        }
    }
}
