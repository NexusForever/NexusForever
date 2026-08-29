namespace NexusForever.Game.Static.Entity
{
    // TODO: research this more
    public enum InventoryLocation
    {
        [InventoryLocation(30u)]
        Equipped  = 0,

        [InventoryLocation(16u)]
        Inventory = 1,

        [InventoryLocation(32u)]
        PlayerBank  = 2,

        [InventoryLocation(512u)]
        Ability   = 4,

        Unknown5  = 5,
        Unknown8  = 8,
        Unknown9  = 9,
        Unknown10 = 10,

        [InventoryLocation(128u)]
        GuildBankTab1 = 100,
        [InventoryLocation(128u)]
        GuildBankTab2 = 101,
        [InventoryLocation(128u)]
        GuildBankTab3 = 102,
        [InventoryLocation(128u)]
        GuildBankTab4 = 103,
        [InventoryLocation(128u)]
        GuildBankTab5 = 104,
        [InventoryLocation(128u)]
        GuildBankTab6 = 105,
        [InventoryLocation(128u)]
        GuildBankTab7 = 106,
        [InventoryLocation(128u)]
        GuildBankTab8 = 107,
        [InventoryLocation(128u)]
        GuildBankTab9 = 108,
        [InventoryLocation(128u)]
        GuildBankTab10 = 109,
        [InventoryLocation(128u)]
        WarPartyBankTab1 = 200,
        [InventoryLocation(128u)]
        WarPartyBankTab2 = 201,
        [InventoryLocation(128u)]
        WarPartyBankTab3 = 202,
        [InventoryLocation(128u)]
        WarPartyBankTab4 = 203,
        [InventoryLocation(128u)]
        WarPartyBankTab5 = 204,
        [InventoryLocation(128u)]
        WarPartyBankTab6 = 205,
        [InventoryLocation(128u)]
        WarPartyBankTab7 = 206,
        [InventoryLocation(128u)]
        WarPartyBankTab8 = 207,
        [InventoryLocation(128u)]
        WarPartyBankTab9 = 208,
        [InventoryLocation(128u)]
        WarPartyBankTab10 = 209,

        None      = ushort.MaxValue
    }
}
