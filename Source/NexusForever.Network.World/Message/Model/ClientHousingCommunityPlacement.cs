using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingCommunityPlacement)]
    public class ClientHousingCommunityPlacement : IReadable
    {
        public TargetResidence TargetResidence { get; } = new();
        public uint PropertyIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
            PropertyIndex = reader.ReadUInt();
        }
    }
}
