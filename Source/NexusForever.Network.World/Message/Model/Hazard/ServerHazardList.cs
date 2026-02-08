using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Hazard
{
    [Message(GameMessageOpcode.ServerHazardList)]
    public class ServerHazardList : IWritable
    {
        public List<Hazard> Hazards { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Hazards.Count);
            Hazards.ForEach(hazard => hazard.Write(writer));
        }
    }
}
