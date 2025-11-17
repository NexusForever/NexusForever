using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Hazard
{
    [Message(GameMessageOpcode.ServerHazardModifiers)]
    public class ServerHazardModifiers : IWritable
    {
        public List<HazardModifier> HazardIdModifiers { get; set; } = [];
        public List<HazardModifier> HazardTypeModifiers { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(HazardIdModifiers.Count);
            HazardIdModifiers.ForEach(hazard => hazard.Write(writer));

            writer.Write(HazardTypeModifiers.Count);
            HazardTypeModifiers.ForEach(hazard => hazard.Write(writer));
        }
    }
}
