using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupInviteResponse)]
    public class ClientGroupInviteResponse : IReadable
    {
        public ulong GroupId { get; set; }
        public GroupInviteResult Result { get; set; }
        public uint Unk1 { get; set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            Result  = reader.ReadEnum<GroupInviteResult>(1);
            Unk1    = reader.ReadUInt();
        }
    }
}
