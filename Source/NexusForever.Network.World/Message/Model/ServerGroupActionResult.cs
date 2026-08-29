using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupActionResult)]
    public class ServerGroupActionResult : IWritable
    {
        public ulong GroupId { get; set; }
        public Identity Identity { get; set; }
        public GroupActionResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            Identity.Write(writer);
            writer.Write(Result, 32);
        }
    }
}
