using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingRenameProperty)]
    public class ClientHousingRenameProperty : IReadable
    {
        public Identity TargetResidence { get; } = new();
        public string Name { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
            Name = reader.ReadWideString();
        }
    }
}
