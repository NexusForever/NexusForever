using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class GuildPlayerLimits: IWritable
    {
        public ulong BankWithdrawnMoney { get; set; }
        public uint[] BankTabWithdrawCount { get; set; } = new uint[10];
        public ulong BankRepairMoney { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BankWithdrawnMoney);
            foreach (uint bankTab in BankTabWithdrawCount)
                writer.Write(bankTab);
            writer.Write(BankRepairMoney);
        }
    }
}
