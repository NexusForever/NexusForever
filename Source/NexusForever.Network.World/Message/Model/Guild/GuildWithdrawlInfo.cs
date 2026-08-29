using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    public class GuildWithdrawlInfo : IWritable
    {
        public ulong BankMoneyWithdrawnToday { get; set; }
        public uint[] BankTabWithdrawCount { get; set; } = new uint[10];
        public ulong BankRepairMoneyToday { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BankMoneyWithdrawnToday);
            foreach (uint bankTab in BankTabWithdrawCount)
                writer.Write(bankTab);
            writer.Write(BankRepairMoneyToday);
        }
    }
}
