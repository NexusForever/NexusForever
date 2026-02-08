using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerWarPlotPlugState)]
    public class ServerWarPlotPlugState : IWritable
    {
        public uint CratedDecorCount { get; set; }
        public ushort Unknown1 { get; set; }
        public ushort PlacedDecorCount { get; set; }
        public List<WarPlotPlug> Plugs { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CratedDecorCount);
            writer.Write(Unknown1);
            writer.Write(PlacedDecorCount);
            writer.Write(Plugs.Count, 8u);
            Plugs.ForEach(plug => plug.Write(writer));
        }
    }
}
