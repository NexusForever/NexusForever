using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    [Message(GameMessageOpcode.ClientCharacterList)]
    public class ClientCharacterList : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
