using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingPlotUpdate)]
    public class ServerHousingPlotUpdate : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong ResidenceId { get; set; }
        public uint PlotIndex { get; set; }
        public uint BuildStage { get; set; }
        public byte BuildState { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(ResidenceId);
            writer.Write(PlotIndex);
            writer.Write(BuildStage);
            writer.Write(BuildState, 3u);
        }
    }
}
