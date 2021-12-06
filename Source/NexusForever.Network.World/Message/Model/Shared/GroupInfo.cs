using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class GroupInfo : IWritable
    {
        public ulong GroupId { get; set; }
        public GroupFlags Flags { get; set; }
        public List<GroupMemberInfo> MemberInfos { get; set; } = new List<GroupMemberInfo>();
        public uint MaxGroupSize { get; set; }

        public LootRule LootRule { get; set; }
        public LootRule LootRuleThreshold { get; set; }
        public LootThreshold LootThreshold { get; set; }
        public HarvestLootRule LootRuleHarvest { get; set; }

        public TargetPlayerIdentity LeaderIdentity { get; set; } = new TargetPlayerIdentity();
        public ushort RealmId { get; set; }     //< Why again? Tf?

        public GroupMarkerInfo MarkerInfo { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Flags, 32);
            writer.Write(MemberInfos.Count);
            writer.Write(MaxGroupSize);

            writer.Write(LootRule, 3u);
            writer.Write(LootRuleThreshold, 3u);
            writer.Write(LootThreshold, 4u);
            writer.Write(LootRuleHarvest, 2u);

            MemberInfos.ForEach(member => member.Write(writer));

            LeaderIdentity.Write(writer);
            writer.Write(RealmId, 14);

            MarkerInfo.Write(writer);
        }
    }
}
