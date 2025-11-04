using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicFullScreenEffectAdd)]
    public class ServerCinematicFullScreenEffectAdd : IWritable
    {
        public uint Delay { get; set; }
        public uint FullScreenEffectId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(FullScreenEffectId);
        }
    }
}
