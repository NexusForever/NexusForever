using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.GameTable.Model
{
    public class CharacterCreationEntry
    {
        public uint Id;
        public uint ClassId;
        public Race RaceId;
        public Sex Sex;
        public Faction FactionId;
        public bool CostumeOnly;
        [GameTableFieldArray(16u)]
        public uint[] ItemIds;
        public bool Enabled;
        public CharacterCreationStart CharacterCreationStartEnum;
        public uint Xp;
        public uint AccountCurrencyTypeIdCost;
        public uint AccountCurrencyAmountCost;
        public uint EntitlementIdRequired;
    }
}
