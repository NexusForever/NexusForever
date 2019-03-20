namespace NexusForever.Database.Character.Model
{
    public partial class GuildRank
    {
        public ulong Id { get; set; }
        public byte Index { get; set; }
        public string Name { get; set; }
        public int Permission { get; set; }
        public ulong BankWithdrawalPermission { get; set; }
        public ulong MoneyWithdrawalLimit { get; set; }
        public ulong RepairLimit { get; set; }

        public Guild IdNavigation { get; set; }
    }
}