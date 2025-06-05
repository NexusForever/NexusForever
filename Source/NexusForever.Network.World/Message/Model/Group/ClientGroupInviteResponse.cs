using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupInviteResponse)]
    public class ClientGroupInviteResponse : IReadable
    {
        public ulong InviteId { get; private set; }
        public GroupInviteResponse Result { get; private set; }
        public uint Unknown { get; private set; } // Client does set this in GroupLib.AcceptInvite

        public void Read(GamePacketReader reader)
        {
            InviteId = reader.ReadULong();
            Result  = reader.ReadEnum<GroupInviteResponse>(1);
            Unknown    = reader.ReadUInt();
        }
    }
}
