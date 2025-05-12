namespace NexusForever.Game.Static.Crafting
{
    public enum CraftingModifierType
    {
        MismatchPenalty             = 0x1,//additive
        UnbuffedFailCapBase         = 0x2,//additive
        Charge                      = 0x3,//additive
        MaterialCost                = 0x6,//additive
        Material2Id                 = 0x7,//fixed
        UnbuffedFailCap             = 0x8,//additive
        ChargeIncrement_RightShift  = 0x9,//additive
        OutputCount                 = 0xA,//multiplier
        AdditiveCost                = 0xE,//multiplier
        AdditiveVector              = 0xF,//multiplier
        AdditiveRadius              = 0x10,//multiplier
        SchematicDiscoveryRadius1   = 0x11,//multiplier
        SchematicDiscoveryRadius2   = 0x14,//multiplier
        ApSpSplitMaxDelta_LeftShift = 0x15,//additive
        Cost                        = 0x16,//multiplier
        CraftIsCritical             = 0x19,//bool
        AdditiveLimit               = 0x1A,//additive
        AdditiveTier                = 0x1B,//additive        
    }
}
