using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    // Fires whenever a player successfully withdraws money or items from the guild bank.  This only fires for the player who performed the withdraw action.
    [Message(GameMessageOpcode.ServerGuildBankWithdraw)]
    public class ServerGuildBankWithdraw : IWritable
    {
        public Identity GuildIdentity { get; private set; }
        public GuildWithdrawlInfo WithdrawlInfo { get; private set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            WithdrawlInfo.Write(writer);
        }
    }
}
