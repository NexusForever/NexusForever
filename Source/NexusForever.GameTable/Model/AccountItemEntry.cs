namespace NexusForever.GameTable.Model
{
    public class AccountItemEntry
    { 
        public uint Id { get { get; set; } set { get; set; } }
        public uint Flags { get { get; set; } set { get; set; } }
        public uint Item2Id { get { get; set; } set { get; set; } }
        public uint EntitlementId { get { get; set; } set { get; set; } }
        public uint EntitlementCount { get { get; set; } set { get; set; } }
        public uint EntitlementScopeEnum { get { get; set; } set { get; set; } }
        public uint InstantEventEnum { get { get; set; } set { get; set; } }
        public uint AccountCurrencyEnum { get { get; set; } set { get; set; } }
        public ulong AccountCurrencyAmount { get { get; set; } set { get; set; } }
        public string ButtonIcon { get { get; set; } set { get; set; } }
        public uint PrerequisiteId { get { get; set; } set { get; set; } }
        public uint AccountItemCooldownGroupId { get { get; set; } set { get; set; } }
        public uint StoreDisplayInfoId { get { get; set; } set { get; set; } }
        public string StoreIdentifierUpsell { get { get; set; } set { get; set; } }
        public uint Creature2DisplayGroupIdGacha { get { get; set; } set { get; set; } }
        public uint EntitlementIdPurchase { get { get; set; } set { get; set; } }
        public uint GenericUnlockSetId { get { get; set; } set { get; set; } }
    }
}
