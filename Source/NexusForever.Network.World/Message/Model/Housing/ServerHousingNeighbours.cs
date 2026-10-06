using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingNeighbours)]
    public class ServerHousingNeighbours : IWritable
    {
        public List<Neighbour> Neighbours { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Neighbours.Count);
            Neighbours.ForEach(n => n.Write(writer));
        }
    }
}
