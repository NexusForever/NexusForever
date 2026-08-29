using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingTakeMeHome)]
    public class ClientHousingTakeMeHome : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
