using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPathMissionUpdate)]
    public class ServerPathMissionUpdate : IWritable
    {
        public ushort MissionId { get; set; }
        public bool Completed { get; set; }
        public uint Userdata { get; set; }
        public uint Statedata { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MissionId, 15);
            writer.Write(Completed);
            writer.Write(Userdata);
            writer.Write(Statedata);
        }
    }
}
