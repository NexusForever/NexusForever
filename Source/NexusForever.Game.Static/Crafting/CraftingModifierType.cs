namespace NexusForever.Game.Static.Crafting
{
    public enum CraftingModifierType
    {
        MismatchPenalty             = 1,  //additive
        UnbuffedFailCapBase         = 2,  //additive
        Charge                      = 3,  //additive
        MaterialCost                = 6,  //additive
        Material2Id                 = 7,  //fixed
        UnbuffedFailCap             = 8,  //additive
        ChargeIncrement_RightShift  = 9,  //additive
        OutputCount                 = 10, //multiplier
        AdditiveCost                = 14, //multiplier
        AdditiveVector              = 15, //multiplier
        AdditiveRadius              = 16, //multiplier
        SchematicDiscoveryRadius1   = 17, //multiplier
        SchematicDiscoveryRadius2   = 20, //multiplier
        ApSpSplitMaxDelta_LeftShift = 21, //additive
        Cost                        = 22, //multiplier
        CraftIsCritical             = 23, //bool
        AdditiveLimit               = 26, //additive
        AdditiveTier                = 27, //additive        
    }
}
