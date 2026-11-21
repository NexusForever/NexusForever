using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicFlags)]
    public class ServerCinematicFlags : IWritable
    {
        public uint Delay { get; set; }
        public uint Flags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Flags);
        }
    }
}
