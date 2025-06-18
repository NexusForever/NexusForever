using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    // Can only be sent after dueling has begun with ServerDuelStart (0x895)
    [Message(GameMessageOpcode.ClientDuelForfeit)]
    public class ClientDuelForfeit : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
