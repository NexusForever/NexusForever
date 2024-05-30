using NexusForever.Network.Message;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientWatchdogStatistics)]
    public class ClientWatchdogStatistics : IReadable
    {
        public ulong randomValueSeed { get; private set; }
        public ulong randomValue { get; private set; }
        public int longestTimeBetweenWatchdogLoops { get; private set; }
        public float timeToMiddleOfCircularBuffer { get; private set; }
        public float timeBetweenWatchdogRunsWeightedAverage { get; private set; }
        public float watchdogWeightedAverageError { get; private set; }
        public uint unknownPlayerRelated { get; private set; }

        public void Read(GamePacketReader reader)
        {
            randomValueSeed = reader.ReadULong();
            randomValue = reader.ReadULong(64);
            longestTimeBetweenWatchdogLoops = reader.ReadInt();
            timeToMiddleOfCircularBuffer = reader.ReadSingle();
            timeBetweenWatchdogRunsWeightedAverage = reader.ReadSingle();
            watchdogWeightedAverageError = reader.ReadSingle();
            unknownPlayerRelated = reader.ReadUInt();
        }
    }
}
