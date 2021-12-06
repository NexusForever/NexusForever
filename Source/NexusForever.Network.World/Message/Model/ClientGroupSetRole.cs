using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupSetRole)]
    public class ClientGroupSetRole : IReadable
    {
        public ulong GroupId { get; set; }

        public TargetPlayerIdentity TargetedPlayer { get; set; } = new TargetPlayerIdentity();

        public GroupMemberInfoFlags CurrentFlags { get; set; }

        public GroupMemberInfoFlags ChangedFlag { get; set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            TargetedPlayer.Read(reader);
            CurrentFlags = reader.ReadEnum<GroupMemberInfoFlags>(32u);
            ChangedFlag = reader.ReadEnum<GroupMemberInfoFlags>(32u);
        }
    }
}
