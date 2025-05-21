using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupJoinResponse)]
    public class ClientGroupJoinResponse : IReadable
    {
        public ulong GroupId { get; private set; }
        public bool AcceptedRequest { get; private set; }
        public string InviteeName { get; private set; }
        public JoinRequestType Type { get; private set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            AcceptedRequest = reader.ReadBit();
            InviteeName = reader.ReadWideString();
            Type = reader.ReadEnum<JoinRequestType>(1);
        }
    }
}
