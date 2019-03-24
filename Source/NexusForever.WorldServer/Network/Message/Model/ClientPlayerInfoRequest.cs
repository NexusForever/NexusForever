using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerInfoRequest, MessageDirection.Client)]
    public class ClientPlayerInfoRequest : IReadable
    {
        public byte Unknown0 { get; private set; }
        public ushort Unknown1 { get; private set; }
        public ulong CharacterId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown0 = reader.ReadByte(4);
            Unknown1 = reader.ReadUShort(14);
            CharacterId = reader.ReadULong();
        }
    }
}
