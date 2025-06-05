using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupSetMemberFlags)]
    public class ClientGroupSetMemberFlags : IReadable
    {
        public ulong GroupId { get; private set; }
        public Identity TargetedPlayer { get; private set; } = new Identity();
        public GroupMemberInfoFlags CurrentFlags { get; private set; }
        public GroupMemberInfoFlags ChangedFlag { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            TargetedPlayer.Read(reader);
            CurrentFlags = reader.ReadEnum<GroupMemberInfoFlags>(32u);
            ChangedFlag = reader.ReadEnum<GroupMemberInfoFlags>(32u);
        }
    }
}
