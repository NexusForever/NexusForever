using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPublicEventObjectiveBusy)]
    public class ServerPublicEventObjectiveBusy : IWritable
    {
        public bool Busy { get; set; }
        public uint ElapsedTimeMs { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Busy);
            writer.Write(ElapsedTimeMs);
        }
    }
}
