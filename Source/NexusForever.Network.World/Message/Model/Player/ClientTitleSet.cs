using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ClientTitleSet)]
    public class ClientTitleSet : IReadable
    {
        public ushort CharacterTitleId { get; set; }

        public void Read(GamePacketReader reader)
        {
            CharacterTitleId = reader.ReadUShort(14u);
        }
    }
}
