using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicScene)]
    public class ServerCinematicScene : IWritable
    {
        public uint Delay { get; set; }
        public uint SceneId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(SceneId);
        }
    }
}
