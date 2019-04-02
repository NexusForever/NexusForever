using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum ItemError
    {
        IsValid                 = 0, // Used internally, not recognised by client.
        InventoryFull           = 27,
        CantUseThat             = 30,
        InvalidForThisSlot      = 31,
        BagMustBeEmpty          = 35,
        WrongRace               = 39,
        WrongClass              = 40,
        LevelTooLow             = 42,
        CannotDoThatInCombat    = 63,
        CannotBeSalvaged        = 72,
        WrongFaction            = 132,
        CannotBeDeleted         = 156
    }
}
