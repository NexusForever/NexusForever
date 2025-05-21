using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupOperationResult)]
    public class ServerGroupOperationResult : IWritable
    {
        public ulong GroupId { get; private set; }
        public Identity Identity { get; private set; } = new Identity();
        public GroupActionResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            Identity.Write(writer);
            writer.Write(Result, 6);
        }
    }
}
