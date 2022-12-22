using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientChangeActiveActionSet)]
    public class ClientChangeActiveActionSet : IReadable
    {
        public byte ActionSetIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ActionSetIndex = reader.ReadByte(3u);
        }
    }
}
