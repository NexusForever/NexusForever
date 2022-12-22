using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCostumeSet)]
    public class ClientCostumeSet : IReadable
    {
        public int Index { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Index = reader.ReadInt();
        }
    }
}
