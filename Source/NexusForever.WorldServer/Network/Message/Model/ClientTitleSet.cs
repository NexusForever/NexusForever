using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientTitleSet)]
    public class ClientTitleSet : IReadable 
    {
        public ushort TitleId { get; set; }

        public void Read(GamePacketReader reader)
        {
            TitleId = reader.ReadUShort(14u);
        }
    }
}
