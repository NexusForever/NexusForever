using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupInviteResult)]
    public class ServerGroupInviteResult : IWritable
    {
        public ulong GroupId { get; set; }
        public string Name { get; set; }
        public GroupResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.WriteStringWide(Name);
            writer.Write(Result, 5);
        }
    }
}
