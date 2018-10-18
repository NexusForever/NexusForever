namespace NexusForever.Shared.GameTable.Model
{
    public class CharacterCreationEntry
    {
        public uint Id;
        public uint ClassId;
        public uint RaceId;
        public uint Sex;
        public uint FactionId;
        public bool CostumeOnly;
        [GameTableFieldArray(16u)]
        public uint[] ItemIds;
        public bool Enabled;
        public uint CharacterCreationStartEnum;
        public uint Xp;
        public uint AccountCurrencyTypeIdCost;
        public uint AccountCurrencyAmountCost;
        public uint EntitlementIdRequired;
    }
}
