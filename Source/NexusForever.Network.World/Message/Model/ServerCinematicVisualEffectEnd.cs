using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicVisualEffectEnd)]
    public class ServerCinematicVisualEffectEnd : IWritable
    {
        public uint Delay { get; set; }
        public uint VisualHandle { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(VisualHandle);
        }
    }
}
