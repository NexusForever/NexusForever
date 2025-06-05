using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMemberInstanceUpdate)]
    public class ServerGroupMemberInstanceUpdate : IWritable
    {
        public ulong GroupId { get; set; }
        public Identity Identity { get; set; }
        public uint RealmId { get; set; }
        public uint ZoneId { get; set; }
        public uint MapId { get; set; }
        public uint PhaseId { get; set; }
        public bool InGroupInstance { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            Identity.Write(writer);
            writer.Write(RealmId, 14);
            writer.Write(ZoneId, 15);
            writer.Write(MapId);
            writer.Write(PhaseId);
            writer.Write(InGroupInstance);
        }
    }
}
