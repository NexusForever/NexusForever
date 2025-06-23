using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Invokes the 'Group_Leave' event in Apollo when someone else left the group.
    /// </summary>
    [Message(GameMessageOpcode.ServerGroupRemove)]
    public class ServerGroupRemove : IWritable
    {
        public ulong GroupId { get; set; }
        public uint GroupMessageIndex { get; set; } // Unpacked but unused by client
        public Identity TargetPlayer { get; set; }
        public RemoveReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(GroupMessageIndex);
            TargetPlayer.Write(writer);
            writer.Write(Reason, 4u);
        }
    }
}
