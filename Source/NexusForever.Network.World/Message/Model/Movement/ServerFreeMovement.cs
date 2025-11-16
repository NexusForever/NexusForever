using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFreeMovement)]
    public class ServerFreeMovement : IWritable
    {
        public bool Allowed { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Allowed == true ? 1u : 0u, 32u);
        }
    }
}
