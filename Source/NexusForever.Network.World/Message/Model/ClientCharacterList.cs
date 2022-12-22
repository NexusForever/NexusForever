using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCharacterList)]
    public class ClientCharacterList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}
