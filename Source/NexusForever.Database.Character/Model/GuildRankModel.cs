namespace NexusForever.Database.Character.Model
{
    public class GuildRankModel
    {
        public ulong Id { get; set; }
        public byte Index { get; set; }
        public string Name { get; set; }
        public uint Permission { get; set; }
        public ulong BankWithdrawalPermission { get; set; }
        public ulong MoneyWithdrawalLimit { get; set; }
        public ulong RepairLimit { get; set; }

        public GuildModel Guild { get; set; }
    }
}