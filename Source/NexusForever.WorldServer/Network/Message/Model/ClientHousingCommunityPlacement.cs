using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
