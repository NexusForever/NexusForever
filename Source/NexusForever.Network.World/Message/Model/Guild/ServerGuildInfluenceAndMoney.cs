using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    // Fires whenever a guild's credits or influence are gained, withdrawn, or spent.
    [Message(GameMessageOpcode.ServerGuildInfluenceAndMoney)]
    public class ServerGuildInfluenceAndMoney : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public uint Influence { get; set; }
        public uint BonusInfluenceRemaining { get; set; }
        public ulong CashAmount { get; set; }
        public uint WarCoins { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(Influence);
            writer.Write(BonusInfluenceRemaining);
            writer.Write(CashAmount);
            writer.Write(WarCoins);
        }
    }
}
