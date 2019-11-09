using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientInspectPlayerRequest)]
    public class ClientInspectPlayerRequest : IReadable
    {
        public uint Guid { get; set; }

        public void Read(GamePacketReader reader)
        {
            Guid = reader.ReadUInt();
        }
    }
}
