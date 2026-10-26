using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Sent when the lua function CharacterScreenLib::GetRealmTransferDestinations is called
    // Response from ServerTransferDestinationRealmList
    [Message(GameMessageOpcode.ClientGetRealmTransferDestinations)]
    public class ClientGetRealmTransferDestinations : IReadable
    {
        public bool Unknown0 { get; private set; } // the client writes one bit; purpose unknown

        public void Read(GamePacketReader reader)
        {
            Unknown0 = reader.ReadBit();
        }
    }
}
