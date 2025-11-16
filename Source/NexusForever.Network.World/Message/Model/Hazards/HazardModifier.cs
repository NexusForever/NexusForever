using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Hazards
{
    public class HazardModifier : IWritable
    {
        public float Multiplier { get; set; }
        public float Offset { get; set; }
        public bool bSuspended { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Multiplier);
            writer.Write(Offset);
            writer.Write(bSuspended);
        }
    }
}
