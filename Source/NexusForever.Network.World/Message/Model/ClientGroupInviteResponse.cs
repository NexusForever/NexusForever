using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupInviteResponse)]
    public class ClientGroupInviteResponse : IReadable
    {
        public ulong GroupId { get; set; }
        public bool Response { get; set; }
        public uint Unk1 { get; set; }

        public void Read(GamePacketReader reader)
        {
            GroupId  = reader.ReadULong();
            Response = reader.ReadBit();
            Unk1     = reader.ReadUInt();
        }
    }
}
