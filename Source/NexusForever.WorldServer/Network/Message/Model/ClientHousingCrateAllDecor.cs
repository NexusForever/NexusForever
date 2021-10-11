using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingCrateAllDecor)]
    public class ClientHousingCrateAllDecor : IReadable
    {
        public TargetResidence TargetResidence { get; } = new();

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
        }
    }
}
