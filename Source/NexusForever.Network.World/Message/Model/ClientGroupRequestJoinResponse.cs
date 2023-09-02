using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupRequestJoinResponse)]
    public class ClientGroupRequestJoinResponse : IReadable
    {
        public ulong GroupId { get; set; }

        public bool AcceptedRequest { get; set; }

        public string InviteeName { get; set; }


        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            AcceptedRequest = reader.ReadBit();
            InviteeName = reader.ReadWideString();
        }
    }
}
