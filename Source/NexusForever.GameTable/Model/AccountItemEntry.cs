namespace NexusForever.GameTable.Model
{
    public class AccountItemEntry
    { 
        public uint Id { get; set; }
        public uint Flags { get; set; }
        public uint Item2Id { get; set; }
        public uint EntitlementId { get; set; }
        public uint EntitlementCount { get; set; }
        public uint EntitlementScopeEnum { get; set; }
        public uint InstantEventEnum { get; set; }
        public uint AccountCurrencyEnum { get; set; }
        public ulong AccountCurrencyAmount { get; set; }
        public string ButtonIcon { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint AccountItemCooldownGroupId { get; set; }
        public uint StoreDisplayInfoId { get; set; }
        public string StoreIdentifierUpsell { get; set; }
        public uint Creature2DisplayGroupIdGacha { get; set; }
        public uint EntitlementIdPurchase { get; set; }
        public uint GenericUnlockSetId { get; set; }
    }
}
