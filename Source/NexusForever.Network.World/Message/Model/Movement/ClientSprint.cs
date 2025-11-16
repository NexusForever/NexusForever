using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientSprint)]
    public class ClientSprint : IReadable
    {
        public bool Sprint { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Sprint = reader.ReadBit();
        }
    }
}
