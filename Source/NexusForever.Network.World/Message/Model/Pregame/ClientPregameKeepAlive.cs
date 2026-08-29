using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Client sents this every 60 seconds while on the CharacterSelect or RealmSelect screen.
    // Presumably sent to keep the TCP socket alive as the client is not sending any other messages.
    [Message(GameMessageOpcode.ClientPregameKeepAlive)]
    public class ClientPregameKeepAlive : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero byte message
        }
    }
}
