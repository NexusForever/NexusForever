using NexusForever.Game.Static.Matching;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class MatchingMap
    {
        public Game.Static.Matching.MatchType MatchType { get; private set; }
        public List<uint> Maps { get; } = [];
        public ushort MatchingGameTypeId { get; private set; }
        public MatchingQueueFlags QueueFlags { get; private set; }

        public void Read(GamePacketReader reader)
        {
            MatchType    = reader.ReadEnum<Game.Static.Matching.MatchType>(5u);
            var mapCount = reader.ReadUInt();

            for (uint i = 0; i < mapCount; i++)
                Maps.Add(reader.ReadUInt());

            MatchingGameTypeId = reader.ReadUShort(14u);
            QueueFlags         = reader.ReadEnum<MatchingQueueFlags>(32u);
        }
    }
}
