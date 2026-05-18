using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Used by <see cref="CurrencyManager"/>, and also used during Combat to provide Momentum Boosts.
    /// </summary>
    [Message(GameMessageOpcode.ServerCombatReward)]
    public class ServerCombatReward : IWritable
    {
        public byte Stat { get; set; }
        public ulong NewValue { get; set; }
        public uint CombatRewardId { get; set; }
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
