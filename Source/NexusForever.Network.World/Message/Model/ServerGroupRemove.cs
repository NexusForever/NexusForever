using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Tells a player they are no longer part of the group.
    /// Invokes the 'Group_Leave' event in Apollo. Which is invoked only for the current player.
    /// </summary>
    [Message(GameMessageOpcode.ServerGroupRemove)]
    public class ServerGroupRemove : IWritable
    {
        public ulong GroupId { get; set; }
        public uint Unk0 { get; set; }
        public TargetPlayerIdentity TargetPlayer { get; set; } = new TargetPlayerIdentity();
        public RemoveReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Unk0);

            TargetPlayer.Write(writer);
            writer.Write(Reason, 4u);
        }
    }
}
