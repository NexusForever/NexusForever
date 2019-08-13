using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum ItemUpdateReason
    {
        StackSplit                        = 0x0000,
        ConsumeCharge                     = 0x0001,
        SpellReagent                      = 0x0002,
        Transform                         = 0x0003,
        Cheat                             = 0x0004,
        Salvage                           = 0x0005,
        Extract                           = 0x0006,
        Vendor                            = 0x0007,
        Quest                             = 0x0008,
        SpellEffect                       = 0x0009,
        Script                            = 0x000A,
        PathReward                        = 0x000B,
        ResourceConversion                = 0x000C,
        Auction                           = 0x000D,
        MaterialBagConversion             = 0x000E,
        Loot                              = 0x000F,
        Buyback                           = 0x0010,
        Crafting                          = 0x0011,
        NewCharacter                      = 0x0012,
        PublicEvent                       = 0x0013,
        Mail                              = 0x0014,
        PlayerRequested                   = 0x0015,
        GuildBank                         = 0x0016,
        Trade                             = 0x0017,
        HousingCrate                      = 0x0018,
        GM                                = 0x0019,
        Expired                           = 0x001A,
        Gadget                            = 0x001B,
        AltSpell                          = 0x001C,
        Challenge                         = 0x001D,
        SettlerImprovementConsumeResource = 0x001E,
        HousingContribution               = 0x001F,
        HousingUpkeep                     = 0x0020,
        TradeskillAdditiveCost            = 0x0021,
        TradeskillGlyph                   = 0x0022,
        UniqueCleanup                     = 0x0023,
        PathExplorerReward                = 0x0024,
        CharacterRecustomization          = 0x0025,
        LootForge                         = 0x0026,
        RewardTrack                       = 0x0027,
        MatchingReward                    = 0x0028,
        DyeCostume                        = 0x0029,
        ItemDeprecation                   = 0x002A,
        PublicEventObjective              = 0x002B,
        MTXChest                          = 0x002C,
        GenericUnlock                     = 0x002F,
        RewardRotation                    = 0x0030,
        NoReason                          = 0x0031
    }
}
