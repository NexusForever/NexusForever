namespace NexusForever.GameTable.Model
{
    public class CurrencyTypeEntry
    {
        public uint Id { get; set; }
        public string Description { get; set; }
        public uint LocalizedTextId { get; set; }
        public string IconName { get; set; }
        public ulong CapAmount { get; set; }
    }
}
