using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class GuildRank : IWritable
    {
        public string RankName { get; set; } = "";
        public GuildRankPermission PermissionMask { get; set; } = GuildRankPermission.Disabled;
        public ulong BankWithdrawalPermissions { get; set; }
        public ulong MoneyWithdrawalLimit { get; set; }
        public ulong RepairLimit { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(RankName);
            writer.Write(PermissionMask, 32u);
            writer.Write(BankWithdrawalPermissions);
            writer.Write(MoneyWithdrawalLimit);
            writer.Write(RepairLimit);
        }
    }
}
