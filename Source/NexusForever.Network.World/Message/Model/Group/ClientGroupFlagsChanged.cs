using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupFlagsChanged)]
    public class ClientGroupFlagsChanged : IReadable
    {
        public ulong GroupId { get; private set; }
        public GroupFlags NewFlags { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            NewFlags = reader.ReadEnum<GroupFlags>(32u);
        }
    }
}
