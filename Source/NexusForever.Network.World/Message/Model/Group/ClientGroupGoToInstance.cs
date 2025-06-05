using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupGoToInstance)]
    public class ClientGroupGoToInstance : IReadable
    {
        public ulong GroupId { get; private set; } // GroupId of instance group player is in

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
        }
    }
}
