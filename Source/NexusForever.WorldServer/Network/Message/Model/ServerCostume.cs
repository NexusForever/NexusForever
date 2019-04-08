using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
