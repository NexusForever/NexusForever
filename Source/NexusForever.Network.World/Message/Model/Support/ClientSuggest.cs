using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Support
{
    // From the /suggest command
    [Message(GameMessageOpcode.ClientSuggest)]
    public class ClientSuggest : IReadable
    {
        public string SuggestionText { get; private set; }

        public void Read(GamePacketReader reader)
        {
            SuggestionText = reader.ReadWideString();
        }
    }
}
