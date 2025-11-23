using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Fortunes
{
    [Message(GameMessageOpcode.ClientFortunesFlipCard)]
    public class ClientFortunesFlipCard : IReadable
    {
        public uint SelectedCardIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            SelectedCardIndex = reader.ReadUInt();
        }
    }
}
