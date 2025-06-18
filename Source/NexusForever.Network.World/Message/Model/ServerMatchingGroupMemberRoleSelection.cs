using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingGroupMemberRoleSelection)]
    public class ServerMatchingGroupMemberRoleSelection : IWritable
    {
        public TargetPlayerIdentity Identity { get; set; } = new();
        public Role Role { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Identity.Write(writer);
            writer.Write(Role, 32u);
        }
    }
}
