using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerInfoRequest)]
    public class ClientPlayerInfoRequest : IReadable
    {
        public byte Type { get; private set; }
        public TargetPlayerIdentity Identity { get; } = new();

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadByte(4u);
            Identity.Read(reader);
        }
    }
}
