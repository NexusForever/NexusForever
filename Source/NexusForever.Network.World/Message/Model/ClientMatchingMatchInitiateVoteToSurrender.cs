using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingMatchInitiateVoteToSurrender)]
    public class ClientMatchingMatchInitiateVoteToSurrender : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // Zero size message
        }
    }
}
