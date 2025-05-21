using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupReferral)]
    public class ServerGroupReferral : IWritable
    {
        public ulong GroupId { get; set; }
        public Identity InvokerIdentity { get; set; } // Member of party that made referral
        public string InviteeName { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            InvokerIdentity.Write(writer);
            writer.WriteStringWide(InviteeName);
        }
    }
}
