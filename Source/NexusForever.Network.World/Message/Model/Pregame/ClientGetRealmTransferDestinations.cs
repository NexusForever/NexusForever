using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Sent when the lua function CharacterScreenLib::GetRealmTransferDestinations is called
    // Response from ServerTransferDestinationRealmList
    [Message(GameMessageOpcode.ClientGetRealmTransferDestinations)]
    public class ClientGetRealmTransferDestinations : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte messages
        }
    }
}
