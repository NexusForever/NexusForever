using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupPositionUpdate)]
    public class ServerGroupPositionUpdate : IWritable
    {
        public class GroupMemberState
        {
            public enum MemberCombatState
            {
                OutOfCombat = 0x0,
                InCombatPvP = 0x1,
                InCombatPvE = 0x2,
                InCombat    = 0x3,
            }

            public Identity Identity { get; set; }
            public Position Position { get; set; }
            public uint WorldZoneId { get; set; }
            public MemberCombatState CombatState { get; set; } = 0; 
        }

        public ulong GroupId { get; set; }
        public uint WorldId { get; set; }
        public List<GroupMemberState> Updates { get; set; } = new List<GroupMemberState>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(WorldId, 15);

            writer.Write((uint)Updates.Count);
            Updates.ForEach(update => update.Identity.Write(writer));
            Updates.ForEach(update => update.Position.Write(writer));
            Updates.ForEach(update => writer.Write(update.WorldZoneId));
            Updates.ForEach(update => writer.Write(update.CombatState));
        }
    }
}
