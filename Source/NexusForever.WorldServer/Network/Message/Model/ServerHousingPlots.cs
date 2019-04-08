using System;
using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
            public uint HousingUpkeepCharges{ get; set; }
            public float HousingUpkeepTime{ get; set; }
            public byte BuildState { get; set; }
            public uint BuildStage { get; set; }
            public float BuildBonus { get; set; }
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
        
        public ushort RealmId { get; set; }
        public ulong ResidenceId { get; set; }
        public List<Plot> Plots { get; set; } = new List<Plot>();
        
        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(ResidenceId);
            
            writer.Write(Plots.Count);
            Plots.ForEach(p => p.Write(writer));
        }
    }
}
