using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingRenameProperty)]
    public class ClientHousingRenameProperty : IReadable
    {
        public TargetResidence TargetResidence { get; } = new();
        public string Name { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
            Name = reader.ReadWideString();
        }
    }
}
