using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientStatisticsWatchdog)]
    public class ClientStatisticsWatchdog : IReadable
    {
        public ulong PropertyHashSeed { get; private set; }
        public ulong PlayerUnitPropertiesHash { get; private set; } // Client iterates over the entire Property array of the player's unit to calculate this hash.
        public int LongestTimeBetweenWatchdogLoops { get; private set; } // An average of the last 30 watchdog loop times. Is approximately 30s when performance is good.
        public float TimeToMiddleOfCircularBuffer { get; private set; } // Should be not much higher than 1000ms when good.
        public float TimeBetweenWatchdogRunsWeightedAverage { get; private set; }
        public float WatchdogWeightedAverageError { get; private set; }
        public uint UnknownPlayerPositionRelated { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PropertyHashSeed = reader.ReadULong();
            PlayerUnitPropertiesHash = reader.ReadULong(64); 
            LongestTimeBetweenWatchdogLoops = reader.ReadInt();
            TimeToMiddleOfCircularBuffer = reader.ReadSingle();
            TimeBetweenWatchdogRunsWeightedAverage = reader.ReadSingle();
            WatchdogWeightedAverageError = reader.ReadSingle();
            UnknownPlayerPositionRelated = reader.ReadUInt();
        }
    }
}
