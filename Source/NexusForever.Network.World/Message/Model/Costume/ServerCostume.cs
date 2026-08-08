using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Costume
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
