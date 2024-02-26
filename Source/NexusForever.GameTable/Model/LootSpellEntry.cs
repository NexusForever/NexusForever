namespace NexusForever.GameTable.Model
{
    public class LootSpellEntry
    {
        public uint Id { get; set; }
        public uint LootSpellTypeEnum { get; set; }
        public uint LootSpellPickupEnumFlags { get; set; }
        public uint Creature2Id { get; set; }
        public string ButtonIcon { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public uint VisualEffectId { get; set; }
        public uint Data { get; set; }
        public uint DataValue { get; set; }
    }
}
