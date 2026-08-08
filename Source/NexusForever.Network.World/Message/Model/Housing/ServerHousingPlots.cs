using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingPlots)]
    public class ServerHousingPlots : IWritable
    {
        public class Plot : IWritable
        {
            public uint PlotPropertyIndex { get; set; }
            public uint PlugItemId { get; set; }
            public HousingPlugFacing PlugFacing { get; set; }
            public uint PlotInfoId { get; set; }
            public uint[] HousingContributionTotals { get; } = new uint[5];
            public uint[] HousingDecayTotals { get; } = new uint[5];
            public uint HousingUpkeepCharges { get; set; }
            public float HousingUpkeepTime { get; set; }
            public BuildState BuildState { get; set; }
            public uint BuildStage { get; set; } // changes the model's sequenceId
            public float BuildBonus { get; set; } // 0.0f to 1.0f, higher is faster build
            public float BuildStartTime { get; set; }

            public Plot()
            {
                Array.Fill(HousingContributionTotals, 0u);
                Array.Fill(HousingDecayTotals, 0u);
            }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(PlotPropertyIndex);
                writer.Write(PlugItemId);
                writer.Write(PlugFacing, 32u);
                writer.Write(PlotInfoId);

                for (uint i = 0u; i < HousingContributionTotals.Length; i++)
                    writer.Write(HousingContributionTotals[i]);

                for (uint i = 0u; i < HousingDecayTotals.Length; i++)
                    writer.Write(HousingDecayTotals[i]);

                writer.Write(HousingUpkeepCharges);
                writer.Write(HousingUpkeepTime);
                writer.Write(BuildState, 3u);
                writer.Write(BuildStage);
                writer.Write(BuildBonus);
                writer.Write(BuildStartTime);
            }
        }

        public Identity ResidenceIdentity { get; set; } 
        public List<Plot> Plots { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            ResidenceIdentity.Write(writer);

            writer.Write(Plots.Count);
            Plots.ForEach(p => p.Write(writer));
        }
    }
}
