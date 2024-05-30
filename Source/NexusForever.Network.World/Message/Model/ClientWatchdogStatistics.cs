using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientWatchdogStatistics)]
    public class ClientWatchdogStatistics : IReadable
    {
        public ulong RandomValueSeed { get; private set; }
        public ulong RandomValue { get; private set; }
        public int LongestTimeBetweenWatchdogLoops { get; private set; }
        public float TimeToMiddleOfCircularBuffer { get; private set; }
        public float TimeBetweenWatchdogRunsWeightedAverage { get; private set; }
        public float WatchdogWeightedAverageError { get; private set; }
        public uint UnknownPlayerRelated { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RandomValueSeed = reader.ReadULong();
            RandomValue = reader.ReadULong(64);
            LongestTimeBetweenWatchdogLoops = reader.ReadInt();
            TimeToMiddleOfCircularBuffer = reader.ReadSingle();
            TimeBetweenWatchdogRunsWeightedAverage = reader.ReadSingle();
            WatchdogWeightedAverageError = reader.ReadSingle();
            UnknownPlayerRelated = reader.ReadUInt();
        }
    }
}
