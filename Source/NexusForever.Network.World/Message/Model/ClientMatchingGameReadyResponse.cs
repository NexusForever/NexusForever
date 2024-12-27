using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingMatchReadyResponse)]
    public class ClientMatchingGameReadyResponse : IReadable
    {
        public bool Response { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Response = reader.ReadBit();
        }
    }
}
