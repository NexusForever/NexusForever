using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Hazard
{
    public class HazardModifier : IWritable
    {
        public float Multiplier { get; set; }
        public float Offset { get; set; }
        public bool Suspended { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Multiplier);
            writer.Write(Offset);
            writer.Write(Suspended);
        }
    }
}
