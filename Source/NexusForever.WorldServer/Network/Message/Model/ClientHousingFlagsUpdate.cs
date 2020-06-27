using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingFlagsUpdate)]
    public class ClientHousingFlagsUpdate : IReadable
    {
        public TargetPlayerIdentity Identity { get; } = new TargetPlayerIdentity();
        public ResidenceFlags Flags { get; private set; }
        public byte ResourceSharing { get; private set; }
        public byte GardenSharing { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Identity.Read(reader);
            Flags           = reader.ReadEnum<ResidenceFlags>(32u);
            ResourceSharing = reader.ReadByte(3u);
            GardenSharing   = reader.ReadByte(3u);
        }
    }
}
