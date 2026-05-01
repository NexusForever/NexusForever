using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    public class WarPlotPlug : IWritable
    {
        public byte Index { get; set; }
        public uint Health { get; set; }
        public uint HealthMax { get; set; }
        public byte Tier { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Index);
            writer.Write(Health);
            writer.Write(HealthMax);
            writer.Write(Tier, 2u);
        }
    }
}
