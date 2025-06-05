using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupSetTargetMark)]
    public class ClientGroupSetTargetMark : IReadable
    {
        public GroupMarker TargetMarkerId { get; private set; }
        public uint UnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetMarkerId = reader.ReadEnum<GroupMarker>(32u);
            UnitId = reader.ReadUInt();
        }
    }
}
