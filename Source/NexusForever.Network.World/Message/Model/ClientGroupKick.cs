using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupKick)]
    public class ClientGroupKick : IReadable
    {
        public ulong GroupId { get; private set; }
        public Identity PlayerToKick { get; private set; } = new Identity();

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            PlayerToKick.Read(reader);
        }
    }
}
