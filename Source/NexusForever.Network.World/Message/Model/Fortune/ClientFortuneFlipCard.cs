using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortune
{
    [Message(GameMessageOpcode.ClientFortuneFlipCard)]
    public class ClientFortuneFlipCard : IReadable
    {
        public uint SelectedCardIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            SelectedCardIndex = reader.ReadUInt();
        }
    }
}
