using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server06BB, MessageDirection.Server)]
    public class Server06BB : IWritable
    {
        public ushort MissionId { get; set; }
        public bool Completed { get; set; }
        public uint ProgressPercent { get; set; }
        public uint MissionStep { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MissionId, 15);
            writer.Write(Completed);
            writer.Write(ProgressPercent);
            writer.Write(MissionStep);
        }
    }
}
