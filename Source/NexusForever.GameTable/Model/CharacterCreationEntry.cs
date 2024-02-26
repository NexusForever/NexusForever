namespace NexusForever.GameTable.Model
{
    public class CharacterCreationEntry
    {
        public uint Id { get; set; }
        public uint ClassId { get; set; }
        public uint RaceId { get; set; }
        public uint Sex { get; set; }
        public uint FactionId { get; set; }
        public bool CostumeOnly { get; set; }
        [GameTableFieldArray(16u)]
        public uint[] ItemIds { get; set; }
        public bool Enabled { get; set; }
        public uint CharacterCreationStartEnum { get; set; }
        public uint Xp { get; set; }
        public uint AccountCurrencyTypeIdCost { get; set; }
        public uint AccountCurrencyAmountCost { get; set; }
        public uint EntitlementIdRequired { get; set; }
    }
}
