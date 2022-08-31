using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
