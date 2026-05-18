using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingCommunityRemoval)]
    public class ClientHousingCommunityRemoval : IReadable
    {
        public TargetResidence TargetResidence { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetResidence = new TargetResidence();
            TargetResidence.Read(reader);
        }
    }
}
