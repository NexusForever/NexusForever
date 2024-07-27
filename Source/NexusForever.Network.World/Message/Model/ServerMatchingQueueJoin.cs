using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingQueueJoin)]
    public class ServerMatchingQueueJoin : IWritable
    {
        public class Map : IWritable
        {
            public Game.Static.Matching.MatchType MatchType { get; set; }
            public List<uint> Maps { get; set; } = [];
            public ushort MatchingGameType { get; set; }
            public MatchingQueueFlags QueueFlags { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(MatchType, 5u);

                writer.Write(Maps.Count);
                foreach (var map in Maps)
                    writer.Write(map);

                writer.Write(MatchingGameType, 14u);
                writer.Write(QueueFlags, 32u);
            }
        }

        public class Queue : IWritable
        {
            public Game.Static.Matching.MatchType MatchType { get; set; }
            public bool IsParty { get; set; }
            public uint QueueTime { get; set; }
            public uint EstimatedWaitTime { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(MatchType, 5u);
                writer.Write(IsParty);
                writer.Write(QueueTime);
                writer.Write(EstimatedWaitTime);
            }
        }

        public Map MapData { get; set; }
        public Queue QueueData { get; set; }
        public void Write(GamePacketWriter writer)
        {
            MapData.Write(writer);
            QueueData.Write(writer);

            writer.Write(0);
        }
    }
}
