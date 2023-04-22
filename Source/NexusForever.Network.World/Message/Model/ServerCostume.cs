using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCostume)]
    public class ServerCostume : IWritable
    {
        public Costume Costume { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Costume.Write(writer);
        }
    }
}
