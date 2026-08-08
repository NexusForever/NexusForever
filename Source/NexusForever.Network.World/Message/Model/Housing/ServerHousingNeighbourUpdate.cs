using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingNeighbourUpdate)]
    public class ServerHousingNeighbourUpdate : IWritable
    {
        public Neighbour Neighbour { get; set; }
        public NeighbourResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Neighbour.Write(writer);
            writer.Write(Result, 7u);
        }
    }
}
