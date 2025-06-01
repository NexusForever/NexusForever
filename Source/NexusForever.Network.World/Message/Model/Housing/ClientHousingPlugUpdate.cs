using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingPlugUpdate)]
    public class ClientHousingPlugUpdate : IReadable
    {
        public class HousingContribution : IReadable
        {
            public uint ContributionPointRequirement { get; private set; } // derived from tbl housingPlugItem.housingContributionInfoId
            public uint Unused1 { get; private set; }
            public uint Unused2 { get; private set; }
            public uint Unused3 { get; private set; }
            public uint Unused4 { get; private set; }

            public void Read(GamePacketReader reader)
            {
                ContributionPointRequirement = reader.ReadUInt();
                Unused1 = reader.ReadUInt();
                Unused2 = reader.ReadUInt();
                Unused3 = reader.ReadUInt();
                Unused4 = reader.ReadUInt();
            }
        }

        public Identity ResidenceIdentity { get; private set; } = new();
        public uint HousingPlotInfoId { get; private set; }
        public uint HousingPlugItemId { get; private set; }
        public uint PlugFacing { get; private set; }
        public uint PlotFlags { get; private set; } // unused
        public HousingPlugOperation Operation { get; private set; }
        public HousingContribution[] HousingContributions { get; private set; } = new HousingContribution[5];

        public void Read(GamePacketReader reader)
        {
            ResidenceIdentity.Read(reader);

            HousingPlotInfoId = reader.ReadUInt();
            HousingPlugItemId = reader.ReadUInt();
            PlugFacing = reader.ReadUInt();
            PlotFlags = reader.ReadUInt();
            Operation = reader.ReadEnum<HousingPlugOperation>(3u);

            for (int i = 0; i < HousingContributions.Length; i++)
            {
                HousingContributions[i].Read(reader);
            }
        }
    }
}
