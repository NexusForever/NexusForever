using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientStatisticsFramerate)]
    public class ClientStatisticsFramerate : IReadable
    {
        public uint RecentAverageFrameTime { get; private set; }
        public bool Unknown1 { get; private set; }
        public uint HighestTimeFrame { get; private set; }
        public bool Unknown2 { get; private set; }
        public Position PositionAtSlowestFrame { get; private set; } = new Position();
        public float GameTimeStepPreecedingSlowestFrame { get; private set; } // value is often a negative value, unsure why
        public uint EntireSessionAverageFrameTime { get; private set; }
        public bool Unknown3 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            uint temp = reader.ReadUInt();
            Unknown1 = (temp & 0x00000001) != 0;
            RecentAverageFrameTime = temp >> 1;

            temp = reader.ReadUInt();
            Unknown2 = (temp & 0x00000001) != 0;
            HighestTimeFrame = temp >> 1;

            PositionAtSlowestFrame.Read(reader);
            GameTimeStepPreecedingSlowestFrame = reader.ReadSingle();

            temp = reader.ReadUInt();
            Unknown3 = (temp & 0x00000001) != 0;
            EntireSessionAverageFrameTime = temp >> 1;
        }
    }
}
