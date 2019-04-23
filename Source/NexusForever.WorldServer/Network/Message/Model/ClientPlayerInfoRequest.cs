using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerInfoRequest)]
    public class ClientPlayerInfoRequest : IReadable
    {
        public byte Type { get; private set; }
        public TargetPlayerIdentity Identity { get; } = new TargetPlayerIdentity();

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadByte(4u);
            Identity.Read(reader);
        }
    }
}
