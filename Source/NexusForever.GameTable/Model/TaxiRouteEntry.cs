namespace NexusForever.GameTable.Model
{
    public class TaxiRouteEntry
    {
        public uint Id { get; set; }
        public uint TaxiNodeIdSource { get; set; }
        public uint TaxiNodeIdDestination { get; set; }
        public uint Price { get; set; }
    }
}
