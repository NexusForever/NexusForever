using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server06BA, MessageDirection.Server)]
    public class Server06BA : IWritable
    {
        public class Mission : IWritable
        {
            public uint MissionId { get; set; }
            public bool Completed { get; set; }
            public uint ProgressPercent { get; set; }
            public uint MissionStep { get; set; }
            public byte Unknown4 { get; set; } 
            public uint Unknown5 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(MissionId, 15);
                writer.Write(Completed);
                writer.Write(ProgressPercent);
                writer.Write(MissionStep);
                writer.Write(Unknown4, 3);
                writer.Write(Unknown5);
            }
        }

        public List<Mission> Missions { get; set; } = new List<Mission>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Missions.Count);
            Missions.ForEach(e => e.Write(writer));
        }
    }
}
