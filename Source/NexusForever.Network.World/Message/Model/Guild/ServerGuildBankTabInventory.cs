using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildBankTabInventory)]
    public class ServerGuildBankTabInventory : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public InventoryLocation BankTabIndex { get; set; } // Guild bank tab indexes are 100-109, WarParty bank tabs 200-209
        public List<InventoryItem> InventoryItems { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(BankTabIndex, 9u);
            foreach(var item in InventoryItems)
            {
                item.Write(writer);
            }
        }
    }
}
