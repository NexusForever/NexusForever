using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Group;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupJoin)]
    public class ServerGroupJoin : IWritable
    {
        public Identity Player { get; set; }
        public GroupInfo GroupInfo { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Player.Write(writer);
            GroupInfo.Write(writer);
        }
    }
}
