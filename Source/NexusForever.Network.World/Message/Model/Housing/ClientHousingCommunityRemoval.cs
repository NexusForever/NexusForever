using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingCommunityRemoval)]
    public class ClientHousingCommunityRemoval : IReadable
    {
        public Identity TargetResidence { get; private set; } = new(); // This is only ever the player's own residence

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
        }
    }
}
