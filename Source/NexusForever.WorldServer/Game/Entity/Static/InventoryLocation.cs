namespace NexusForever.WorldServer.Game.Entity.Static
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
        None      = ushort.MaxValue
    }
}
