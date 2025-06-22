using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingListPlayersQueuedForMap)]
    public class ServerMatchingListPlayersQueuedForMap : IWritable
    {
        public class UnknownStruct0 : IWritable
        {
            public ushort Unknown6 { get; set; } = 0;
            public byte Unknown7 { get; set; } = 48;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown6);
                writer.Write(Unknown7);
            }
        }

        public class PrimeLevelInfo : IWritable
        {
            public ushort WorldId { get; set; }
            public ushort PrimeLevelAchieved { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(WorldId, 15u);
                writer.Write(PrimeLevelAchieved);
            }
        }

        public class QueuedPlayerInfo : IWritable
        {
            public Identity Identity { get; set; }
            public string Name { get; set; }
            public Faction Faction { get; set; }
            public uint Race {  get; set; }
            public uint Class { get; set; }
            public uint Gender { get; set; }
            public uint Level { get; set; }
            public uint Path { get; set; }
            public uint field_30_32bit { get; set; }
            public bool field_34_1bit { get; set; }
            public uint field_38_32bit { get; set; }
            public float field_3C_32bit { get; set; }
            public UnknownStruct0[] SomeStatList = new UnknownStruct0[5];
            public List<PrimeLevelInfo> PrimeLevels { get; set; } = new List<PrimeLevelInfo>();

            public void Write(GamePacketWriter writer)
            {
                Identity.Write(writer);
                writer.WriteStringWide(Name);
                writer.Write(Faction, 14u);
                writer.Write(Race, 14u);
                writer.Write(Class, 14u);
                writer.Write(Gender, 14u);
                writer.Write(Level);
                writer.Write(Path, 3u);
                writer.Write(field_30_32bit);
                writer.Write(field_34_1bit);
                writer.Write(field_38_32bit);
                writer.Write(field_3C_32bit);
                foreach (var stat in SomeStatList)
                {
                    stat.Write(writer);
                }
                writer.Write(PrimeLevels.Count);
                PrimeLevels.ForEach(i => i.Write(writer));
            }

        }

        public uint MapId { get; set; }
        public List<QueuedPlayerInfo> QueuedPlayers { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MapId);
            writer.Write(QueuedPlayers.Count);
            foreach (var player in QueuedPlayers)
            {
                player.Write(writer);
            }
        }
    }
}
