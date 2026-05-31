using NexusForever.Network.Message;
using NexusForever.Game.Static.Crafting;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ClientCraftingRuneSlotReroll)]
    public class ClientCraftingRuneSlotReroll : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public uint SlotIndex { get; private set; }
        public RuneType Type { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            SlotIndex = reader.ReadUInt();
            Type = reader.ReadEnum<RuneType>(5);
        }
    }
}
