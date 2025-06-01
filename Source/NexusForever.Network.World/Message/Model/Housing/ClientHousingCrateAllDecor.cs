using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{

    [Message(GameMessageOpcode.ClientHousingCrateAllDecor)]
    public class ClientHousingCrateAllDecor : IReadable
    {
        public Identity TargetResidence { get; } = new();

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
        }
    }
}
