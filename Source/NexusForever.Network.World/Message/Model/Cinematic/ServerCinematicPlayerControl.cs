using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicPlayerControl)]
    public class ServerCinematicPlayerControl : IWritable
    {
        public uint Delay { get; set; }
        public bool Allow { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Allow);
        }
    }
}
