using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingFlagsUpdate)]
    public class ClientHousingFlagsUpdate : IReadable
    {
        public Identity ResidenceIdentity { get; } = new();
        public ResidenceFlags Flags { get; private set; }
        public byte NeighbourHarvestSplit { get; private set; } // can never be more than 5
        public byte NeighbourGardenSplit { get; private set; } // can never be more than 5

        public void Read(GamePacketReader reader)
        {
            ResidenceIdentity.Read(reader);
            Flags = reader.ReadEnum<ResidenceFlags>(32u);
            NeighbourHarvestSplit = reader.ReadByte(3u);
            NeighbourGardenSplit = reader.ReadByte(3u);
        }
    }
}
