using NexusForever.Network.Message;
using System.Diagnostics;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientStatisticsConnection)]
    public class ClientStatisticsConnection : IReadable
    {
        public uint AverageRoundTripInMs { get; private set; }
        public uint BytesReceivedPerSecond { get; private set; }
        public uint BytesSentPerSecond { get; private set; }
        public uint UnitHashTableEntryCount { get; private set; }
        public bool Unknown { get; private set; }

        public void Read(GamePacketReader reader)
        {
            AverageRoundTripInMs = reader.ReadUInt();
            BytesReceivedPerSecond = reader.ReadUInt();
            BytesSentPerSecond = reader.ReadUInt();

            uint temp = reader.ReadUInt();
            Unknown = (temp & 0x00000001) != 0;
            UnitHashTableEntryCount = temp >> 1;
        }
    }
}
