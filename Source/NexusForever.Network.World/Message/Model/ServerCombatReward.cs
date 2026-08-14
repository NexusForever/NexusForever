using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Used by <see cref="CurrencyManager"/>, and also used during Combat to provide Momentum Boosts.
    /// </summary>
    [Message(GameMessageOpcode.ServerCombatReward)]
    public class ServerCombatReward : IWritable
    {
        public enum CombatRewardType
        {
            Currency_Credits                   = 0x0, // NewValue is total currency amount
            Currency_Renown                    = 0x1,
            Currency_ElderGems                 = 0x2,
            Currency_CraftingVouchers          = 0x3,
            Currency_Prestige                  = 0x4,
            Currency_ShadeSilver               = 0x5,
            Currency_Glory                     = 0x6,
            Currency_ProtoCoins                = 0x8,
            Currency_Triploons                 = 0x9,
            Currency_RedEssence                = 0xA,
            Currency_BlueEssence               = 0xB,
            Currency_GreenEssence              = 0xC,
            Currency_PurpleEssence             = 0xD,
            Currency_GroupCurrency             = 0xE,
            // values up through 0x15 reserved for currencies
            ItemProficiency                    = 0x16, // NewValue is ItemProficiency flags
            //
            CombatMomentum_Impulse            = 0x17, // NewValue is count  
            CombatMomentum_KillingPerformance = 0x18, // NewValue is count
            CombatMomentum_KillChain          = 0x19, // NewValue is count
            CombatMomentum_Evade              = 0x1A,
            CombatMomentum_Interrupt          = 0x1B,
            CombatMomentum_CCBreak            = 0x1C,
            //
            WakeHereCooldown                  = 0x1D, // NewValue is wake here cooldown length
            MoneyTradeLimit                   = 0x1E, // NewValue is money trade limit 
        }

        public CombatRewardType Stat { get; set; }
        public ulong NewValue { get; set; }
        public uint CombatRewardId { get; set; } // only used for CombatRewardTypes of CombatMomentum_
        public uint TargetUnitId { get; set; } 

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Stat, 5);
            writer.Write(NewValue);
            writer.Write(CombatRewardId);
            writer.Write(TargetUnitId);
        }
    }
}
