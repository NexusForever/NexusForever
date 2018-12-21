using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server06B5, MessageDirection.Server)]
    public class Server06B5 : IWritable
    {
        public class Mission : IWritable
        {
            public uint MissionId { get; set; }
            public bool Unknown1 { get; set; }
            public uint Unknown2 { get; set; }
            public uint Unknown3 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(MissionId, 15);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
            }
        }

        public ushort EpisodeId { get; set; }
        public List<Mission> Missions { get; set; } = new List<Mission>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EpisodeId, 14);
            writer.Write(Missions.Count);
            Missions.ForEach(e => e.Write(writer));
        }
    }
}
