using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupRequestJoinResult)]
    public class ServerGroupRequestJoinResult : IWritable
    {
        public ulong GroupId { get; set; }

        public string Name { get; set; }

        public GroupResult Result { get; set; }

        public bool IsJoin { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.WriteStringWide(Name);
            writer.Write(Result, 5u);
            writer.Write(IsJoin);
        }
    }
}
