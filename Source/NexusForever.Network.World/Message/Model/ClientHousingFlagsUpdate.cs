using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingFlagsUpdate)]
    public class ClientHousingFlagsUpdate : IReadable
    {
        public TargetResidence TargetResidence { get; } = new();
        public ResidenceFlags Flags { get; private set; }
        public byte ResourceSharing { get; private set; }
        public byte GardenSharing { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
            Flags           = reader.ReadEnum<ResidenceFlags>(32u);
            ResourceSharing = reader.ReadByte(3u);
            GardenSharing   = reader.ReadByte(3u);
        }
    }
}
