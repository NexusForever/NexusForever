using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingRoleCheckResponse)]
    public class ClientMatchingRoleCheckResponse : IReadable
    {
        public uint Unknown1 { get; private set; }
        public Role Roles { get; private set; }
        public bool Response { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown1 = reader.ReadByte(5u);
            Roles    = reader.ReadEnum<Role>();
            Response = reader.ReadBit();
        }
    }
}
