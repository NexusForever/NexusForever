using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    // Sent by client if inventory has not been loaded when the tab is opened.
    // Server sends ServerGuildBankTabInventory (0x47A) response
    [Message(GameMessageOpcode.ClientGuildBankTabOpen)]
    public class ClientGuildBankTabOpen : IReadable
    {
        public Identity GuildIdentity { get; private set; } = new();
        public ushort BankTabIndex { get; private set; } // Guild bank tab indexes are 100-109, WarParty bank tabs 200-209

        public void Read(GamePacketReader reader)
        {
            GuildIdentity.Read(reader);
            BankTabIndex = reader.ReadUShort(9u);
        }
    }
}
