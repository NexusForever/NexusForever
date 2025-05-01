namespace NexusForever.Network.World.Message.Model.Shared
{
    public class CraftingLib
    {
        public enum CraftingDirection
        {
            None    = 0x0,
            N       = 0x1,
            NE      = 0x2,
            E       = 0x3,
            SE      = 0x4,
            S       = 0x5,
            SW      = 0x6,
            W       = 0x7,
            NW      = 0x8,
        };

        public enum CraftingDiscovery
        {
            Cold    = 0x0,
            Warm    = 0x1,
            Hot     = 0x2,
            Success = 0x3,
        };

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

        public enum RuneType
        {
            Air     = 0x7,
            Fire    = 0xA,
            Water   = 0x8,
            Earth   = 0x9,
            Logic   = 0xB,
            Life    = 0xC,
            Fusion  = 0xD,
        };

        public enum TradeskillResult
        {
            Success                  = 0x0,
            InsufficentFund          = 0x1,
            InvalidItem              = 0x2,
            InvalidSlot              = 0x3,
            MissingEngravingStation  = 0x4,
            Unlocked                 = 0x5,
            UnknownError             = 0x6,
            RuneExists               = 0x7,
            MissingRune              = 0x8,
            DuplicateRune            = 0x9,
            AttemptFailed            = 0xA,
            RuneSlotLimit            = 0xB,
        };
    }
}
