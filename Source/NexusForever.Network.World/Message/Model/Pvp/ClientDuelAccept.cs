using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    // Can only be sent after being identified as OpponentUnitId in ServerDuelChallenge (0x896)
    [Message(GameMessageOpcode.ClientDuelAccept)]
    public class ClientDuelAccept : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
